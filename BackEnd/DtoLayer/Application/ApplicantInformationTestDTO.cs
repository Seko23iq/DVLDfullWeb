using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Application
{
    public class ApplicantInformationTestDTO
    {
        public int LocalDrivingLicenseApplicationID { get; set; }
        public string ApplicantName { get; set; }
        public string ApplicantClass { get; set; }
        public decimal Fees { get; set; }

        public ApplicantInformationTestDTO()
        {
        }

        public ApplicantInformationTestDTO(int localDrivingLicenseApplicationID, string applicantName, string applicantClass, decimal fees)
        {
            LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            ApplicantName = applicantName;
            ApplicantClass = applicantClass;
            Fees = fees;
        }
    }
}
