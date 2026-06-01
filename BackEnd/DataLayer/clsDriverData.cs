using ContactsDataAccessLayer;
using DtoLayer.Driver;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class clsDriverData
    {
        static string MainQuery = @"SELECT * FROM Drivers_View";


        public static List<DriverDTO> GetAllDrivers()
        {
            List<DriverDTO> drivers = new List<DriverDTO>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            //string query = @"SELECT * FROM Drivers_View;";
            SqlCommand command = new SqlCommand(MainQuery, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    drivers.Add(new DriverDTO
                            (
                            reader.GetInt32(reader.GetOrdinal("DriverID")),
                            reader.GetInt32(reader.GetOrdinal("PersonID")),
                            reader.GetString(reader.GetOrdinal("NationalNo")),
                            reader.GetString(reader.GetOrdinal("FullName")),
                            reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
                            reader.GetInt32(reader.GetOrdinal("NumberOfActiveLicenses"))
                           ));
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

            return drivers;
        }

        public static DataTable SearchRecords(string FilterBy, string SearchText)
        {
            DataTable dt = new DataTable();
            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = MainQuery + " WHERE " + FilterBy + " LIKE @SearchText";
            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@SearchText", SearchText + "%");
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


        public static DataTable GetAllLicenseRecordsData(string LocalDrivingLicenseApplicationID)
        {

            DataTable dt = new DataTable();

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            
            string Query = @"SELECT LicenseID,Licenses.ApplicationID,ClassName, IssueDate,ExpirationDate,IsActive  FROM LocalDrivingLicenseApplications
INNER JOIN LicenseClasses ON LocalDrivingLicenseApplications.LicenseClassID = LicenseClasses.LicenseClassID
INNER JOIN Licenses ON LicenseClasses.LicenseClassID = Licenses.LicenseClass
WHERE LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID = @LocalDrivingLicenseApplicationID ";
            
            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@LocalDrivingLicenseApplicationID", LocalDrivingLicenseApplicationID);

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



    }
}
