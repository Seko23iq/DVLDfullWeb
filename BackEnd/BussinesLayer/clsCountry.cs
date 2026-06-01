using DataLayer;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace BussinesLayer
{
    public class clsCountry
    {
   
        public static List<string> GetAllCountry()
        {
            return clsCountryData.GetAllCountry();
        }
    }
}
