using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Application
{
    public class testAppointmentsDTO
    {
        public int testAppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public decimal PaidFees { get; set; }
        public bool IsLocked { get; set; }

        public testAppointmentsDTO()
        {

        }

        public testAppointmentsDTO(int testAppointmentID, DateTime AppointmentDate, decimal PaidFees, bool IsLocked)
        {
            this.testAppointmentID = testAppointmentID;
            this.AppointmentDate = AppointmentDate;
            this.PaidFees = PaidFees;
            this.IsLocked = IsLocked;
        }
    }
}
