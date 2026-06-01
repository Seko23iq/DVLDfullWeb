using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Driver
{
    public class DriverDTO
    {
        public int DriverID { get; set; }
        public int PersonID { get; set; }
        public string NationalNo { get; set; }
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int NumberOfActiveLicenses { get; set; }
        public DriverDTO()
        {
        }
        public DriverDTO(int driverID, int personID, string nationalNo, string fullName, DateTime createdDate, int numberOfActiveLicenses)
        {
            this.DriverID = driverID;
            this.PersonID = personID;
            this.NationalNo = nationalNo;
            this.FullName = fullName;
            this.CreatedDate = createdDate;
            this.NumberOfActiveLicenses = numberOfActiveLicenses;
        }
    }
}
