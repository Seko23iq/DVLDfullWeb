using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer
{
    public class IssueDLFirstTimeBussines
    {
        public static int AddNewDLBussines(string PersonID, string ApplicationID, string LicenseClass, string Notes)
        { 
            return IssueDLFirstTimeData.AddNewDLData(PersonID, ApplicationID, LicenseClass, Notes);
        }

        public static bool HasDLactiveBussines(string ApplicationID, string LicenseClass)
        {
            return IssueDLFirstTimeData.HasDLactiveData(ApplicationID, LicenseClass);
        }

        public static bool UpdateLicenseActiveBussines(string LicenseID)
        {
            return IssueDLFirstTimeData.UpdateLicenseActiveData(LicenseID);
        }
        
    }
}
