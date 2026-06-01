using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.InternationalAppliaction
{
    public class AddInternationalApplicationDTO
    {
        public int ApplicantPersonID { get; set; }
        public int DriverID { get; set; }
        public int IssuedUsingLocalLicenseID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int CreatedByUserID { get; set; }
        public decimal PaidFees { get; set; }
        public AddInternationalApplicationDTO() { }
        public AddInternationalApplicationDTO(int ApplicantPersonID,int driverID, int issuedUsingLocalLicenseID, DateTime issueDate, DateTime expirationDate,int createdByUserID, decimal paidFees)
        {
            this.ApplicantPersonID = ApplicantPersonID;
            this.DriverID = driverID;
            this.IssuedUsingLocalLicenseID = issuedUsingLocalLicenseID;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.CreatedByUserID = createdByUserID;
            this.PaidFees = paidFees;
        }
    }
}
