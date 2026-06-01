using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DtoLayer.Test
{
    public class LocalAndTestTypeidDTO
    {
        [Range(1, int.MaxValue,
       ErrorMessage = "Local Driving License Application ID must be larger than 0")]
        public int LocalDrivingLicenseApplicationID { get; set; }

        [Range(1, 3,
        ErrorMessage = "TestTypeID must be between 1 and 3")]
        public int TestTypeID { get; set; }

    }
}
