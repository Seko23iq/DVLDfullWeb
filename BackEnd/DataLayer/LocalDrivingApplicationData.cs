using ContactsDataAccessLayer;
using DtoLayer.LicenseHistory;
using DtoLayer.LocalDrivingApplication;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Server;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static DataLayer.clsPersonData;
using static DataLayer.ShowDrivingLicenseData;
using static System.Net.Mime.MediaTypeNames;
namespace DataLayer
{
    public class LocalDrivingApplicationData
    {
        static string MainQuery = @"
SELECT        
LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID AS 'L.D.L.AppID', 
LicenseClasses.ClassName AS 'Driving Class', 
People.NationalNo AS 'National No.', 
People.FirstName + ' ' + People.SecondName + ' ' + ISNULL(People.ThirdName, '')  + ' ' + People.LastName AS 'Full Name',
Applications.ApplicationDate AS 'Application Date.',
(
SELECT COUNT(TestAppointments.TestTypeID) AS 'Passed Tests'
FROM Tests 
INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
WHERE  
(TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID)
AND 
(Tests.TestResult = 1)
) 
AS 'Passed Tests',

CASE 
WHEN Applications.ApplicationStatus = 1 THEN 'New' 
WHEN Applications.ApplicationStatus = 2 THEN 'Cancelled' 
WHEN Applications.ApplicationStatus = 3 THEN 'Completed' 
END AS Status

FROM LocalDrivingLicenseApplications 
INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID 
INNER JOIN LicenseClasses ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID 
INNER JOIN People ON Applications.ApplicantPersonID = People.PersonID";
        public static List<LocalDrivingApplicationDTO> GetAllRecords()
        {
            List<LocalDrivingApplicationDTO> applications = new List<LocalDrivingApplicationDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = MainQuery;

            SqlCommand command = new SqlCommand( MainQuery, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    applications.Add
                        (new LocalDrivingApplicationDTO
                        (
                        reader.GetInt32(reader.GetOrdinal("L.D.L.AppID")).ToString(),
                        reader.GetString(reader.GetOrdinal("Driving Class")),
                        reader.GetString(reader.GetOrdinal("National No.")),
                        reader.GetString(reader.GetOrdinal("Full Name")),
                        reader.IsDBNull(reader.GetOrdinal("Application Date.")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("Application Date.")),
                        reader.GetInt32(reader.GetOrdinal("Passed Tests")),
                        reader.GetString(reader.GetOrdinal("Status"))
                        )
                        );
                }

                reader.Close();
            }
            catch (Exception ex)  {throw ex;}
            finally               { connection.Close(); }

            return applications;
        }
        public enum ApplicationFilterType
        {
            AppID,
            FullName,
            Status
        }
        private static int MapStatusToInt(string status)
        {
            return status switch
            {
                "New" => 1,
                "Cancelled" => 2,
                "Completed" => 3,
                _ => throw new ArgumentException("Invalid status value")
            };
        }
        private static LocalDrivingApplicationDTO MapReaderToDTO(SqlDataReader reader)
        {
            return new LocalDrivingApplicationDTO(
                reader["L.D.L.AppID"].ToString(),
                reader["Driving Class"].ToString(),
                reader["National No."].ToString(),
                reader["Full Name"].ToString(),
                reader["Application Date."] == DBNull.Value
                    ? null
                    : (DateTime?)reader["Application Date."],
                Convert.ToInt32(reader["Passed Tests"]),
                reader["Status"].ToString()
            );
        }
        public static List<LocalDrivingApplicationDTO> GetByFilter(ApplicationFilterType filterType, string value)
        {
            var applications = new List<LocalDrivingApplicationDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;

                string whereClause = string.Empty;

                switch (filterType)
                {
                    case ApplicationFilterType.AppID:
                        whereClause = "CAST(LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID AS NVARCHAR) LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = value + "%";
                        break;

                    case ApplicationFilterType.FullName:
                        whereClause = @"People.FirstName + ' ' + People.SecondName + ' ' + 
                                ISNULL(People.ThirdName, '') + ' ' + People.LastName LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = value + "%";
                        break;

                    case ApplicationFilterType.Status:
                        whereClause = "Applications.ApplicationStatus = @value";
                        command.Parameters.Add("@value", SqlDbType.Int).Value = MapStatusToInt(value);
                        break;
                }

                command.CommandText = $"{MainQuery} WHERE {whereClause}";

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            applications.Add(MapReaderToDTO(reader));
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return applications;
        }
        public static List<string> GetLicenseClasses()
        {
            List<string> LicenseClassesList = new List<string>();


            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;

                string Query = "SELECT ClassName FROM LicenseClasses";
                command.CommandText = Query;

                try
                {
                    connection.Open();
                    using (SqlDataReader Reader = command.ExecuteReader())
                    {

                        while (Reader.Read())
                        {
                            LicenseClassesList.Add(Reader.GetString(0));
                        }
                    }
                }
                catch
                {
                    throw;
                }
            }

            return LicenseClassesList;
        }
        public static int AddNewLocalDrvingLicenseRecord_Data(AddNewLocalDrivingLicenseDTO dTO)
        {
            int L_D_DrivingAdded = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string Query = @"
        DECLARE @ApplicationID INT;

        INSERT INTO Applications
        (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus, LastStatusDate, PaidFees, CreatedByUserID)
        VALUES
        (@ApplicantPersonID, SYSDATETIME(), 1, 1, SYSDATETIME(), @PaidFees, @CreatedByUserID);

        SET @ApplicationID = SCOPE_IDENTITY();

        INSERT INTO LocalDrivingLicenseApplications
        (ApplicationID, LicenseClassID)
        VALUES
        (@ApplicationID, @LicenseClassID);

        SELECT SCOPE_IDENTITY();
        ";

                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    // تمرير القيم من DTO
                    command.Parameters.AddWithValue("@ApplicantPersonID", dTO.ApplicantPersonID);
                    command.Parameters.AddWithValue("@PaidFees", dTO.PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", dTO.CreatedByUserID);
                    command.Parameters.AddWithValue("@LicenseClassID", dTO.LicenseClassID);


                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int insertedID))
                    {
                        L_D_DrivingAdded = insertedID;
                    }
                }
            }

            return L_D_DrivingAdded;
        }
        public static int GetUserIDByUsername(string username)
        {
            int UserID = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString)) 

            using(SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                string Query = @"SELECT UserID FROM Users WHERE UserName = @username;
                                SELECT SCOPE_IDENTITY();";
                command.Parameters.AddWithValue("@username", username);
                command.CommandText = Query;

                try
                {
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if(result != null && int.TryParse(result.ToString(), out int Userid))
                    {
                        UserID = Userid;
                    }
                }
                catch 
                {
                    throw;
                }

                return UserID;
            }
        }
        public static float GetPaidFeesLocalApplication()
        {
            float PaidFees = -1;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                string Query = @"SELECT ApplicationFees FROM ApplicationTypes WHERE ApplicationTypeID = 1;
                                SELECT SCOPE_IDENTITY();";

                command.CommandText = Query;

                try
                {
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != null && float.TryParse(result.ToString(), out float PaidFeesValue))
                    {
                        PaidFees = PaidFeesValue;
                    }
                }
                catch
                {
                    throw;
                }

                return PaidFees;
            }
        }
        public static int HowManyPassTestForThisLDL_Data(int LocalDrivingLicenseApplicationID)
        {
            int TestResult = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT COUNT(TestAppointments.TestTypeID) FROM Tests
                    INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
                    INNER JOIN LocalDrivingLicenseApplications ON TestAppointments.LocalDrivingLicenseApplicationID
                    = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                    WHERE TestAppointments.LocalDrivingLicenseApplicationID = 
                    LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                    AND (Tests.TestResult = 1)
                    AND LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

            try
            {
                connection.Open();

                TestResult = (int)command.ExecuteScalar();

            }
            catch (Exception ex)
            {
                TestResult = -1;
            }
            finally
            {
                connection.Close();
            }

            return TestResult;
        }
        public static ApplicationBasicInfoDTO GetApplicationBasicInfo(int LocalDrivingLicenseApplicationID)
        {
            ApplicationBasicInfoDTO dTO = new ApplicationBasicInfoDTO();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 
                people.PersonID,
                LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,
				LicenseClasses.ClassName,
                Applications.ApplicationID,
                Applications.ApplicationStatus,
                Applications.PaidFees,
                ApplicationTypes.ApplicationTypeTitle,
                (SELECT People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) AS Applicant,
                Applications.ApplicationDate, 
                Applications.LastStatusDate,
				LicenseClasses.LicenseClassID,

(
SELECT COUNT(TestAppointments.TestTypeID) AS 'Passed Tests'
FROM Tests 
INNER JOIN TestAppointments ON Tests.TestAppointmentID = TestAppointments.TestAppointmentID
WHERE  
(TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID)
AND 
(Tests.TestResult = 1)
) 
AS 'Passed Tests'

FROM LocalDrivingLicenseApplications 
INNER JOIN LicenseClasses ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID
INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
INNER JOIN ApplicationTypes ON ApplicationTypes.ApplicationTypeID = Applications.ApplicationTypeID
INNER JOIN People ON People.PersonID = Applications.ApplicantPersonID
WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    dTO.PersonID = reader["PersonID"].ToString();
                    dTO.LocalDrivingLicenseApplicationID = reader["LocalDrivingLicenseApplicationID"].ToString();
                    dTO.ClassName = reader["ClassName"].ToString();
                    dTO.ApplicationID = reader["ApplicationID"].ToString();
                    dTO.Status = reader["ApplicationStatus"].ToString();
                    dTO.Fees = reader["PaidFees"].ToString();
                    dTO.ApplicationType = reader["ApplicationTypeTitle"].ToString();
                    dTO.Applicant = reader["Applicant"].ToString();
                    //ApplicationBasicInfo.TestTypeID                         = reader["TestTypeID"].ToString();
                    dTO.applicationDate = reader["ApplicationDate"] == DBNull.Value
                                                      ? null
                                                      : (DateTime?)Convert.ToDateTime(reader["ApplicationDate"]);
                    dTO.lastStatusDate = reader["LastStatusDate"] == DBNull.Value
                                                      ? null
                                                      : (DateTime?)Convert.ToDateTime(reader["LastStatusDate"]);

                    dTO.passedTests = (int)reader["Passed Tests"];
                    dTO.LicenseClassID = (int)reader["LicenseClassID"];
                    
                }
                reader.Close();
            }
            catch (Exception )
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return dTO;

        }
        public static int issueDrivingLicenseFirst(IssueDrivingLicenseFirstTimeDTO dTO)
        {
            int RowAffected = 0;

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"
            DECLARE @DriverID INT;
            INSERT INTO Drivers
            (PersonID,CreatedByUserID,CreatedDate)
            VALUES
            (@PersonID, 1,SYSDATETIME());
            SET @DriverID = SCOPE_IDENTITY();

            INSERT INTO Licenses
            (ApplicationID,DriverID,LicenseClass,IssueDate,ExpirationDate,Notes,PaidFees,IsActive,IssueReason,CreatedByUserID)
            VALUES
            (@ApplicationID, @DriverID, @LicenseClass,SYSDATETIME(),DATEADD(YEAR, 10, SYSDATETIME()), @Notes, 20, 1,1,1);

            UPDATE Applications 
            SET ApplicationStatus = 3
            WHERE ApplicationID = @ApplicationID;
            SELECT SCOPE_IDENTITY();
";


            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@PersonID", dTO.PersonID);
            Command.Parameters.AddWithValue("@ApplicationID", dTO.ApplicationID);
            Command.Parameters.AddWithValue("@LicenseClass", dTO.LicenseClass);
            Command.Parameters.AddWithValue("@Notes", dTO.Notes);

            try
            {
                Connection.Open();

                object result = Command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    RowAffected = insertedID;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return RowAffected;
        }
        public static bool HasLicenseWithThisClass(int personID, int licenseClassID)
        {
            bool hasLicense = false;
            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        SELECT 1
        WHERE EXISTS
        (
            SELECT *
            FROM LocalDrivingLicenseApplications
            INNER JOIN Applications
                ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
            WHERE ApplicantPersonID = @PersonID
              AND LicenseClassID = @LicenseClassID
              AND ApplicationStatus = 1
        )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);
                    command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if(result != null && int.TryParse(result.ToString(), out int Number))
                    {
                        return ((Number > 0) ? true : false);
                    }
                    return false;
                }
            }
        }
        public static int GetLicenseIDforApplicationID(int ApplicationID)
        {
            int LicenseID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT LicenseID FROM Licenses WHERE ApplicationID = @ApplicationID;";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

            try
            {
                connection.Open();


                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    LicenseID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                LicenseID = -1;
            }
            finally
            {
                connection.Close();
            }

            return LicenseID;
        }
        public static bool HasLicenseIssueWithThisClass(int personID, int licenseClassID)
        {
            bool hasLicense = false;
            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
        SELECT 1
        WHERE EXISTS
        (
            SELECT *
            FROM LocalDrivingLicenseApplications
            INNER JOIN Applications
                ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
            WHERE ApplicantPersonID = @PersonID
              AND LicenseClassID = @LicenseClassID
              AND ApplicationStatus = 3
        )";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PersonID", personID);
                    command.Parameters.AddWithValue("@LicenseClassID", licenseClassID);

                    connection.Open();

                    object result = command.ExecuteScalar();

                    if (result != null && int.TryParse(result.ToString(), out int Number))
                    {
                        return ((Number > 0) ? true : false);
                    }
                    return false;
                }
            }
        }


        public static int GetPersonIDforLocalApplicationID(int LocalApplicationID)
        {
            int PersonID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT 
                people.PersonID
                FROM LocalDrivingLicenseApplications 
                INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
                INNER JOIN People ON People.PersonID = Applications.ApplicantPersonID
                WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalApplicationID";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LocalApplicationID", LocalApplicationID);

            try
            {
                connection.Open();


                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    PersonID = Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {
                PersonID = -1;
            }
            finally
            {
                connection.Close();
            }

            return PersonID;
        }


        public static List<LocalLicenseHistoryDTO> GetAllLicenseHistroyRecords(int PersonID)
        {
            List<LocalLicenseHistoryDTO> applications = new List<LocalLicenseHistoryDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                       SELECT
                       Licenses.LicenseID,
                       Applications.ApplicationID,
                       ClassName,
                       ApplicationDate,
                       DATEADD(YEAR,LicenseClasses.DefaultValidityLength,ApplicationDate) AS ExpirationDate,
                       Licenses.IsActive

                   FROM Licenses
                   INNER JOIN Applications ON Licenses.ApplicationID = Applications.ApplicationID
                   INNER JOIN LicenseClasses ON Licenses.LicenseClass = LicenseClasses.LicenseClassID
                    WHERE Applications.ApplicantPersonID = @PersonID";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("PersonID", PersonID);
            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    applications.Add
                        (new LocalLicenseHistoryDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("LicenseID")),
                            reader.GetInt32(reader.GetOrdinal("ApplicationID")),
                            reader.GetString(reader.GetOrdinal("ClassName")),
                            reader.GetDateTime(reader.GetOrdinal("ApplicationDate")),
                            reader.GetDateTime(reader.GetOrdinal("ExpirationDate")),
                            reader.GetBoolean(reader.GetOrdinal("IsActive"))
                        )
                        );
                }

                reader.Close();
            }
            catch (Exception ) { throw ; }
            finally { connection.Close(); }

            return applications;
        }























        public static List<LocalLicenseDTO> GetAllLicenseRecordsData(
           string localDrivingLicenseApplicationID)
        {
            List<LocalLicenseDTO> dtoList = new List<LocalLicenseDTO>();

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
SELECT 
    Licenses.LicenseID,
    Licenses.ApplicationID,
    LicenseClasses.ClassName,
    Licenses.IssueDate,
    Licenses.ExpirationDate,
    Licenses.IsActive

FROM LocalDrivingLicenseApplications

INNER JOIN LicenseClasses
    ON LocalDrivingLicenseApplications.LicenseClassID =
       LicenseClasses.LicenseClassID

INNER JOIN Licenses
    ON LicenseClasses.LicenseClassID = Licenses.LicenseClass

WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID =
      @LocalDrivingLicenseApplicationID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue(
                        "@LocalDrivingLicenseApplicationID",
                        localDrivingLicenseApplicationID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                dtoList.Add(
                                    new LocalLicenseDTO(
                                        Convert.ToInt32(reader["LicenseID"]),
                                        Convert.ToInt32(reader["ApplicationID"]),
                                        reader["ClassName"]?.ToString(),

                                        reader["IssueDate"] != DBNull.Value
                                            ? (DateTime?)reader["IssueDate"]
                                            : null,

                                        reader["ExpirationDate"] != DBNull.Value
                                            ? (DateTime?)reader["ExpirationDate"]
                                            : null,

                                        Convert.ToBoolean(reader["IsActive"])
                                    )
                                );
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return dtoList;
        }
        public static LicenseInfoDTO GetDrivingLicenseInfo(int LicenseID)
        {
            LicenseInfoDTO dto = null;

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = $@"
SELECT 
    People.PersonID,
    LicenseClasses.ClassName,
    (People.FirstName + ' ' + People.SecondName + ' ' +
     People.ThirdName + ' ' + People.LastName) AS FullName,

    Licenses.LicenseID,
    LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,
    Licenses.ApplicationID,
    People.NationalNo,
    People.Gendor,
    Licenses.IssueDate,
    Licenses.IssueReason,
    Licenses.Notes,
    Licenses.IsActive,
    People.DateOfBirth,
    Drivers.DriverID,
    Licenses.ExpirationDate

FROM Licenses

INNER JOIN Applications
    ON Licenses.ApplicationID = Applications.ApplicationID

INNER JOIN People
    ON Applications.ApplicantPersonID = People.PersonID

INNER JOIN LicenseClasses
    ON Licenses.LicenseClass = LicenseClasses.LicenseClassID

INNER JOIN Drivers
    ON Licenses.DriverID = Drivers.DriverID

INNER JOIN LocalDrivingLicenseApplications
    ON LocalDrivingLicenseApplications.LicenseClassID =
       LicenseClasses.LicenseClassID

WHERE Licenses.LicenseID = @LicenseID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@LicenseID", LicenseID);

                    try
                    {
                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dto = new LicenseInfoDTO
                                {
                                    PersonID = (int)reader["PersonID"],

                                    ClassName = reader["ClassName"]?.ToString(),

                                    FullName = reader["FullName"]?.ToString(),

                                    LicenseID = (int)reader["LicenseID"],

                                    LocalDrivingLicenseApplicationID =
                                        (int)reader["LocalDrivingLicenseApplicationID"],

                                    ApplicationID = (int)reader["ApplicationID"],

                                    NationalNo = reader["NationalNo"]?.ToString(),

                                    Gendor = (byte)reader["Gendor"],

                                    IssueDate = reader["IssueDate"] != DBNull.Value
                                        ? (DateTime?)reader["IssueDate"]
                                        : null,

                                    IssueReason = (byte)reader["IssueReason"],

                                    Notes = reader["Notes"]?.ToString(),

                                    IsActive = (bool)reader["IsActive"],

                                    DateOfBirth = reader["DateOfBirth"] != DBNull.Value
                                        ? (DateTime?)reader["DateOfBirth"]
                                        : null,

                                    DriverID = (int)reader["DriverID"],

                                    ExpirationDate =
                                        reader["ExpirationDate"] != DBNull.Value
                                        ? (DateTime?)reader["ExpirationDate"]
                                        : null
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }

            return dto;
        }
        public static bool CancelLocalDrivingApplication_Data(int LocalDrivingLicenseApplicationID)
        {
            bool isCancel = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"UPDATE a
                                SET a.ApplicationStatus = 2
                                FROM Applications a
                                INNER JOIN LocalDrivingLicenseApplications l
                                    ON a.ApplicationID = l.ApplicationID
                                WHERE l.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID;";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);


            try
            {
                connection.Open();

                int rowAffected = command.ExecuteNonQuery();
                isCancel = (rowAffected > 0);

            }
            catch (Exception ex)
            {
                isCancel = false;
            }
            finally
            {
                connection.Close();
            }

            return isCancel;
        }
        public static bool DeleteLocalDrivingLicenseApplication_Data(int LocalDrivingLicenseApplicationID)
        {
            bool isDeleted = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"DELETE FROM LocalDrivingLicenseApplications WHERE 
                            LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID;";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);


            try
            {
                connection.Open();

                int RowAffected = command.ExecuteNonQuery();

                isDeleted = (RowAffected > 0);

            }
            catch (Exception ex)
            {
                isDeleted = false;
            }
            finally
            {
                connection.Close();
            }

            return isDeleted;
        }
        public static bool IsLicenseDetain(int LicenseID)
        {
            bool IsDetained = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"
                     SELECT IsDetained=1  From DetainedLicenses WHERE LicenseID = @LicenseID AND IsReleased = 0
                            ";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                connection.Open();

                object reulst = command.ExecuteScalar();
                if (reulst != null)
                {
                    IsDetained = Convert.ToBoolean(reulst);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return IsDetained;
        }
        public static bool DoesPersonIDHaveNewStautsOfThisClassType_Data(string ApplicantPersonID, string ApplicationTypeID)
        {
            bool have = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT Found = 1 FROM Applications
                            WHERE 
                            ApplicantPersonID = @ApplicantPersonID 
                            AND ApplicationTypeID = @ApplicationTypeID
                            AND ApplicationStatus = 1;";


            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);


            try
            {
                connection.Open();

                object result = command.ExecuteScalar(); // يرجع أول قيمة من أول صف
                have = (result != null); // إذا فيه نتيجة = true

            }
            catch (Exception ex)
            {
                have = false;
            }
            finally
            {
                connection.Close();
            }

            return have;
        }
        public static string GetApplicationID_Data(string PersonID)
        {
            string ApplicationID = "";
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT TOP 1 ApplicationID FROM Applications INNER JOIN People 
                                ON Applications.ApplicantPersonID = People.PersonID
                                WHERE PersonID = @PersonID AND ApplicationStatus = 1;
                         ";



            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null)
                    ApplicationID = result.ToString(); // يحوله سترنج


            }
            catch (Exception ex)
            {
                ApplicationID = "";
            }
            finally
            {
                connection.Close();
            }

            return ApplicationID;
        }
        public static int AddNewLDLApplication_Data(string ApplicationPersonID, string ApplicationTypeID)
        {
            int ApplicationID = -1;
            
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);


            // ApplicationTypeID = 1 -> New Local Driving License Service Category.

            string Query = @"INSERT INTO Applications
                            ( ApplicantPersonID,ApplicationDate,ApplicationTypeID,ApplicationStatus,
                              LastStatusDate,PaidFees,CreatedByUserID )
                            VALUES
                            (@ApplicationPersonID,SYSDATETIME(),@ApplicationTypeID,1,SYSDATETIME(),19,1);

                             SELECT SCOPE_IDENTITY();";


            SqlCommand command = new SqlCommand(Query, connection);

            command.Parameters.AddWithValue("@ApplicationPersonID", ApplicationPersonID);
            command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    ApplicationID = insertedID;
                }



            }
            catch (Exception ex)
            {
                ApplicationID = -1;
            }
            finally
            {
                connection.Close();
            }

            return ApplicationID;
        }
    }
}
