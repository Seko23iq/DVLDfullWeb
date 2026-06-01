using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LocalDrivingApplication
{
    public class ApplicationBasicInfoDTO
    {
        public string PersonID { get; set; }
        public string LocalDrivingLicenseApplicationID { get; set; }
        public string ClassName { get; set; }
        public string ApplicationID { get; set; }
        public string Status { get; set; }
        public string Fees { get; set; }
        public string ApplicationType { get; set; }
        public string Applicant { get; set; }
        public DateTime? applicationDate { get; set; }
        public DateTime? lastStatusDate { get; set; }
        public string CreatedBy { get; set; }
        public int passedTests { get; set; }
        public int LicenseClassID { get; set; }

        public ApplicationBasicInfoDTO()
        {
        }
        public ApplicationBasicInfoDTO(string personID, string localDrivingLicenseApplicationID, string drivingClass, string applicationID, string status, string fees, string applicationType, string applicant, DateTime? applicationDate, DateTime? lastStatusDate, string createdByUserName, int passedTests, int LicenseClassID)
        {
            this.PersonID = personID;
            this.LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            this.ClassName = drivingClass;
            this.ApplicationID = applicationID;
            this.Status = status;
            this.Fees = fees;
            this.ApplicationType = applicationType;
            this.Applicant = applicant;
            this.applicationDate = applicationDate;
            this.lastStatusDate = lastStatusDate;
            this.CreatedBy = createdByUserName;
            this.passedTests = passedTests;
            this.LicenseClassID = LicenseClassID;
        }
    }
}
