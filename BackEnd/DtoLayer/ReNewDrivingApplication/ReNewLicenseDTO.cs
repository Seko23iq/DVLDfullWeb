using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.ReNewDrivingApplication
{
    public class ReNewLicenseDTO
    {
      
        public int DriverID { get; set; }
        public int LicenseClass { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public int IssueReason { get; set; }
        public int CreatedByUserID { get; set; }

        public ReNewLicenseDTO()
        {
        }

        public ReNewLicenseDTO(
            int driverID,
            int licenseClass,
            DateTime issueDate,
            DateTime expirationDate,
            string notes,
            decimal paidFees,
            bool isActive,
            int issueReason,
            int createdByUserID)
        {
            this.DriverID = driverID;
            this.LicenseClass = licenseClass;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.Notes = notes;
            this.PaidFees = paidFees;
            this.IsActive = isActive;
            this.IssueReason = issueReason;
            this.CreatedByUserID = createdByUserID;
        }
    }
}
