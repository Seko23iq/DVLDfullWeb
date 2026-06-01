using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class clsCountryData
    {
        public static bool GetCountryInfoByID(int CountryID, ref string CountryName)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT CountryName FROM Countries WHERE CountryID = @CountryID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CountryID", CountryID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if(reader.HasRows)
                {
                    isFound = true;
                    CountryName = (string)reader["CountryName"];
                }
                else
                {
                    isFound = false;
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


            return isFound;
        }
        public static bool GetCountryInfoByName(string CountryName, ref int CountryID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT CountryID FROM Countries WHERE CountryName = @CountryName";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@CountryName", CountryName);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    isFound = true;
                    CountryID = (int)reader["CountryID"];
                }
                else
                {
                    isFound = false;
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

            return isFound;
        }

        public static List<string> GetAllCountry()
        {
            List<string> Countries = new List<string>();
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT CountryName FROM Countries;";
            SqlCommand command = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();
                while(reader.Read())
                {
                    Countries.Add((string)reader["CountryName"]);
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

            return Countries;
        }

    }
}
