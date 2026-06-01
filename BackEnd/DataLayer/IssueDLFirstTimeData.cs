using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Data.SqlClient;
using ContactsDataAccessLayer;

namespace DataLayer
{
    public class IssueDLFirstTimeData
    {
        public static int AddNewDLData(string PersonID,string ApplicationID,string LicenseClass,string Notes)
        {
            int RowAffected = 0;

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"
            DECLARE @DriverID INT;
            INSERT INTO Drivers
            (PersonID,CreatedByUserID,CreatedDate)
            VALUES
            (@PersonID, 1,SYSDATETIME());
            SET @DriverID = SCOPE_IDENTITY();

            INSERT INTO Licenses
            (ApplicationID,DriverID,LicenseClass,IssueDate,ExpirationDate,Notes,PaidFees,IsActive,IssueReason,CreatedByUserID)
            VALUES
            (@ApplicationID, @DriverID, @LicenseClass,SYSDATETIME(),DATEADD(YEAR, 10, SYSDATETIME()), @Notes, 20, 1,0,1);

            UPDATE Applications 
            SET ApplicationStatus = 3
            WHERE ApplicationID = @ApplicationID;
            SELECT SCOPE_IDENTITY();
";


            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            Command.Parameters.AddWithValue("@LicenseClass", LicenseClass);
            Command.Parameters.AddWithValue("@Notes", Notes);

            try
            {
                Connection.Open();

                object result = Command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    RowAffected = insertedID;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return RowAffected;
        }


        public static bool HasDLactiveData(string ApplicationID, string LicenseClass)
        {

            bool isActive = false;

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"SELECT FOUND = 1 FROM Licenses
                    WHERE ApplicationID = @ApplicationID AND LicenseClass = @LicenseClass AND IsActive = 1;
                    ";

            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            Command.Parameters.AddWithValue("@LicenseClass", LicenseClass);

            try
            {
                Connection.Open();

                object result = Command.ExecuteScalar(); // يرجع أول قيمة من أول صف
                isActive = (result != null); // إذا فيه نتيجة = true

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return isActive;
        }


        public static bool UpdateLicenseActiveData(string LicenseID)
        {
            bool isActiveFlase = false;

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = @"UPDATE Licenses
                               SET  IsActive = 0
                             WHERE LicenseID = @LicenseID";

            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                Connection.Open();

                object result = Command.ExecuteScalar(); // يرجع أول قيمة من أول صف
                isActiveFlase = (result != null); // إذا فيه نتيجة = true

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return isActiveFlase;
        }


    }
}
