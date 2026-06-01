using ContactsDataAccessLayer;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinesLayer
{
    public class ShowDrivingLicenseBussines
    {
        public struct stDrivingLicenseBussinesLayer
        {
            public string PersonID;
            public string ClassName;
            public string FullName;
            public string LicenseID;
            public string LocalDrivingLicenseApplicationID;
            public string ApplicationID;
            public string NationalNo;
            public string Gendor;
            public DateTime? IssueDate;
            public string IssueReason;
            public string Notes;
            public string IsActive;
            public DateTime? DateOfBirth;
            public string DriverID;
            public DateTime? ExpirationDate;
            public string ImagePath;

        }


        public static stDrivingLicenseBussinesLayer GetDrivingInfoBy(string Type,string Value)
        {
            ShowDrivingLicenseData.stDrivingLicense DrivingLicense = ShowDrivingLicenseData.GetDrivingLicenseInfo(Type,Value);

            stDrivingLicenseBussinesLayer detailsBussinesLayer = new stDrivingLicenseBussinesLayer();

            detailsBussinesLayer.PersonID = DrivingLicense.PersonID;
            detailsBussinesLayer.ClassName = DrivingLicense.ClassName;
            detailsBussinesLayer.FullName = DrivingLicense.FullName;
            detailsBussinesLayer.LicenseID = DrivingLicense.LicenseID;
            detailsBussinesLayer.LocalDrivingLicenseApplicationID = DrivingLicense.LocalDrivingLicenseApplicationID;
            detailsBussinesLayer.ApplicationID = DrivingLicense.ApplicationID;
            detailsBussinesLayer.NationalNo = DrivingLicense.NationalNo;
            detailsBussinesLayer.Gendor = DrivingLicense.Gendor;
            detailsBussinesLayer.IssueDate = DrivingLicense.IssueDate;
            detailsBussinesLayer.IssueReason = DrivingLicense.IssueReason;
            detailsBussinesLayer.Notes = DrivingLicense.Notes;
            detailsBussinesLayer.IsActive = DrivingLicense.IsActive;
            detailsBussinesLayer.DateOfBirth = DrivingLicense.DateOfBirth;
            detailsBussinesLayer.DriverID = DrivingLicense.DriverID;
            detailsBussinesLayer.ExpirationDate = DrivingLicense.ExpirationDate;
            detailsBussinesLayer.ImagePath = DrivingLicense.ImagePath;

            return detailsBussinesLayer;
        }


    }
}
