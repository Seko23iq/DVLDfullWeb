using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.InternationalAppliaction
{
    public class InternationalLicenseDTO
    {
        public int InternationalLicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseID { get; set; } // IssuedUsingLocalLicenseID
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        // Empty Constructor
        public InternationalLicenseDTO() { }

        // Full Constructor
        public InternationalLicenseDTO(
            int internationalLicenseID,
            int applicationID,
            int driverID,
            int licenseID,
            DateTime issueDate,
            DateTime expirationDate,
            bool isActive)
        {
            InternationalLicenseID = internationalLicenseID;
            ApplicationID = applicationID;
            DriverID = driverID;
            LicenseID = licenseID;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IsActive = isActive;
        }
    }
}
