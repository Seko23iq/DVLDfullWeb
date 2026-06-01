using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer
{
    //LicenseClassID ClassName   ClassDescription MinimumAllowedAge   DefaultValidityLength ClassFees

    public class LicenseClassInfoDTO
    {
        public int LicenseClassID { get; set; }
        public string ClassName { get; set; }
        public string ClassDescription { get; set; }
        public int MinimumAllowedAge { get; set; }
        public int DefaultValidityLength { get; set; }
        public decimal ClassFees { get; set; }  
        public LicenseClassInfoDTO() { }

        public LicenseClassInfoDTO(int licenseClassID, string className, string classDescription, int minimumAllowedAge, int defaultValidityLength, decimal classFees)
        {
            this.LicenseClassID = licenseClassID;
            this.ClassName = className;
            this.ClassDescription = classDescription;
            this.MinimumAllowedAge = minimumAllowedAge;
            this.DefaultValidityLength = defaultValidityLength;
            this.ClassFees = classFees;
        }
    }
}
