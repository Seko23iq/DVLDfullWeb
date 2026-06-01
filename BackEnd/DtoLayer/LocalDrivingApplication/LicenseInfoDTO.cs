using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LocalDrivingApplication
{
    public class LicenseInfoDTO
    {
        public int PersonID { get; set; }
        public string ClassName { get; set; }
        public string FullName { get; set; }
        public int LicenseID { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public int ApplicationID { get; set; }
        public string NationalNo { get; set; }
        public byte Gendor { get; set; }
        public DateTime? IssueDate { get; set; }
        public int IssueReason { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int DriverID { get; set; }
        public DateTime? ExpirationDate { get; set; }

        // public string ImagePath { get; set; }
        public bool IsDetained { get; set; }

        public LicenseInfoDTO()
        {
        }

        public LicenseInfoDTO(
            int personID,
            string className,
            string fullName,
            int licenseID,
            int localDrivingLicenseApplicationID,
            int applicationID,
            string nationalNo,
            byte gendor,
            DateTime? issueDate,
            int issueReason,
            string notes,
            bool isActive,
            DateTime? dateOfBirth,
            int driverID,
            DateTime? expirationDate)
        {
            this.PersonID = personID;
            this.ClassName = className;
            this.FullName = fullName;
            this.LicenseID = licenseID;
            this.LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            this.ApplicationID = applicationID;
            this.NationalNo = nationalNo;
            this.Gendor = gendor;
            this.IssueDate = issueDate;
            this.IssueReason = issueReason;
            this.Notes = notes;
            this.IsActive = isActive;
            this.DateOfBirth = dateOfBirth;
            this.DriverID = driverID;
            this.IsDetained = IsDetained;
            this.ExpirationDate = expirationDate;
        }
    }
}
