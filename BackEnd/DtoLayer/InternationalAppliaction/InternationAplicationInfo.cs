using System;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DtoLayer.InternationalAppliaction
{
    public class InternationalApplicationInfoDto
    {
//        Name
//IntLicenseID
//ApplicationID
//lICENSEid
//IsActive
//NationalNo
//DateOfBirth
//Gendor
//DriverID
//IssueDate
//ExpirationDate
//ImagePath
        public string FullName { get; set; }
        public int InternationalLicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int LicenseID { get; set; }
        public bool IsActive { get; set; }
        public string NationalNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public byte Gendor { get; set; }
        public int DriverID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ImagePath { get; set; }

        // Empty Constructor
        public InternationalApplicationInfoDto()
        {

        }

        // Full Constructor
        public InternationalApplicationInfoDto(
            string fullName,
            int internationalLicenseID,
            int applicationID,
            int licenseID,
            bool isActive,
            string nationalNo,
            DateTime dateOfBirth,
            byte gendor,
            int driverID,
            DateTime issueDate,
            DateTime expirationDate)
        {
            FullName = fullName;
            InternationalLicenseID = internationalLicenseID;
            ApplicationID = applicationID;
            LicenseID = licenseID;
            IsActive = isActive;
            NationalNo = nationalNo;
            DateOfBirth = dateOfBirth;
            Gendor = gendor;
            DriverID = driverID;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
        }
    }
}