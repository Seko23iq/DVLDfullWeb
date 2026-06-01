using DataLayer;
using DtoLayer.InternationalAppliaction;
using DtoLayer.LicenseHistory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer
{
    public class InternationalLicenseBussines
    {
        static public NewInternationalLicenseResultDTO AddNewInternationalLicense(AddInternationalApplicationDTO dTO)
        {
            return InternationalLicenseData.AddNewInternationalLicenseData(dTO);
        }

        public static int GetPersonIDByLicenseID(int LicenseID)
        {
            return InternationalLicenseData.GetPersonIDByLicenseID(LicenseID);
        }
        public static DataTable GetAllInternationalRecords(string InternationalLicenseID)
        {
            return InternationalLicenseData.GetAllInternationalRecordsData(InternationalLicenseID);
        }

        public static List<InternationalLicenseDTO> GetAllAllInternationalLicenseRecords()
        {
            return InternationalLicenseData.GetAllAllInternationalLicenseRecordsData();
        }

        public static DataTable GetInfoBy(string Type, int Value)
        {
            return InternationalLicenseData.GetInfoBy_DataLayer(Type, Value);
        }

        public static bool HasInternationalLicense(int DriverID)
        {
            return InternationalLicenseData.HasInternationalLicenseData(DriverID);
        }


        public static InternationalApplicationInfoDto GetInternationApplicationInfo(int InternationalApplicationID)
        {
            var dto = InternationalLicenseData.GetInternationApplicationInfo(InternationalApplicationID);

            return dto;
        }

        public static int GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            return InternationalLicenseData.GetActiveInternationalLicenseIDByDriverID(DriverID);
        }

        public static List<InternationalLicenseHistoryDTO> GetAllInternationalLicenseHistroyRecordsByPersonID(int PersonID)
        {
            return InternationalLicenseData.GetAllInternationalLicenseHistroyRecordsByPersonID(PersonID);
        }


    }
}
