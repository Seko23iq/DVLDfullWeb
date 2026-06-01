using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.Country
{
    public class CountriesDTO
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }

        public CountriesDTO() { }
        public CountriesDTO(int CountryID, string CountryName)
        {
            this.CountryID = CountryID;
            this.CountryName = CountryName;
        }
    }
}
