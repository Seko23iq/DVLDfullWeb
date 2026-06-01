using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DtoLayer.ApplicationTypes;

namespace DataLayer
{
    public class clsApplicationTypesData
    {
        public static List<ApplicationTypesDTO> GetAllRecords()
        {

            List<ApplicationTypesDTO> MATDTOs = new List<ApplicationTypesDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string Query = "SELECT * FROM ApplicationTypes";

                using (SqlCommand command = new SqlCommand(Query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                MATDTOs.Add(new ApplicationTypesDTO
                                (
                                    reader.GetInt32(reader.GetOrdinal("ApplicationTypeID")),
                                    reader.GetString(reader.GetOrdinal("ApplicationTypeTitle")),
                                    reader.GetDecimal(reader.GetOrdinal("ApplicationFees"))
                               ));
                        }
                    }
                    catch (Exception ex)
                    {
                    Console.WriteLine(ex.Message);
                    throw;
                    // يفضل تسجيل الخطأ هنا (Logging) لتعرف سبب المشكلة لاحقاً
                }
                }
            }

            return MATDTOs;
        }
        public static ApplicationTypesDTO GetApplicationTypeInfo(int ApplicationTypeID)
        {
            ApplicationTypesDTO ApplicationType = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM ApplicationTypes WHERE ApplicationTypeID = @ApplicationTypeID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@ApplicationTypeID", SqlDbType.Int).Value = ApplicationTypeID;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApplicationType = new ApplicationTypesDTO(
                                 reader.GetInt32(reader.GetOrdinal("ApplicationTypeID")),
                                 reader.GetString(reader.GetOrdinal("ApplicationTypeTitle")),
                                 reader.GetDecimal(reader.GetOrdinal("ApplicationFees"))
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

            return ApplicationType;
        }
        public static bool UpdateApplicationType(ApplicationTypesDTO dto)
        {
            int rowsAffected = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE ApplicationTypes 
                         SET ApplicationTypeTitle = @ApplicationTypeTitle,
                             ApplicationFees = @ApplicationFees
                         WHERE ApplicationTypeID = @ApplicationTypeID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.ID);
                    command.Parameters.AddWithValue("@ApplicationTypeTitle", dto.Title);
                    command.Parameters.AddWithValue("@ApplicationFees", dto.Fees);

                    connection.Open();
                    rowsAffected = command.ExecuteNonQuery();
                }
            }

            return rowsAffected > 0;
        }
    }
}
