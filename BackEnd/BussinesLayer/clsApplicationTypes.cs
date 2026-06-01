using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtoLayer.ApplicationTypes;

namespace BussinesLayer
{
    public class clsApplicationTypes
    {
        public static List<ApplicationTypesDTO> GetAllRecords()
        {
            return clsApplicationTypesData.GetAllRecords();
        }
        public static ApplicationTypesDTO GetApplicationTypeInfo(int ApplicationTypeID)
        {
            return clsApplicationTypesData.GetApplicationTypeInfo(ApplicationTypeID);
        }
        public static bool UpdateApplicationType(ApplicationTypesDTO DTO)
        {
            return clsApplicationTypesData.UpdateApplicationType(DTO);
        }
    }
}
