using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace DtoLayer.DamageApplication
{
    public class DamageApplicationDTO
    {
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; private set; } = DateTime.Now;
        public int ApplicationTypeID { get; private set; } = 4;
        public int ApplicationStatus { get; private set; } = 3;
        public DateTime LastStatusDate { get; private set; } = DateTime.Now;
        public decimal PaidFees { get; private set; } = 5;
        public int CreatedByUserID { get; set; }

        public DamageApplicationDTO()
        {
        }

        public DamageApplicationDTO(int applicantPersonID, int createdByUserID)
        {
            ApplicantPersonID = applicantPersonID;
            CreatedByUserID = createdByUserID;
        }
    }
}
