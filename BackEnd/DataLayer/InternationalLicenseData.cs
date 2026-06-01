using ContactsDataAccessLayer;
using DtoLayer.InternationalAppliaction;
using DtoLayer.LicenseHistory;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DataLayer
{
    public class InternationalLicenseData
    {
        static public NewInternationalLicenseResultDTO AddNewInternationalLicenseData(AddInternationalApplicationDTO dTO)
        {
            NewInternationalLicenseResultDTO resultDTO = new NewInternationalLicenseResultDTO();

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
        DECLARE @ApplicationID INT;
        DECLARE @InternationalLicenseID INT;

        INSERT INTO Applications
        (ApplicantPersonID, ApplicationDate, ApplicationTypeID, ApplicationStatus,
         LastStatusDate, PaidFees, CreatedByUserID)
        VALUES
        (@ApplicantPersonID, SYSDATETIME(), 6, 3,
         SYSDATETIME(), @PaidFees, @CreatedByUserID);

        SET @ApplicationID = SCOPE_IDENTITY();

        INSERT INTO InternationalLicenses
        (ApplicationID, DriverID, IssuedUsingLocalLicenseID,
         IssueDate, ExpirationDate, IsActive, CreatedByUserID)
        VALUES
        (@ApplicationID, @DriverID, @IssuedUsingLocalLicenseID,
         @IssueDate, @ExpirationDate, 1, @CreatedByUserID);

        SET @InternationalLicenseID = SCOPE_IDENTITY();

        SELECT 
            @ApplicationID AS ApplicationID,
            @InternationalLicenseID AS InternationalLicenseID;
    ";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@ApplicantPersonID", dTO.ApplicantPersonID);
            sqlCommand.Parameters.AddWithValue("@DriverID", dTO.DriverID);
            sqlCommand.Parameters.AddWithValue("@IssuedUsingLocalLicenseID", dTO.IssuedUsingLocalLicenseID);
            sqlCommand.Parameters.AddWithValue("@IssueDate", dTO.IssueDate);
            sqlCommand.Parameters.AddWithValue("@ExpirationDate", dTO.ExpirationDate);
            sqlCommand.Parameters.AddWithValue("@CreatedByUserID", dTO.CreatedByUserID);
            sqlCommand.Parameters.AddWithValue("@PaidFees", dTO.PaidFees);

            try
            {
                sqlConnection.Open();

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.Read())
                {
                    resultDTO.ApplicationID = Convert.ToInt32(reader["ApplicationID"]);
                    resultDTO.InternationalLicenseID = Convert.ToInt32(reader["InternationalLicenseID"]);
                }

                reader.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return resultDTO;
        }

        // This Method must go to license class not here.
        public static int GetPersonIDByLicenseID(int LicenseID)
        {
            int PersonID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"
                        SELECT Applications.ApplicantPersonID FROM Licenses
                        INNER JOIN Applications ON Licenses.LicenseID = Applications.ApplicationID
                        WHERE Licenses.LicenseID = @LicenseID";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LicenseID", LicenseID);


            try
            {
                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int personID))
                {
                    PersonID = personID;
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return PersonID;
        }
        public static DataTable GetAllInternationalRecordsData(string InternationalLicenseID)
        {

            DataTable dt = new DataTable();

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"SELECT 
                        InternationalLicenseID,
                        ApplicationID,
                        IssuedUsingLocalLicenseID AS LicenseID, 
                        IssueDate,
                        ExpirationDate,
                        IsActive  
                        FROM InternationalLicenses
                        WHERE InternationalLicenseID = @InternationalLicenseID";

            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@InternationalLicenseID", InternationalLicenseID);

            try
            {
                Connection.Open();

                SqlDataReader reader = Command.ExecuteReader();
                if (reader.HasRows) { dt.Load(reader); }
                reader.Close();
            }
            catch (Exception ex) { }
            finally { Connection.Close(); }

            return dt;

        }
        public static List<InternationalLicenseDTO> GetAllAllInternationalLicenseRecordsData()
        {

            List<InternationalLicenseDTO> dTOs = new List<InternationalLicenseDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT 
                        InternationalLicenseID,
                        ApplicationID,
                        DriverID,
                        IssuedUsingLocalLicenseID AS LicenseID, 
                        IssueDate,
                        ExpirationDate,
                        IsActive  
                        FROM InternationalLicenses";

            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    dTOs.Add(new InternationalLicenseDTO
                    {
                        InternationalLicenseID = Convert.ToInt32(reader["InternationalLicenseID"]),
                        ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                        DriverID = Convert.ToInt32(reader["DriverID"]),
                        LicenseID = Convert.ToInt32(reader["LicenseID"]),
                        IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                        ExpirationDate = Convert.ToDateTime(reader["ExpirationDate"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dTOs;

        }
        public static DataTable GetInfoBy_DataLayer(string Type, int Value)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = $@"SELECT
                        InternationalLicenseID,
                        ApplicationID,
                        DriverID,
                        IssuedUsingLocalLicenseID AS LicenseID, 
                        IssueDate,
                        ExpirationDate,
                        IsActive
                        FROM InternationalLicenses 
                        WHERE {Type} LIKE @Value ";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Value", Value + "%");
            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)

                {
                    dt.Load(reader);
                }

                reader.Close();


            }

            catch (Exception ex)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }

            return dt;
        }
        public static bool HasInternationalLicenseData(int DriverID)
        {
            bool hasLicense = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT FOUND=1 FROM InternationalLicenses
                            WHERE DriverID = @DriverID;";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@DriverID", DriverID);
            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                hasLicense = reader.HasRows;

                reader.Close();
            }

            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            return hasLicense;
        }
        public static InternationalApplicationInfoDto GetInternationApplicationInfo(int InternationApplicationID)
        {
            InternationalApplicationInfoDto dto = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"
            SELECT 
                (SELECT People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) AS FullName,
                InternationalLicenses.InternationalLicenseID,
                Applications.ApplicationID,
                Licenses.LicenseID,
                InternationalLicenses.IsActive,
                People.NationalNo,
                People.DateOfBirth,
                People.Gendor,
                InternationalLicenses.DriverID,
                InternationalLicenses.IssueDate,
                InternationalLicenses.ExpirationDate,
	            People.ImagePath
            FROM InternationalLicenses
            INNER JOIN Applications 
                ON InternationalLicenses.ApplicationID = Applications.ApplicationID
            INNER JOIN People 
                ON Applications.ApplicantPersonID = People.PersonID
            INNER JOIN Licenses 
                ON Licenses.LicenseID = InternationalLicenses.IssuedUsingLocalLicenseID
            WHERE InternationalLicenses.InternationalLicenseID = @ID;";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", InternationApplicationID);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        dto = new InternationalApplicationInfoDto
                        {
                            FullName = reader["FullName"].ToString(),
                            InternationalLicenseID = Convert.ToInt32(reader["InternationalLicenseID"]),
                            ApplicationID = Convert.ToInt32(reader["ApplicationID"]),
                            LicenseID = Convert.ToInt32(reader["LicenseID"]),
                            IsActive = Convert.ToBoolean(reader["IsActive"]),
                            NationalNo = reader["NationalNo"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                            Gendor = Convert.ToByte(reader["Gendor"]),
                            DriverID = Convert.ToInt32(reader["DriverID"]),
                            IssueDate = Convert.ToDateTime(reader["IssueDate"]),
                            ExpirationDate = Convert.ToDateTime(reader["ExpirationDate"]),
                            ImagePath = reader["ImagePath"] == DBNull.Value ? null : reader["ImagePath"].ToString()
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return dto;
        }
        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            int InternationLicenseID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            // In this query , we first set IsActive to 0 for all existing licenses of the driver, then we insert the new license and return its ID.
            string Query = @"
                            SELECT TOP 1 InternationalLicenseID 
                            FROM InternationalLicenses 
                            WHERE DriverID = @DriverID AND GETDATE() BETWEEN IssueDate AND ExpirationDate
                            ORDER BY ExpirationDate DESC
                            ";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@DriverID", DriverID);


            try
            {
                connection.Open();

                object result = command.ExecuteScalar();
                if (result != null && int.TryParse(result.ToString(), out int InsertedID))
                {
                    InternationLicenseID = InsertedID;
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return InternationLicenseID;
        }
        public static List<InternationalLicenseHistoryDTO> GetAllInternationalLicenseHistroyRecordsByPersonID(int PersonID)
        {
            List<InternationalLicenseHistoryDTO> applications = new List<InternationalLicenseHistoryDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                     SELECT
                     InternationalLicenses.InternationalLicenseID,
                     Applications.ApplicationID,
                     LicenseClasses.ClassName,
                     Applications.ApplicationDate,
	                 
                     DATEADD(YEAR,LicenseClasses.DefaultValidityLength,Applications.ApplicationDate) AS     ExpirationDate,
                     CASE 
                         WHEN DATEADD(YEAR,LicenseClasses.DefaultValidityLength,Applications.   ApplicationDate) >     SYSDATETIME () 
                             THEN 1
                             ELSE  0
                     END AS IsActive
	 

                    FROM InternationalLicenses
                    INNER JOIN Applications on InternationalLicenses.ApplicationID = Applications.ApplicationID
                    INNER JOIN Licenses ON InternationalLicenses.IssuedUsingLocalLicenseID = Licenses.LicenseID
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
                        (new InternationalLicenseHistoryDTO
                        (
                            reader.GetInt32(reader.GetOrdinal("InternationalLicenseID")),
                            reader.GetInt32(reader.GetOrdinal("ApplicationID")),
                            reader.GetString(reader.GetOrdinal("ClassName")),
                            reader.GetDateTime(reader.GetOrdinal("ApplicationDate")),
                            reader.GetDateTime(reader.GetOrdinal("ExpirationDate")),
                            reader.GetInt32(reader.GetOrdinal("IsActive")) == 1 ? true : false
                        )
                        );
                }

                reader.Close();
            }
            catch (Exception) { throw; }
            finally { connection.Close(); }

            return applications;
        }
    }
}
