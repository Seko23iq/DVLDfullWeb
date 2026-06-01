using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.ReNewDrivingApplication
{
    public class AddRenewLicenseDTO
    {
        public int OldLicenseID { get; set; }
        public ReNewApplicationDTO Application { get; set; }
        public ReNewLicenseDTO License { get; set; }
        public AddRenewLicenseDTO()
        {
        }
        public AddRenewLicenseDTO(int OldLicenseID, ReNewApplicationDTO application, ReNewLicenseDTO license)
        {
            this.OldLicenseID = OldLicenseID;
            this.Application = application;
            this.License = license;
        }
    }
}
