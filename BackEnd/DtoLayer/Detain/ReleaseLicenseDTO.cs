using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Detain
{
    public class ReleaseLicenseDTO
    {
        public ReleaseDetainedApplicationDTO ReleaseDetainedApplication { get; set; }
        public int LicenseID { get; set; }
        public int ReleasedByUserID { get; set; }
        public int DetainID { get; set; }
        public ReleaseLicenseDTO() { }
        public ReleaseLicenseDTO( int licenseID, int ReleasedByUserID, int DetainID, ReleaseDetainedApplicationDTO releaseDetainedApplication)
        {
            this.LicenseID = licenseID;
            this.ReleasedByUserID = ReleasedByUserID;
            this.DetainID = DetainID;
            this.ReleaseDetainedApplication = releaseDetainedApplication;
        }
    }
}
