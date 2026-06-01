using ContactsDataAccessLayer.RenewLicense;
using DtoLayer.ReNewDrivingApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer
{
    public class RenewLicenseBussines
    {

        public static (int,int) AddRenewLicense(AddRenewLicenseDTO dto)
        {
            return RenewLicenseData.AddRenewLicense(dto);
        }

        public static bool IsLicenseExpire(int LicenseID)
        {
            return RenewLicenseData.IsLicenseExpire(LicenseID);
        }
    }
}
