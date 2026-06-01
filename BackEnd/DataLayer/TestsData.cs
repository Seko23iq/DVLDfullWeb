using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DtoLayer.Application;
using DtoLayer.Test;

namespace DataLayer
{

 
    public class TestsData
    {
        private static string MainQuery = @"SELECT 
                    TestAppointments.TestAppointmentID AS 'testAppointmentID',
                    TestAppointments.AppointmentDate AS 'AppointmentDate',
                    TestAppointments.PaidFees AS 'PaidFees',
                    TestAppointments.IsLocked AS 'IsLocked'
                    FROM TestAppointments
                    INNER JOIN LocalDrivingLicenseApplications 
                    ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                    INNER JOIN Applications ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID

                    WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
                    AND TestAppointments.TestTypeID = @TestTypeID";

        public static List<testAppointmentsDTO> GetAllTestAppointmentRecordsData(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            List<testAppointmentsDTO> dTOs = new List<testAppointmentsDTO>();


            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            SqlCommand command = new SqlCommand(MainQuery, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dTOs.Add
                        (new testAppointmentsDTO
                        (
                        reader.GetInt32(reader.GetOrdinal("testAppointmentID")),
                        reader.GetDateTime(reader.GetOrdinal("AppointmentDate")),
                        reader.GetDecimal(reader.GetOrdinal("PaidFees")),
                        reader.GetBoolean(reader.GetOrdinal("IsLocked"))
                        )
                        );
                }

                reader.Close();
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }


            return dTOs;
        }

        public static ApplicantInformationTestDTO GetApplicantInformation(int LocalDrivingLicenseApplicationID)
        {
            ApplicantInformationTestDTO dTO = new ApplicantInformationTestDTO();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString)) 

            using(SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                string query = @"
SELECT LocalDrivingLicenseApplicationID,
LicenseClasses.ClassName,
(People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) as 'FullName',
Applications.PaidFees
FROM LocalDrivingLicenseApplications
INNER JOIN Applications 
ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
INNER JOIN People
ON Applications.ApplicantPersonID = People.PersonID
INNER JOIN LicenseClasses 
ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID
WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID
AND Applications.ApplicationTypeID = 1";

                //string query = @"SELECT LocalDrivingLicenseApplicationID,FullName,ClassName,PaidFees 
                //    FROM TestAppointments_View 
                //    WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID";

                command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

                command.CommandText = query;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())

                if(reader.Read())
                {
                    dTO.LocalDrivingLicenseApplicationID = (int) reader["LocalDrivingLicenseApplicationID"];
                    dTO.ApplicantClass = reader["ClassName"].ToString();
                    dTO.ApplicantName = reader["FullName"].ToString();
                    dTO.Fees = (decimal)reader["PaidFees"];
                }
            }
            return dTO;
        }

        public static int AddNewTestAppointmentData(AddApplicationDTO dTO)
        {
            int Result = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);



            string Query = @"DECLARE @Fees DECIMAL(10,2);
                        SELECT @Fees = TestTypeFees
                        FROM TestTypes
                        WHERE TestTypeID = @TestTypeID;

                        INSERT INTO TestAppointments
                        (TestTypeID, LocalDrivingLicenseApplicationID, AppointmentDate, PaidFees, CreatedByUserID, IsLocked)
                        VALUES 
                        (@TestTypeID, @LocalDrivingLicenseApplicationID, @AppointmentDate, @Fees, @CreatedByUserID, 0);

                        SELECT SCOPE_IDENTITY();";

            SqlCommand command = new SqlCommand(Query, connection);

            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", dTO.LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@AppointmentDate", dTO.AppointmentDate);
            command.Parameters.AddWithValue("@TestTypeID", dTO.TestTypeID);
            command.Parameters.AddWithValue("@CreatedByUserID", dTO.CreatedByUserID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    Result = insertedID;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
            return Result;
        }




        public static TakerTestInfoDTO GetTakerTestInfoInformation(int TestAppointmentID)
        {
            TakerTestInfoDTO dTO = new TakerTestInfoDTO();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))

            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                string query = @"
SELECT 
LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,
LicenseClasses.ClassName,
(People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) as 'FullName',
Applications.ApplicationDate,
Applications.PaidFees
FROM TestAppointments
INNER JOIN LocalDrivingLicenseApplications 
ON TestAppointments.LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
INNER JOIN Applications
ON LocalDrivingLicenseApplications.ApplicationID = Applications.ApplicationID
INNER JOIN People
ON Applications.ApplicantPersonID = People.PersonID
INNER JOIN LicenseClasses 
ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID
WHERE TestAppointments.TestAppointmentID = @TestAppointmentID
AND Applications.ApplicationTypeID = 1";

                command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);

                command.CommandText = query;

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())

                    if (reader.Read())
                    {
                        dTO.DrivingLicenseApplicationID = (int)reader["LocalDrivingLicenseApplicationID"];
                        dTO.LicenseClass = reader["ClassName"].ToString();
                        dTO.Name = reader["FullName"].ToString();
                        dTO.Date =(DateTime) reader["ApplicationDate"];
                        dTO.Fees = (decimal)reader["PaidFees"];
                    }
            }
            return dTO;
        }

        public static int TakeTestData(TakeTestDTO dTO)
        {
            int rowAffected = -1;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"INSERT INTO Tests 
                                            (TestAppointmentID,TestResult,Notes,CreatedByUserID)
                                            VALUES
                                            (@TestAppointmentID,@TestResult,@Notes,@CreatedByUserID);
                                            UPDATE TestAppointments
                                            SET IsLocked = CASE 
                                                             WHEN @TestResult = 1 THEN 1
                                                             WHEN @TestResult = 0 THEN 1 

                                                             ELSE IsLocked 
                                                           END
                                        SELECT SCOPE_IDENTITY();

                ";
            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@TestAppointmentID", dTO.TestAppointmentID);
            command.Parameters.AddWithValue("@TestResult", dTO.TestResult);
            command.Parameters.AddWithValue("@Notes", dTO.Notes);
            command.Parameters.AddWithValue("@CreatedByUserID", dTO.CreatedByUserID);

            try
            {
                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    rowAffected = insertedID;
                }

            }
            catch (Exception ex)
            {
                rowAffected = -1;
            }
            finally
            {
                connection.Close();
            }
            return rowAffected;
        }

        public static bool EditTestAppointmentData(UpdateTestAppointmentDto dto)
        {
            bool isEdit = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"UPDATE TestAppointments 
                                SET AppointmentDate = @AppointmentUpdateDate
                                WHERE TestAppointmentID = @TestAppointmentID;";
            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@AppointmentUpdateDate", dto.AppointmentUpdateDate);
            command.Parameters.AddWithValue("@TestAppointmentID", dto.TestAppointmentID);

            try
            {
                connection.Open();

                int RowAfftected = command.ExecuteNonQuery();
                isEdit = (RowAfftected > 0);

            }
            catch (Exception ex)
            {
                isEdit = false;
            }
            finally
            {
                connection.Close();
            }
            return isEdit;
        }



















        public static bool HasTestAppointmentData(LocalAndTestTypeidDTO dTO)
        {
            bool HasRecord = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"SELECT FOUND = 1 FROM TestAppointments
                                WHERE LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID 
                                AND TestTypeID = @TestTypeID AND IsLocked = 0";

            SqlCommand command = new SqlCommand(Query, connection);
            command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", dTO.LocalDrivingLicenseApplicationID);
            command.Parameters.AddWithValue("@TestTypeID", dTO.TestTypeID);


            try
            {
                connection.Open();
                SqlDataReader reder = command.ExecuteReader();
                HasRecord = reder.HasRows;
                reder.Close();

            }
            catch (Exception ex)
            {
                HasRecord = false;
            }
            finally
            {
                connection.Close();
            }
            return HasRecord;
        }
       
   
     
    }
}
