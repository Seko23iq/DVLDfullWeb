using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LicenseHistory
{
    public class InternationalLicenseHistoryDTO
    {
        public int InternationalLicenseID { get; set; }
        public int ApplicationID { get; set; }
        public string LicenseClassName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public InternationalLicenseHistoryDTO() { }

        public InternationalLicenseHistoryDTO(int InternationalLicenseID, int ApplicationID, string licenseClassName, DateTime issueDate, DateTime expirationDate, bool isActive)
        {
            this.InternationalLicenseID = InternationalLicenseID;
            this.ApplicationID = ApplicationID;
            this.LicenseClassName = licenseClassName;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.IsActive = isActive;
        }
    }
}
