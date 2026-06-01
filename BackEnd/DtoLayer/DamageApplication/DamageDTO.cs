using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.DamageApplication
{
    public class DamageDTO
    {
        public int OldLicenseID { get; set; }
        public DamageApplicationDTO DamageApplication { get; set; }
        public DamagedLicenseDTO damagedLicense { get; set; }
        public DamageDTO()
        {

        }
        public DamageDTO(int OldLicenseID, DamageApplicationDTO damageApplication, DamagedLicenseDTO damagedLicense)
        {
            this.OldLicenseID = OldLicenseID;
            this.DamageApplication = damageApplication;
            this.damagedLicense = damagedLicense;
        }
    }
}
