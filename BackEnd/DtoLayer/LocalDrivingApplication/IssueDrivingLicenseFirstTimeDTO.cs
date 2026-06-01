using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LocalDrivingApplication
{
    public class IssueDrivingLicenseFirstTimeDTO
    {
        public int PersonID { get; set; }
        public int ApplicationID { get; set; }
        public int LicenseClass { get; set; }
        public string Notes { get; set; }
        public IssueDrivingLicenseFirstTimeDTO()
        {
        }
        public IssueDrivingLicenseFirstTimeDTO(int personID, int applicationID, int licenseClass, string notes)
        {
            this.PersonID = personID;
            this.ApplicationID = applicationID;
            this.LicenseClass = licenseClass;
            this.Notes = notes;
        }
    }
}
