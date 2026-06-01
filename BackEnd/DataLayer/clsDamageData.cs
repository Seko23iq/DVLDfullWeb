using ContactsDataAccessLayer;
using DtoLayer.DamageApplication;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DataLayer
{
    public class clsDamageData
    {
        public static (int ApplicationID, int LicenseID) AddDamageLicense(DamageDTO dto)
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
                    command.Parameters.AddWithValue("@ApplicantPersonID", dto.DamageApplication.ApplicantPersonID);
                    command.Parameters.AddWithValue("@ApplicationDate", dto.DamageApplication.ApplicationDate);
                    command.Parameters.AddWithValue("@ApplicationTypeID", dto.DamageApplication.ApplicationTypeID);
                    command.Parameters.AddWithValue("@ApplicationStatus", dto.DamageApplication.ApplicationStatus);
                    command.Parameters.AddWithValue("@LastStatusDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ApplicationPaidFees", dto.DamageApplication.PaidFees);
                    command.Parameters.AddWithValue("@CreatedByUserID", dto.DamageApplication.CreatedByUserID);

                    // License
                    command.Parameters.AddWithValue("@DriverID", dto.damagedLicense.DriverID);
                    command.Parameters.AddWithValue("@LicenseClass", dto.damagedLicense.LicenseClass);
                    command.Parameters.AddWithValue("@IssueDate", dto.damagedLicense.IssueDate);
                    command.Parameters.AddWithValue("@ExpirationDate", dto.damagedLicense.ExpirationDate);
                    command.Parameters.AddWithValue("@Notes",
                        string.IsNullOrEmpty(dto.damagedLicense.Notes)
                        ? (object)DBNull.Value
                        : dto.damagedLicense.Notes);
                    command.Parameters.AddWithValue("@LicensePaidFees", dto.damagedLicense.PaidFees);
                    command.Parameters.AddWithValue("@IsActive", dto.damagedLicense.IsActive);
                    command.Parameters.AddWithValue("@IssueReason", dto.damagedLicense.IssueReason);

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
