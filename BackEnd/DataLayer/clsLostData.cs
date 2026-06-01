using ContactsDataAccessLayer;
using DtoLayer.DamageApplication;
using DtoLayer.LostApplication;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLayer
{
    public class clsLostData
    {
        public static (int ApplicationID, int LicenseID) AddLostLicense(LostDTO dto)
        {
            int newApplicationID = 0;
            int newLicenseID = 0;

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
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
    @PaidFees,
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
                    command.Parameters.AddWithValue("@ApplicantPersonID", dto.lostApplication.ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", dto.lostApplication.ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.lostApplication.ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", dto.lostApplication.ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
                    command.Parameters.AddWithValue("@PaidFees", dto.lostApplication.PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", dto.lostApplication.CreatedByUserID);

                    // License
                    command.Parameters.AddWithValue("@DriverID", dto.lostLicense.DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", dto.lostLicense.LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", dto.lostLicense.IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", dto.lostLicense.ExpirationDate);
                    command.Parameters.AddWithValue("@Notes",
                        string.IsNullOrEmpty(dto.lostLicense.Notes)
                        ? (object)DBNull.Value
                        : dto.lostLicense.Notes);
                    command.Parameters.AddWithValue("@LicensePaidFees", dto.lostLicense.PaidFees);
                    command.Parameters.AddWithValue("@IsActive", dto.lostLicense.IsActive);
                    command.Parameters.AddWithValue("@IssueReason", dto.lostLicense.IssueReason);

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
