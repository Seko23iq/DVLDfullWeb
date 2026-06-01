using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Test
{
    public class TakerTestInfoDTO
    {

        public int DrivingLicenseApplicationID { get; set; }
        public string LicenseClass { get; set; }
        public string Name { get; set; }
        //public int Trial { get; set; }
        public DateTime Date { get; set; }
        public decimal Fees { get; set; }

        public TakerTestInfoDTO()
        {

        }

        public TakerTestInfoDTO(
            int drivingLicenseApplicationID,
            string licenseClass,
            string name,
            DateTime date,
            decimal fees
            )
        {
            this.DrivingLicenseApplicationID = drivingLicenseApplicationID;
            this.LicenseClass = licenseClass;
            this.Name = name;
            this.Date = date;
            this.Fees = fees;
        }
    }
}
