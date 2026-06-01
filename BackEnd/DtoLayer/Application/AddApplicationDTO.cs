using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Application
{
    public class AddApplicationDTO
    {
        public int TestTypeID { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int CreatedByUserID { get; set; }
        public bool IsLocked { get; set; }

        public AddApplicationDTO()
        {
        }

        public AddApplicationDTO(int testTypeID, int localDrivingLicenseApplicationID,
            DateTime appointmentDate, int createdByUserID, bool isLocked)
        {
            TestTypeID = testTypeID;
            LocalDrivingLicenseApplicationID = localDrivingLicenseApplicationID;
            AppointmentDate = appointmentDate;
            CreatedByUserID = createdByUserID;
            IsLocked = isLocked;
        }
    }
}
