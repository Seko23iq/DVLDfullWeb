using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Net;
using ContactsDataAccessLayer;
using DtoLayer.Person;
using System.ComponentModel.DataAnnotations;

namespace DataLayer
{
    public class clsPersonData
    {
        static public string MainTableView =
            @"SELECT 
                        People.PersonID,
                  People.NationalNo,
                        People.FirstName,
                        People.SecondName,
                        People.ThirdName,
                        People.LastName,
                        CASE People.Gendor
                             WHEN 0 THEN 'Male'
                             WHEN 1 THEN 'Female'
                        END AS Gendor,
                        People.Address,
                        People.DateOfBirth,
                        Countries.CountryName AS Nationality,
                        People.Phone,
                        People.Email ,
                        People.ImagePath
            FROM People 
            INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID";
        public static int AddNewPersonData(AddPersonDTO personDTO)
        {
            int PersonID = -1;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"INSERT INTO People 
                                (
		                                NationalNo,
		                                FirstName,
		                                SecondName, 
		                                ThirdName, 
		                                LastName, 
		                                DateOfBirth, 
		                                Gendor, 
		                                Address,
		                                Phone, 
		                                Email, 
		                                NationalityCountryID,
		                                ImagePath
                                )
                                VALUES (@NationalNo,@FirstName,@SecondName,@ThirdName, @LastName, @DateOfBirth, 
                                        @Gendor,@Address, @Phone,@Email,@NationalityCountryID,@ImagePath);
                                SELECT SCOPE_IDENTITY();";

            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@NationalNo", personDTO.NationalNo);
            sqlCommand.Parameters.AddWithValue("@FirstName", personDTO.FirstName);
            sqlCommand.Parameters.AddWithValue("@SecondName", personDTO.SecondName);
            sqlCommand.Parameters.AddWithValue("@ThirdName", personDTO.ThirdName);
            sqlCommand.Parameters.AddWithValue("@LastName", personDTO.LastName);
            sqlCommand.Parameters.AddWithValue("@DateOfBirth", personDTO.DateOfBirth);

            if (personDTO.Gender == "Male")
            {
                sqlCommand.Parameters.AddWithValue("@Gendor", 0);
            }
            else if (personDTO.Gender == "Female")
            {
                sqlCommand.Parameters.AddWithValue("@Gendor", 1);
            }

            sqlCommand.Parameters.AddWithValue("@Address", personDTO.Address);
            sqlCommand.Parameters.AddWithValue("@Phone", personDTO.Phone);
            sqlCommand.Parameters.AddWithValue("@Email", personDTO.Email);
            sqlCommand.Parameters.AddWithValue("@NationalityCountryID", personDTO.Nationality);
            sqlCommand.Parameters.AddWithValue("@ImagePath", personDTO.ImagePath);

            try
            {
                sqlConnection.Open();
                object result = sqlCommand.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    PersonID = insertedID;
                }
            }
            catch (Exception ex)
            {
                throw; // أو سجل الخطأ (Logging)
            }
            finally { sqlConnection.Close(); }
            return PersonID;
        }
        public static bool UpdatePerson(UpdatePersonDTO personDTO)
        {

            int rowsAffected = 0;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"Update  People  
                            set nationalNo = @nationalNo,
                                firstName = @firstName, 
                                secondName = @secondName, 
                                thirdName = @thirdName, 
                                lastName = @lastName, 
                                dateOfBirth = @dateOfBirth,
                                gendor = @gendor,
                                address = @address, 
                                email = @email, 
                                phone = @phone, 
                                NationalityCountryID = @Country,
                                imagePath = @imagePath
                                where personID = @personID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@personID", personDTO.PersonID);
            command.Parameters.AddWithValue("@nationalNo", personDTO.NationalNo);
            command.Parameters.AddWithValue("@firstName", personDTO.FirstName);
            command.Parameters.AddWithValue("@secondName", personDTO.SecondName);
            command.Parameters.AddWithValue("@thirdName", personDTO.ThirdName);
            command.Parameters.AddWithValue("@lastName", personDTO.LastName);
            command.Parameters.AddWithValue("@dateOfBirth", personDTO.DateOfBirth);
            command.Parameters.AddWithValue("@address", personDTO.Address);
            command.Parameters.AddWithValue("@email", personDTO.Email);
            command.Parameters.AddWithValue("@Country", personDTO.Nationality);
            command.Parameters.AddWithValue("@phone", personDTO.Phone);


            if (personDTO.Gender == "Male")
            {
                command.Parameters.AddWithValue("@Gendor", 0);
            }
            else if (personDTO.Gender == "Female")
            {
                command.Parameters.AddWithValue("@Gendor", 1);
            }



            if (!string.IsNullOrEmpty(personDTO.ImagePath))
                command.Parameters.AddWithValue("@imagePath", personDTO.ImagePath);
            else
                command.Parameters.AddWithValue("@imagePath", DBNull.Value);

            try
            {
                connection.Open();
                rowsAffected = command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                //return false;
                throw; // 👈 مهم جدًا

            }

            finally
            {
                connection.Close();
            }

            return (rowsAffected > 0);
        }
        public static List<PeopleDTO> GetAllPeopleDTO()
     {
         var lsPeople = new List<PeopleDTO>();
     
             using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
             {
                 string query = MainTableView;
                 using (SqlCommand command = new SqlCommand(query, connection))
                 {
                     try
                     {
                         connection.Open();
                         using (SqlDataReader reader = command.ExecuteReader())
                         {
                             while (reader.Read())
                             {
                                 lsPeople.Add(new PeopleDTO
                                 (
                                 reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("NationalNo")),
                                 reader.GetString(reader.GetOrdinal("FirstName")),
                                 reader.GetString(reader.GetOrdinal("SecondName")),
                                 reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? "" : reader.GetString(reader.GetOrdinal("ThirdName")),
                                 reader.GetString(reader.GetOrdinal("LastName")),
                                 reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                 reader.GetString(reader.GetOrdinal("Gendor")),
                                 reader.GetString(reader.GetOrdinal("Address")),
                                 reader.GetString(reader.GetOrdinal("Phone")),
                                 reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetString(reader.GetOrdinal("Nationality")),
                                 reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString(reader.GetOrdinal("ImagePath"))
                                ));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        return lsPeople;

     }
        public static List<PeopleDTO> GetInfoByData(string DataType, string Datavalue)
        {
            var lsPeople = new List<PeopleDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "";
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                switch (DataType)
                {
                    case "PersonID":
                        query = MainTableView + " WHERE CAST(People.PersonID AS NVARCHAR) LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = Datavalue + "%"; 
                        break;
                    case "NationalNo":
                        query = MainTableView + " WHERE People.NationalNo = @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = Datavalue;
                        break;
                    case "Name":
                        query = MainTableView + " WHERE People.FirstName LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = Datavalue + "%";
                        break;

                    case "Email":
                        query = MainTableView + " WHERE People.Email LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = Datavalue + "%";
                        break;
                    case "Nationality":
                        query = MainTableView + " WHERE Countries.CountryName LIKE @value";
                        command.Parameters.Add("@value", SqlDbType.NVarChar).Value = Datavalue + "%";
                        break;

                    case "Gendor":
                        query = MainTableView + " WHERE People.Gendor = @value";
                        command.Parameters.Add("@value", SqlDbType.Int).Value = Datavalue == "Male" ? 0 : 1;
                        break;

                    default:
                        throw new Exception("Invalid filter type");
                }

                command.CommandText = query;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lsPeople.Add(new PeopleDTO(
                                   reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("NationalNo")),
                                 reader.GetString(reader.GetOrdinal("FirstName")),
                                 reader.GetString(reader.GetOrdinal("SecondName")),
                                 reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? "" : reader.GetString(reader.GetOrdinal("ThirdName")),
                                 reader.GetString(reader.GetOrdinal("LastName")),
                                 reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                 // يفضل التأكد من نوع الـ Gendor (هل هو byte أم string في القاعدة؟)
                                 reader.GetString(reader.GetOrdinal("Gendor")),
                                 reader.GetString(reader.GetOrdinal("Address")),
                                 reader.GetString(reader.GetOrdinal("Phone")),
                                 reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetString(reader.GetOrdinal("Nationality")),
                                 reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString(reader.GetOrdinal("ImagePath"))
                            ));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return lsPeople;
        }
        public static bool DeletePersonData(int PersonID)
     {
         int rowsAffected = 0;
     
         SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
         string query = "DELETE FROM People WHERE PersonID = @PersonID;";
         SqlCommand command = new SqlCommand(query, connection);
         command.Parameters.AddWithValue("@PersonID", PersonID);
     
         try
         {
             connection.Open();
             rowsAffected = command.ExecuteNonQuery();
         }
         catch (Exception ex) {return false; }
     
         finally { connection.Close(); }
     
         return (rowsAffected > 0);
     }
        public static PeopleDTO GetPersonInfo(int PersonID)
        {
            PeopleDTO Person = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = MainTableView + " WHERE People.PersonID = @personID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@personID", SqlDbType.Int).Value = PersonID;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Person = new PeopleDTO(
                                 reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("NationalNo")),
                                 reader.GetString(reader.GetOrdinal("FirstName")),
                                 reader.GetString(reader.GetOrdinal("SecondName")),
                                 reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? "" : reader.GetString(reader.GetOrdinal("ThirdName")),
                                 reader.GetString(reader.GetOrdinal("LastName")),
                                 reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                 // يفضل التأكد من نوع الـ Gendor (هل هو byte أم string في القاعدة؟)
                                 reader.GetString(reader.GetOrdinal("Gendor")),
                                 reader.GetString(reader.GetOrdinal("Address")),
                                 reader.GetString(reader.GetOrdinal("Phone")),
                                 reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetString(reader.GetOrdinal("Nationality")),
                                 reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString(reader.GetOrdinal("ImagePath"))
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

            return Person;
        }
        public static PeopleDTO GetPersonInfoByNationalNo(string NationalNo)
        {
            PeopleDTO Person = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = MainTableView + " WHERE People.NationalNo = @NationalNo";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@NationalNo", NationalNo);

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Person = new PeopleDTO(
                                 reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("NationalNo")),
                                 reader.GetString(reader.GetOrdinal("FirstName")),
                                 reader.GetString(reader.GetOrdinal("SecondName")),
                                 reader.IsDBNull(reader.GetOrdinal("ThirdName")) ? "" : reader.GetString(reader.GetOrdinal("ThirdName")),
                                 reader.GetString(reader.GetOrdinal("LastName")),
                                 reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                                 // يفضل التأكد من نوع الـ Gendor (هل هو byte أم string في القاعدة؟)
                                 reader.GetString(reader.GetOrdinal("Gendor")),
                                 reader.GetString(reader.GetOrdinal("Address")),
                                 reader.GetString(reader.GetOrdinal("Phone")),
                                 reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
                                 reader.GetString(reader.GetOrdinal("Nationality")),
                                 reader.IsDBNull(reader.GetOrdinal("ImagePath")) ? "" : reader.GetString(reader.GetOrdinal("ImagePath"))
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

            return Person;
        }
        public static bool IsPersonExsitsByData(string DataType,string DataValue)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = $"SELECT Found=1 FROM People WHERE {DataType} LIKE @DataValue + '%'";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@DataValue", DataValue);

            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                connection.Close();
            }

            return isFound;
        }
    }
}
