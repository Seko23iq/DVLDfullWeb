using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Detain
{
    public class DetainLicenseDTO
    {
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }
        public int CreatedByUserID { get; set; }

        public DetainLicenseDTO()
        {
        }
        public DetainLicenseDTO(int licenseID, DateTime detainDate, decimal fineFees, int createdByUserID)
        {
            LicenseID = licenseID;
            DetainDate = detainDate;
            FineFees = fineFees;
            CreatedByUserID = createdByUserID;
        }
    }
}
