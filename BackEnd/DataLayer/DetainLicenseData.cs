using ContactsDataAccessLayer;
using DtoLayer.Detain;
using DtoLayer.Person;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class DetainLicenseData
    {

        public static int AddDetainLicenseData(DetainLicenseDTO dTO)
        {
            int DetainID = -1;
            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"INSERT INTO DetainedLicenses
                                (
	                                LicenseID
	                                ,DetainDate
	                                ,FineFees
	                                ,CreatedByUserID
	                                ,IsReleased
	                                ,ReleaseDate
	                                ,ReleasedByUserID
	                                ,ReleaseApplicationID
                                )
                                VALUES
                                (@LicenseID,@DetainDate,@FineFees,@CreatedByUserID,0,NULL,NULL,NULL)
                            SELECT SCOPE_IDENTITY();";


            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@LicenseID", dTO.LicenseID);
            sqlCommand.Parameters.AddWithValue("@DetainDate", dTO.DetainDate);
            sqlCommand.Parameters.AddWithValue("@FineFees", dTO.FineFees);
            sqlCommand.Parameters.AddWithValue("@CreatedByUserID", dTO.CreatedByUserID);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && int.TryParse(Resulte.ToString(), out int InsertedID))
                {
                    DetainID = InsertedID;
                }

            }
            catch (Exception ex)
            {
            }
            finally
            {
                sqlConnection.Close();
            }

            return DetainID;
        }

        public static DetainLicenseInfoDTO GetDetainInfoData(int LicenseID)
        {
            DetainLicenseInfoDTO dTO = null;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                            SELECT TOP 1 DetainID, DetainDate, FineFees,UserName FROM DetainedLicenses
                            INNER JOIN Users
                            ON DetainedLicenses.CreatedByUserID = Users.UserID
                            WHERE LicenseID = @LicenseID
                            ORDER BY DetainID DESC;";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                sqlConnection.Open();

                SqlDataReader Reader = sqlCommand.ExecuteReader();

                while(Reader.Read())
                {
                    dTO = new DetainLicenseInfoDTO(
                        Reader.GetInt32(Reader.GetOrdinal("DetainID")),
                        Reader.GetDateTime(Reader.GetOrdinal("DetainDate")),
                        Reader.GetDecimal(Reader.GetOrdinal("FineFees")),
                        Reader.GetString(Reader.GetOrdinal("UserName"))
                        );
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                sqlConnection.Close();
            }

            return dTO;
        }

        public static int ReleaseLicense(ReleaseLicenseDTO dTO)
        {
            int newApplicationID = -1;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"
INSERT INTO Applications
(
    ApplicantPersonID,
    ApplicationDate,
    ApplicationTypeID,
    ApplicationStatus,
    LastStatusDate,
    PaidFees,
    CreatedByUserID
)
VALUES
(
    @ApplicantPersonID,
    @ApplicationDate,
    @ApplicationTypeID,
    @ApplicationStatus,
    @LastStatusDate,
    @ApplicationPaidFees,
    @CreatedByUserID
);

UPDATE DetainedLicenses
SET IsReleased = 1,
    ReleaseDate = SYSDATETIME(),
    ReleasedByUserID = @ReleasedByUserID,
    ReleaseApplicationID = SCOPE_IDENTITY() 
WHERE DetainID = @DetainID;

SELECT SCOPE_IDENTITY();";
    
    SqlCommand command = new SqlCommand(query, connection);
            // Application
            command.Parameters.AddWithValue("@ApplicantPersonID", dTO.ReleaseDetainedApplication.ApplicantPersonID);
            command.Parameters.AddWithValue("@ApplicationDate", dTO.ReleaseDetainedApplication.ApplicationDate);
            command.Parameters.AddWithValue("@ApplicationTypeID", dTO.ReleaseDetainedApplication.ApplicationTypeID);
            command.Parameters.AddWithValue("@ApplicationStatus", dTO.ReleaseDetainedApplication.ApplicationStatus);
            command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
            command.Parameters.AddWithValue("@ApplicationPaidFees", dTO.ReleaseDetainedApplication.PaidFees);
            command.Parameters.AddWithValue("@CreatedByUserID", dTO.ReleaseDetainedApplication.CreatedByUserID);
            command.Parameters.AddWithValue("@ReleasedByUserID", dTO.ReleasedByUserID);
            command.Parameters.AddWithValue("@LicenseID", dTO.LicenseID);
            command.Parameters.AddWithValue("@DetainID", dTO.DetainID);

            try
            {
                connection.Open();
                object result = command.ExecuteScalar(); 
                if (result != null && int.TryParse(result.ToString(), out int id))
                    newApplicationID = id;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return newApplicationID; 
        }

























        static string MainQuery = @"SELECT
                                DetainedLicenses.DetainID,
                                DetainedLicenses.LicenseID,
                                DetainedLicenses.DetainDate,
                                DetainedLicenses.IsReleased,
                                DetainedLicenses.FineFees,
                                DetainedLicenses.ReleaseDate,
                                People.NationalNo,
                                (People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) AS FullName,
                                DetainedLicenses.ReleaseApplicationID
                                FROM DetainedLicenses 
                                INNER JOIN Licenses ON DetainedLicenses.LicenseID = Licenses.LicenseID
                                INNER JOIN Applications ON Licenses.ApplicationID = Applications.ApplicationID
                                INNER JOIN People ON People.PersonID = Applications.ApplicantPersonID";

        public static List<ListDetainedLicenseDTO> GetAllRecords()
        {
            List<ListDetainedLicenseDTO> list = new List<ListDetainedLicenseDTO>();

            using (SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string Query = MainQuery;

                using (SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection))
                {
                    try
                    {
                        sqlConnection.Open();

                        using (SqlDataReader reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ListDetainedLicenseDTO dto = new ListDetainedLicenseDTO
                                {
                                    DetainID = reader.GetInt32(reader.GetOrdinal("DetainID")),
                                    LicenseID = reader.GetInt32(reader.GetOrdinal("LicenseID")),
                                    DetainDate = reader.GetDateTime(reader.GetOrdinal("DetainDate")),
                                    IsReleased = reader.GetBoolean(reader.GetOrdinal("IsReleased")),
                                    FineFees = reader.GetDecimal(reader.GetOrdinal("FineFees")),
                                    ReleaseDate = reader.IsDBNull(reader.GetOrdinal("ReleaseDate"))
                                        ? (DateTime?)null
                                        : reader.GetDateTime(reader.GetOrdinal("ReleaseDate")),
                                    NationalNo = reader["NationalNo"].ToString(),
                                    FullName = reader["FullName"].ToString(),
                                    ReleaseApplicationID = reader.IsDBNull(reader.GetOrdinal("ReleaseApplicationID"))
                                        ? (int?)null
                                        : reader.GetInt32(reader.GetOrdinal("ReleaseApplicationID"))
                                };

                                list.Add(dto);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // الأفضل تسجيل الخطأ بدل إخفائه
                        throw new Exception("Error while fetching detained licenses", ex);
                    }
                }
            }

            return list;
        }

        public static DataTable GetInfoBy_DataLayer(string Type,string Value)
        {
            DataTable dt = new DataTable();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + $" WHERE {Type} LIKE @Value ";

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


    }
}
