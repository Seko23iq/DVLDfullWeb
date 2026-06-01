using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Detain
{
    public class ReleaseDetainedApplicationDTO
    {
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; private set; } = DateTime.Now;
        public int ApplicationTypeID { get; private set; } = 5;
        public int ApplicationStatus { get; private set; } = 3;
        public DateTime LastStatusDate { get; private set; } = DateTime.Now;
        public decimal PaidFees { get; private set; } = 15;
        public int CreatedByUserID { get; set; }

        public ReleaseDetainedApplicationDTO() { }
        public ReleaseDetainedApplicationDTO(int applicantPersonID, int createdByUserID)
        {
            ApplicantPersonID = applicantPersonID;
            CreatedByUserID = createdByUserID;
        }
    }
}
