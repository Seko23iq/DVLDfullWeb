using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Test
{
    public class TakeTestDTO
    {
        public int TestAppointmentID { get; set; }
        public int TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

        public TakeTestDTO()
        {
        }

        public TakeTestDTO(int TestAppointmentID, int TestResult, string Notes, int CreatedByUserID)
        {
            this.TestAppointmentID = TestAppointmentID;
            this.TestResult = TestResult;
            this.Notes = Notes;
            this.CreatedByUserID = CreatedByUserID;
        }
    }
}
