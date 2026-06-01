using DataLayer;
using DtoLayer.LicenseHistory;
using DtoLayer.LocalDrivingApplication;
using System;
using System.Data;
using System.Security.Cryptography.Pkcs;
using static DataLayer.clsPersonData;
namespace BussinesLayer
{
    public class LocalDrivingApplicationBussines
    {

        
        public static List<LocalDrivingApplicationDTO> GetAllRecords()
        {
            return LocalDrivingApplicationData.GetAllRecords();
        }
        public static List<LocalLicenseHistoryDTO> GetAllLicenseHistroyRecords(int PersonID)
        {
            return LocalDrivingApplicationData.GetAllLicenseHistroyRecords(PersonID);
        }
        public static List<LocalDrivingApplicationDTO> GetApplications(LocalDrivingApplicationData.ApplicationFilterType filterType, string value)
        {
            return LocalDrivingApplicationData.GetByFilter(filterType, value);
        }




        public static List<string> GetLicenseClasses()
        {
            return LocalDrivingApplicationData.GetLicenseClasses();
        }

        public static int AddNewLocalDrivingRecord(AddNewLocalDrivingLicenseDTO dTO)
        {
            return LocalDrivingApplicationData.AddNewLocalDrvingLicenseRecord_Data(dTO);
        }

        public static int GetUserIDByUsername(string Username)
        {
            return LocalDrivingApplicationData.GetUserIDByUsername(Username);
        }



        public static float GetPaidFeesLocalApplication()
        {
            return LocalDrivingApplicationData.GetPaidFeesLocalApplication();
        }

        public static int HowManyPassTestForThisLDL(int LocalDrivingLicenseApplicationID)
        {
            return LocalDrivingApplicationData.HowManyPassTestForThisLDL_Data(LocalDrivingLicenseApplicationID);
        }


        public static ApplicationBasicInfoDTO GetApplicationBasicInfoDTO(int LocalDrivingLicenseApplicationID)
        {
            return LocalDrivingApplicationData.GetApplicationBasicInfo(LocalDrivingLicenseApplicationID);
        }

        public static List<LocalLicenseDTO> GetAllLicenseRecordsData(string LocalDrivingLicenseApplicationID)
        {
            return LocalDrivingApplicationData.GetAllLicenseRecordsData(LocalDrivingLicenseApplicationID);
        }

        public static bool HasLicenseWithThisClass(int PersonID, int LicenseClassID)
        {
            return LocalDrivingApplicationData.HasLicenseWithThisClass(PersonID, LicenseClassID);
        }
        public static bool HasLicenseIssueWithThisClass(int personID, int licenseClassID)
        {
            return LocalDrivingApplicationData.HasLicenseIssueWithThisClass(personID, licenseClassID);
        }
        public static int issueDrivingLicenseFirst(IssueDrivingLicenseFirstTimeDTO dTO)
        {
            return LocalDrivingApplicationData.issueDrivingLicenseFirst(dTO);
        }

        public static int GetLicenseIDforApplicationID(int ApplicationID)
        {
            return LocalDrivingApplicationData.GetLicenseIDforApplicationID(ApplicationID);
        }
        public static int GetPersonIDforLocalApplicationID(int LocalApplicationID)
        {
            return LocalDrivingApplicationData.GetPersonIDforLocalApplicationID(LocalApplicationID);
        }
        public static LicenseInfoDTO GetDrivingLicenseInfo(int LicenseID)
        {
            LicenseInfoDTO licenseInfo = LocalDrivingApplicationData.GetDrivingLicenseInfo(LicenseID);
            if(licenseInfo != null)
            {
                licenseInfo.IsDetained = LocalDrivingApplicationData.IsLicenseDetain(LicenseID);
            }

            return licenseInfo;
        }
        public static bool IsLicenseDetain(int LicenseID)
        {
            return LocalDrivingApplicationData.IsLicenseDetain(LicenseID);
        }

        public static bool DeleteLocalDrivingLicenseApplication(int LocalDrivingLicenseApplicationID)
        {
            return LocalDrivingApplicationData.DeleteLocalDrivingLicenseApplication_Data(LocalDrivingLicenseApplicationID);
        }
        public static bool CancelLocalDrivingApplication(int LocalDrivingLicenseApplicationID)
        {
            return LocalDrivingApplicationData.CancelLocalDrivingApplication_Data(LocalDrivingLicenseApplicationID);
        }










        public static string GetApplicationID(string PersonID)
        {
            return LocalDrivingApplicationData.GetApplicationID_Data(PersonID);
        }

        public static int AddNewLDLApplication(string ApplicationPersonID, string ApplicationTypeID)
        {
            return LocalDrivingApplicationData.AddNewLDLApplication_Data(ApplicationPersonID, ApplicationTypeID);
        }
        

        public static bool DoesPersonIDHaveNewStautsOfThisClassType(string ApplicantPersonID, string ApplicationTypeID)
        {
            return LocalDrivingApplicationData.DoesPersonIDHaveNewStautsOfThisClassType_Data(ApplicantPersonID, ApplicationTypeID);
        }

    

    }
}
