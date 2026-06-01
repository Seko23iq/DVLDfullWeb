using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography.Pkcs;
using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
using DtoLayer.TestTypes;

namespace DataLayer
{
    public class clsTestTypesData
    {
        public static List<TestTypesDTO> GetAllRecords()
        {
            List<TestTypesDTO> dTOs = new List<TestTypesDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = "SELECT * FROM TestTypes";
            SqlCommand command = new SqlCommand(Query, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while(reader.Read())
                {
                    dTOs.Add(new TestTypesDTO(
                   reader.GetInt32(reader.GetOrdinal("TestTypeID")),
                   reader.GetString(reader.GetOrdinal("TestTypeTitle")),
                   reader.GetString(reader.GetOrdinal("TestTypeDescription")),
                   reader.GetDecimal(reader.GetOrdinal("TestTypeFees"))
                   ));
                }
               

                reader.Close();

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return dTOs;
        }
        public static TestTypesDTO GetTypeInfoByID(int ID)
        {
            TestTypesDTO ApplicationTestType = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM TestTypes WHERE TestTypeID = @TestTypeID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@TestTypeID", SqlDbType.Int).Value = ID;

                try
                {
                    connection.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApplicationTestType = new TestTypesDTO(
                                 reader.GetInt32(reader.GetOrdinal("TestTypeID")),
                                 reader.GetString(reader.GetOrdinal("TestTypeTitle")),
                                 reader.GetString(reader.GetOrdinal("TestTypeDescription")),
                                 reader.GetDecimal(reader.GetOrdinal("TestTypeFees"))
                             );
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return ApplicationTestType;
        }
        public static bool UpdateApplicationType(TestTypesDTO dTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string Query = @"UPDATE TestTypes 
	                            SET TestTypeTitle = @TestTypeTitle,
	                                TestTypeDescription = @TestTypeDescription,
	                                TestTypeFees = @TestTypeFees
	                                WHERE TestTypeID = @TestTypeID;

                            SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(Query, connection);

                command.Parameters.AddWithValue("@TestTypeID", dTO.ID);
                command.Parameters.AddWithValue("@TestTypeTitle", dTO.Title);
                command.Parameters.AddWithValue("@TestTypeDescription", dTO.Description);
                command.Parameters.AddWithValue("@TestTypeFees", dTO.Fees);

                try
                {
                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {

                }
            }
            return (rowsAffected > 0);
        }

    }
}
