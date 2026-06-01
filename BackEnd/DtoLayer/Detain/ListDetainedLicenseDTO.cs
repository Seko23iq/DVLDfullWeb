using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Detain
{
    public class ListDetainedLicenseDTO
    {
        public int DetainID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string NationalNo { get; set; }
        public string FullName { get; set; }
        public int? ReleaseApplicationID { get; set; }

        public ListDetainedLicenseDTO() { }
        public ListDetainedLicenseDTO(int detainID, int licenseID, DateTime detainDate, bool isReleased, decimal fineFees, DateTime? releaseDate, string nationalNo, string fullName, int? releaseApplicationID)
        {
            DetainID = detainID;
            LicenseID = licenseID;
            DetainDate = detainDate;
            IsReleased = isReleased;
            FineFees = fineFees;
            ReleaseDate = releaseDate;
            NationalNo = nationalNo;
            FullName = fullName;
            ReleaseApplicationID = releaseApplicationID;
        }
    }
}
