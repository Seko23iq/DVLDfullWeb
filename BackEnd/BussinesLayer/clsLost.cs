using System;
using System.Collections.Generic;
using System.Text;

namespace BussinesLayer
{
    public class clsLost
    {
        public static (int ApplicationID, int LicenseID) AddLostLicense(DtoLayer.LostApplication.LostDTO dto)
        {
            return DataLayer.clsLostData.AddLostLicense(dto);
        }
    }
}
