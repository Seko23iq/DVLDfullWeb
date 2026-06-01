using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LocalDrivingApplication
{
    public class AddNewLocalDrivingLicenseDTO
    {
        public int ApplicantPersonID { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public int LicenseClassID { get; set; }

        public AddNewLocalDrivingLicenseDTO()
        {
        }
        public AddNewLocalDrivingLicenseDTO(
            int applicantPersonID,
            decimal paidFees,
            int createdByUserID,
            int licenseClassID)
        {
            this.ApplicantPersonID = applicantPersonID;
            this.PaidFees = paidFees;
            this.CreatedByUserID = createdByUserID;
            this.LicenseClassID = licenseClassID;
        }
    }
}
