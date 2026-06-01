using DataLayer;
using DtoLayer.ReNewDrivingApplication;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactsDataAccessLayer.RenewLicense
{
    public class RenewLicenseData
    {

        public static bool IsLicenseExpire(int LicenseID)
        {
            bool IsLicenseExpire = false;

            SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString);
            string query = @"SELECT
	                            CASE 
		                            WHEN SYSDATETIME() > ExpirationDate THEN 1
		                            ELSE 0
	                            END AS IsLicenseExpired
                            FROM Licenses
                            WHERE LicenseID = 1; ";

            SqlCommand command = new SqlCommand(query,connection);


            try
            {
                connection.Open();
                IsLicenseExpire = Convert.ToBoolean(command.ExecuteScalar());
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

            return IsLicenseExpire;
        }


        public static (int, int) AddRenewLicense(AddRenewLicenseDTO dto)
        {
            int newApplicationID = 0;
            int newLicenseID = 0;

            using (SqlConnection connection =
                   new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    string query = @"
UPDATE Licenses
SET IsActive = 0
WHERE LicenseID = @OldLicenseID;

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

DECLARE @ApplicationID INT = SCOPE_IDENTITY();

INSERT INTO Licenses
(
    ApplicationID,
    DriverID,
    LicenseClass,
    IssueDate,
    ExpirationDate,
    Notes,
    PaidFees,
    IsActive,
    IssueReason,
    CreatedByUserID
)
VALUES
(
    @ApplicationID,
    @DriverID,
    @LicenseClass,
    @IssueDate,
    @ExpirationDate,
    @Notes,
    @LicensePaidFees,
    @IsActive,
    @IssueReason,
    @CreatedByUserID
);

SELECT @ApplicationID, SCOPE_IDENTITY();

";
                    SqlCommand command = new SqlCommand(query, connection, transaction);

                    // Old License
                    command.Parameters.AddWithValue("@OldLicenseID", dto.OldLicenseID);

                    // Application
                    command.Parameters.AddWithValue("@ApplicantPersonID", dto.Application.ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", dto.Application.ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.Application.ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", dto.Application.ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ApplicationPaidFees", dto.Application.PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", dto.Application.CreatedByUserID);

                    // License
                    command.Parameters.AddWithValue("@DriverID", dto.License.DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", dto.License.LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", dto.License.IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", dto.License.ExpirationDate);
                    command.Parameters.AddWithValue("@Notes",
                        string.IsNullOrEmpty(dto.License.Notes)
                        ? (object)DBNull.Value
                        : dto.License.Notes);
                    command.Parameters.AddWithValue("@LicensePaidFees", dto.License.PaidFees);
                    command.Parameters.AddWithValue("@IsActive", dto.License.IsActive);
                    command.Parameters.AddWithValue("@IssueReason", dto.License.IssueReason);
                    //command.Parameters.AddWithValue("@CreatedByUserID", dto.License.CreatedByUserID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newApplicationID = Convert.ToInt32(reader[0]);
                            newLicenseID = Convert.ToInt32(reader[1]);
                        }
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return (newApplicationID, newLicenseID);
        }

    }
}
