using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.LostApplication
{
    public class LostApplicationDTO
    {
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; private set; } = DateTime.Now;
        public int ApplicationTypeID { get; private set; } = 3;
        public int ApplicationStatus { get; private set; } = 3;
        public DateTime LastStatusDate { get; private set; } = DateTime.Now;
        public decimal PaidFees { get; private set; } = 10;
        public int CreatedByUserID { get; set; }

        public LostApplicationDTO()
        {
        }

        public LostApplicationDTO(int applicantPersonID, int createdByUserID)
        {
            ApplicantPersonID = applicantPersonID;
            CreatedByUserID = createdByUserID;
        }
    }
}
