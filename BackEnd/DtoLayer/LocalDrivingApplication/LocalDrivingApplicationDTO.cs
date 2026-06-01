using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LocalDrivingApplication
{
    public class LocalDrivingApplicationDTO
    {
        public LocalDrivingApplicationDTO(string l_D_L_AppID, string drivingClass, string nationalNo, string fullName, DateTime? applicationDate, int passedTests, string status)
        {
            L_D_L_AppID = l_D_L_AppID;
            DrivingClass = drivingClass;
            NationalNo = nationalNo;
            FullName = fullName;
            ApplicationDate = applicationDate;
            PassedTests = passedTests;
            Status = status;
        }
        public string L_D_L_AppID { get; set; }
        public string DrivingClass { get; set; }
        public string NationalNo { get; set; }
        public string FullName { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public int PassedTests { get; set; }
        public string Status { get; set; }
    }
}
