using DtoLayer.DamageApplication;
using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LostApplication
{
    public class LostDTO
    {
        public int OldLicenseID { get; set; }
        public LostApplicationDTO lostApplication { get; set; }
        public LostLicenseDTO lostLicense { get; set; }
        public LostDTO()
        {

        }
        public LostDTO(int OldLicenseID, LostApplicationDTO lostApplication, LostLicenseDTO lostLicense)
        {
            this.OldLicenseID = OldLicenseID;
            this.lostApplication = lostApplication;
            this.lostLicense = lostLicense;
        }
    }
}
