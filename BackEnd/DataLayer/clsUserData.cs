using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using DtoLayer.User;


namespace DataLayer
{
    public class clsUserData
    {
        static string MainQuery = @"SELECT UserID,USERS.PersonID,
                            (People.FirstName + ' ' + People.LastName + ' ' + People.ThirdName + ' ' + LastName)AS FullName,
                            UserName, Password ,IsActive 
                            FROM People INNER JOIN Users ON Users.PersonID = People.PersonID";
        public static bool LoginInData(string UserName, string Password)
        {

            bool isFound = false;
            //AND IsActive = 1; // If You want to not allow inActive User to login .
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = @"SELECT FOUND = 1 FROM Users WHERE @username = username AND @Password = password";


            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@UserName", UserName);
            command.Parameters.AddWithValue("@Password", Password);

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
        public static List<UserDTO> GetAllUsersDTO()
        {
            var lsUsers = new List<UserDTO>();

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = MainQuery;
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lsUsers.Add(new UserDTO
                                (
                                reader.GetInt32(reader.GetOrdinal("UserID")),
                                reader.GetInt32(reader.GetOrdinal("PersonID")),
                                reader.IsDBNull(reader.GetOrdinal("FullName")) ? "" : 
                                reader.GetString(reader.GetOrdinal("FullName")),
                                reader.GetString(reader.GetOrdinal("UserName")),
                                reader.GetBoolean(reader.GetOrdinal("IsActive"))
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
            return lsUsers;
        }
        public static UserDTO GetInfoByUserID(int userID)
        {
            UserDTO user = null;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = MainQuery + " WHERE Users.UserID LIKE @UserID";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.Add("@userID", SqlDbType.Int).Value = userID;

                try
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user = new UserDTO(
                                 reader.GetInt32(reader.GetOrdinal("UserID")),
                                 reader.GetInt32(reader.GetOrdinal("PersonID")),
                                 reader.GetString(reader.GetOrdinal("FullName")),
                                 reader.GetString(reader.GetOrdinal("UserName")),
                                 reader.GetBoolean(reader.GetOrdinal("IsActive"))
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

            return user;
        }
        public static List<UserDTO> GetInfoByUserID_DataLayer(int UserID)
        {
            List<UserDTO> users = new List<UserDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + " WHERE Users.UserID = @UserID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserDTO(
                        Convert.ToInt32(reader["UserID"]),
                        Convert.ToInt32(reader["PersonID"]),
                        reader["FullName"].ToString(),
                        reader["UserName"].ToString(),
                        Convert.ToBoolean(reader["IsActive"])
                    ));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // log error
            }
            finally
            {
                connection.Close();
            }

            return users;
        }
        public static List<UserDTO> GetInfoByPersonID_DataLayer(int PersonID)
        {
            List<UserDTO> users = new List<UserDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + " WHERE Users.PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserDTO(
                        Convert.ToInt32(reader["UserID"]),
                        Convert.ToInt32(reader["PersonID"]),
                        reader["FullName"].ToString(),
                        reader["UserName"].ToString(),
                        Convert.ToBoolean(reader["IsActive"])
                    ));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                // log error
            }
            finally
            {
                connection.Close();
            }

            return users;
        }
        public static List<UserDTO> GetInfoByFullName_DataLayer(string FullName)
        {
            List<UserDTO> users = new List<UserDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + @" WHERE 
        (People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) 
        LIKE @FullName";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FullName", FullName + "%");

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserDTO(
                        Convert.ToInt32(reader["UserID"]),
                        Convert.ToInt32(reader["PersonID"]),
                        reader["FullName"].ToString(),
                        reader["UserName"].ToString(),
                        Convert.ToBoolean(reader["IsActive"])
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                // log error
            }
            finally
            {
                connection.Close();
            }

            return users;
        }
        public static List<UserDTO> GetInfoByUserName_DataLayer(string UserName)
        {
            List<UserDTO> users = new List<UserDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + @" WHERE Users.UserName LIKE @UserName";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserName", UserName + "%");

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserDTO(
                        Convert.ToInt32(reader["UserID"]),
                        Convert.ToInt32(reader["PersonID"]),
                        reader["FullName"].ToString(),
                        reader["UserName"].ToString(),
                        Convert.ToBoolean(reader["IsActive"])
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                // log error
            }
            finally
            {
                connection.Close();
            }

            return users;
        }
        public static List<UserDTO> GetInfoByUserIsActive_DataLayer(int Status)
        {
            List<UserDTO> users = new List<UserDTO>();

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = MainQuery + @" WHERE Users.IsActive = @Status";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Status", Status);

            try
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new UserDTO(
                        Convert.ToInt32(reader["UserID"]),
                        Convert.ToInt32(reader["PersonID"]),
                        reader["FullName"].ToString(),
                        reader["UserName"].ToString(),
                        Convert.ToBoolean(reader["IsActive"])
                    ));
                }

                reader.Close();
            }
            catch (Exception)
            {
                // log error
            }
            finally
            {
                connection.Close();
            }

            return users;
        }
        public static bool DeleteUserData(int UserID)
        {
            bool isUserDelete = false;
            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"DELETE FROM Users WHERE UserID = @UserID;";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                sqlConnection.Open();

                int rowsAffected = sqlCommand.ExecuteNonQuery();

                isUserDelete = (rowsAffected > 0);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return isUserDelete;


        }
        public static bool IsUserExists(int UserID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 1 FROM Users WHERE UserID = @UserID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@UserID", UserID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

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
            return isFound;
        }
        public static bool IsUserExistsByPersonID(int PersonID)
        {
            bool isFound = false;
            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT 1 FROM Users WHERE PersonID = @PersonID";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@PersonID", PersonID);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

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
            return isFound;
        }
        public static int AddNewUser(UserAddDTO userDTO)
        {
            int UserID = -1;
            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"INSERT INTO Users 
                            ( PersonID,UserName,Password,IsActive)
                            VALUES (@PersonID,@Username,@Password,@isActive);
                            SELECT SCOPE_IDENTITY();";


            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@PersonID", userDTO.PersonID);
            sqlCommand.Parameters.AddWithValue("@Username", userDTO.UserName);
            sqlCommand.Parameters.AddWithValue("@Password", userDTO.Password);
            sqlCommand.Parameters.AddWithValue("@isActive", userDTO.IsActive);


            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && int.TryParse(Resulte.ToString(), out int InsertedID))
                {
                    UserID = InsertedID;
                }

            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return UserID;
        }
        public static bool UpdateUserData(UserUpdateDTO userUpdateDTO)
        {
            bool IsUpdated = false;
            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"UPDATE Users
                                SET UserName = @Username,
	                                Password = @Password,
                                    IsActive = @IsActive
                                WHERE UserID = @UserID;";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@Username", userUpdateDTO.UserName);
            sqlCommand.Parameters.AddWithValue("@Password", userUpdateDTO.Password);
            sqlCommand.Parameters.AddWithValue("@UserID", userUpdateDTO.UserID);
            sqlCommand.Parameters.AddWithValue("@IsActive", userUpdateDTO.IsActive);

            try
            {
                sqlConnection.Open();

                int RowAffected = sqlCommand.ExecuteNonQuery();
                IsUpdated = (RowAffected > 0);

            }
            catch (Exception ex)
            {
                IsUpdated = false;
            }
            finally
            {
                sqlConnection.Close();
            }

            return IsUpdated;
        }
        public static string GetImagePathByUsername(string Username)
        {
            string ImagePath = "";

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                        SELECT ImagePath 
                        FROM Users 
                        INNER JOIN People 
                        ON Users.PersonID = People.PersonID 
                        WHERE Users.UserName = @Username
                        ";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", Username);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && Resulte.ToString() != "")
                {
                    ImagePath = Resulte.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return ImagePath;
        }
        public static int GetUserIDByUsername(string Username)
        {
            int UserID = 0;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                        SELECT UserID 
                        FROM Users 
                        WHERE Users.UserName = @Username
                        ";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", Username);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && int.TryParse(Resulte.ToString(), out int ID))
                {
                    UserID = ID;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return UserID;
        }
        public static int GetActiveByUsername(string Username)
        {
            int IsActive = 0;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = @"
                        SELECT IsActive 
                        FROM Users 
                        WHERE Users.UserName = @Username
                        ";

            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", Username);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && Resulte != DBNull.Value)
                {
                    IsActive = Convert.ToBoolean(Resulte) ? 1 : 0;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                sqlConnection.Close();
            }

            return IsActive;
        }




















        public static bool UpdatePassword(string UpdatePassword, string UserID)
        {
            bool isUpdate = false;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "UPDATE Users SET Password = @Password WHERE UserID = @UserID;";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Password", UpdatePassword);
            sqlCommand.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                sqlConnection.Open();

                int rowAffected= sqlCommand.ExecuteNonQuery();
                isUpdate = (rowAffected > 0);
            }
            catch(Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return isUpdate;
        }
        public static string CurrentPassword( string UserID)
        {
            string CurrentPassword = "";

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT Password FROM Users WHERE UserID = @UserID;";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@UserID", UserID);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if(Resulte != null && Resulte.ToString() != "")
                {
                    CurrentPassword = Resulte.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return CurrentPassword;
        }
          public static string GetPersonIDByUsername(string Username)
        {
            string PersonID = "";

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT PersonID FROM Users WHERE Username = @Username;";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", Username);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && Resulte.ToString() != "")
                {
                    PersonID = Resulte.ToString();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return PersonID;
        }
        public static bool GetIsActiveByUsername(string Username)
        {
            bool IsActive = false;

            SqlConnection sqlConnection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string Query = "SELECT IsActive FROM Users WHERE Username = @Username;";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@Username", Username);

            try
            {
                sqlConnection.Open();

                object Resulte = sqlCommand.ExecuteScalar();

                if (Resulte != null && Resulte.ToString() != "")
                {
                    IsActive = Convert.ToBoolean(Resulte);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                sqlConnection.Close();
            }

            return IsActive;
        }
        public static bool IsPersonUserByPersonIDData(string PersonID)
        {
            bool isFound = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);

            string query = "SELECT Found=1 FROM Users WHERE PersonID = @PersonID";

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

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
