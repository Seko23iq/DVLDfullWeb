using ContactsDataAccessLayer;
using DtoLayer;
using DtoLayer.Person;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace DataLayer
{
    public class clsLicenseClassData
    {
        public static LicenseClassInfoDTO GetLicenseClassInfo(string ClassName)
        {
            LicenseClassInfoDTO licenseClassInfo = null;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = "SELECT * FROM LicenseClasses WHERE ClassName = @ClassName";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ClassName", ClassName);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    licenseClassInfo = new LicenseClassInfoDTO
                        (
                    reader.GetInt32(reader.GetOrdinal("LicenseClassID")),
                    reader.GetString(reader.GetOrdinal("ClassName")),
                    reader.GetString(reader.GetOrdinal("ClassDescription")),
                    (int)reader.GetByte(reader.GetOrdinal("MinimumAllowedAge")),
                    (int)reader.GetByte(reader.GetOrdinal("DefaultValidityLength")),
                    reader.GetDecimal(reader.GetOrdinal("ClassFees"))
                );
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

            return licenseClassInfo;
        }
    }
}
