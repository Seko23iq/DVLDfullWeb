using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Detain
{
    public class DetainLicenseInfoDTO
    {
        public int DetainID { get; set; }
        public DateTime DetainDate { get; set; }
        public decimal FineFees { get; set; }
        public string UserName { get; set; }

        public DetainLicenseInfoDTO() { }
        public DetainLicenseInfoDTO(int detainID, DateTime detainDate, decimal fineFees, string userName)
        {
            DetainID = detainID;
            DetainDate = detainDate;
            FineFees = fineFees;
            UserName = userName;
        }
    }
}
