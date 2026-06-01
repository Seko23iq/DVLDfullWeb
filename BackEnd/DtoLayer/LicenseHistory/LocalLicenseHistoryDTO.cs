using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LicenseHistory
{
    public class LocalLicenseHistoryDTO
    {
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public string LicenseClassName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public LocalLicenseHistoryDTO() { }

        public LocalLicenseHistoryDTO(int LicenseID, int ApplicationID, string licenseClassName, DateTime issueDate, DateTime expirationDate, bool isActive)
        {
            this.LicenseID = LicenseID;
            this.ApplicationID = ApplicationID;
            this.LicenseClassName = licenseClassName;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.IsActive = isActive;
        }
    }
}
