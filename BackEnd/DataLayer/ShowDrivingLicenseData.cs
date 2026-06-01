using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ContactsDataAccessLayer;
using Microsoft.Data.SqlClient;
namespace DataLayer
{
    public class ShowDrivingLicenseData
    {
        public struct stDrivingLicense
        {
            public string PersonID;
            public string ClassName;
            public string FullName;
            public string LicenseID;
            public string LocalDrivingLicenseApplicationID;
            public string ApplicationID;
            public string NationalNo;
            public string Gendor;
            public DateTime? IssueDate;
            public string IssueReason;
            public string Notes;
            public string IsActive;
            public DateTime? DateOfBirth;
            public string DriverID;
            public DateTime? ExpirationDate;
            public string ImagePath;
        }



        public static stDrivingLicense GetDrivingLicenseInfo(string Type,string Value)
        {
            stDrivingLicense drivingLicense = new stDrivingLicense();

            SqlConnection Connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string Query = $@"SELECT 
                                    People.PersonID,
                                    LicenseClasses.ClassName,
                                    ( People.FirstName + ' ' + People.SecondName + ' ' + People.ThirdName + ' ' + People.LastName) AS FullName,
                                    Licenses.LicenseID,
                                    LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID,
                                    Licenses.ApplicationID,
                                    People.NationalNo,
                                    People.Gendor,
                                    Licenses.IssueDate,
                                    Licenses.IssueReason,
                                    Licenses.Notes,
                                    Licenses.IsActive,
                                    People.DateOfBirth,
                                    Drivers.DriverID,
                                    Licenses.ExpirationDate,
                                    People.ImagePath
                                    
                                    FROM Licenses
                                    INNER JOIN Applications   ON Licenses.ApplicationID			 = Applications.ApplicationID
                                    INNER JOIN People		  ON Applications.ApplicantPersonID  = People.PersonID
                                    INNER JOIN LicenseClasses ON Licenses.LicenseClass			 = LicenseClasses.LicenseClassID
                                    INNER JOIN Drivers	      ON Licenses.DriverID				 = Drivers.DriverID

INNER JOIN LocalDrivingLicenseApplications ON LocalDrivingLicenseApplications.LicenseClassID = 
LicenseClasses.LicenseClassID
WHERE {Type} = @Value 
";


            SqlCommand Command = new SqlCommand(Query, Connection);
            Command.Parameters.AddWithValue("@Value", Value);

            try
            {
                Connection.Open();

                SqlDataReader reader = Command.ExecuteReader();

                if (reader.Read())
                {
                    drivingLicense = new stDrivingLicense
                    {
                        PersonID = reader["PersonID"]?.ToString(),
                        ClassName = reader["ClassName"]?.ToString(),
                        FullName = reader["FullName"]?.ToString(),
                        LicenseID = reader["LicenseID"]?.ToString(),
                        LocalDrivingLicenseApplicationID = reader["LocalDrivingLicenseApplicationID"]?.ToString(),
                        ApplicationID = reader["ApplicationID"]?.ToString(),
                        NationalNo = reader["NationalNo"]?.ToString(),
                        Gendor = reader["Gendor"]?.ToString(),

                        IssueDate = reader["IssueDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["IssueDate"]) : null,
                        IssueReason = reader["IssueReason"]?.ToString(),
                        Notes = reader["Notes"]?.ToString(),
                        IsActive = reader["IsActive"]?.ToString(),
                        DateOfBirth = reader["DateOfBirth"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["DateOfBirth"]) : null,
                        DriverID = reader["DriverID"]?.ToString(),
                        ExpirationDate = reader["ExpirationDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ExpirationDate"]) : null,
                        ImagePath = reader["ImagePath"]?.ToString()
                    };
                }

                reader.Close();

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return drivingLicense;
        }







    }
}
