using System;
using System.Collections.Generic;
using System.Text;

namespace BussinesLayer
{
    public class clsDamage
    {
        public static (int ApplicationID, int LicenseID) AddDamageLicense(DtoLayer.DamageApplication.DamageDTO dto)
        {
            return DataLayer.clsDamageData.AddDamageLicense(dto);
        }
    }
}
