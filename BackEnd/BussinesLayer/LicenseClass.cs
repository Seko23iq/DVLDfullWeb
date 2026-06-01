using DtoLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace BussinesLayer
{
    public class LicenseClass
    {
        public static LicenseClassInfoDTO GetLicenseClassInfo(string ClassName)
        {
            return DataLayer.clsLicenseClassData.GetLicenseClassInfo(ClassName);
        }
    }
}
