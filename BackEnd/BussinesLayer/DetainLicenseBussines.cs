using DataLayer;
using DtoLayer.Detain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer
{
    public class DetainLicenseBussines
    {

        public static List<ListDetainedLicenseDTO> GetAllRecords()
        {
            return DetainLicenseData.GetAllRecords();
        }
        
        public static int AddDetainLicense(DetainLicenseDTO dTO)
        {
            return DetainLicenseData.AddDetainLicenseData(dTO);
        }

        public static DetainLicenseInfoDTO GetDetainLicenseInfo(int LicenseID)
        {
            return DetainLicenseData.GetDetainInfoData(LicenseID);
        }

        public static int ReleaseLicense(ReleaseLicenseDTO dTO)
        {
            return DetainLicenseData.ReleaseLicense(dTO);
        }

        public static DataTable GetInfoBy(string Type,string Value)
        {
            return DetainLicenseData.GetInfoBy_DataLayer(Type,Value);
        }

    }
}
