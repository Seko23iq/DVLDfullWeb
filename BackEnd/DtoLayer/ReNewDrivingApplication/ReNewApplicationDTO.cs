using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.ReNewDrivingApplication
{
    public class ReNewApplicationDTO
    {
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public int ApplicationStatus { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }


        public ReNewApplicationDTO()
        {
        }
        public ReNewApplicationDTO(
            int applicantPersonID,
            DateTime applicationDate,
            int applicationTypeID,
            int applicationStatus,
            decimal paidFees,
            int createdByUserID)
        {
            this.ApplicantPersonID = applicantPersonID;
            this.ApplicationDate = applicationDate;
            this.ApplicationTypeID = applicationTypeID;
            this.ApplicationStatus = applicationStatus;
            this.PaidFees = paidFees;
            this.CreatedByUserID = createdByUserID;
        }

    }
}
