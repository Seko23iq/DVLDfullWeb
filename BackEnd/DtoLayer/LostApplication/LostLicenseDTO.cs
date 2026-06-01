using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LostApplication
{
    public class LostLicenseDTO
    {
        public int DriverID { get; set; }
        public int LicenseClass { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsActive { get; set; }
        public int IssueReason { get; set; }

        public LostLicenseDTO()
        {
        }

        public LostLicenseDTO(
            int driverID,
            int licenseClass,
            DateTime issueDate,
            DateTime expirationDate,
            string notes,
            decimal paidFees,
            bool isActive,
            int issueReason
            )
        {
            this.DriverID = driverID;
            this.LicenseClass = licenseClass;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.Notes = notes;
            this.PaidFees = paidFees;
            this.IsActive = isActive;
            this.IssueReason = issueReason;
        }
    }
}
