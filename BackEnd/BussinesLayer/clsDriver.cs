using DataLayer;
using DtoLayer.Driver;
using System.Data;


namespace BussinesLayer
{
    public class clsDriver
    {
        public static List<DriverDTO> GetAllDrivers()
        {
            return clsDriverData.GetAllDrivers();
        }
        //public static DataTable GetAllRecords()
        //{
        //    return clsDriverData.GetAllRecords();
        //}

        public static DataTable SearchForRecordsBy(string FilterBy, string SearchText)
        {
            return clsDriverData.SearchRecords(FilterBy, SearchText);
        }

        public static DataTable GetAllLicenseRecords(string LocalDrivingLicenseApplicationID)
        {
            return clsDriverData.GetAllLicenseRecordsData(LocalDrivingLicenseApplicationID);
        }

    }
}
