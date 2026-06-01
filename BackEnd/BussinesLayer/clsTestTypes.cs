using DataLayer;
using System.Data;
using DtoLayer.TestTypes;

namespace BussinesLayer
{
    public class clsTestTypes
    {
        public static List<TestTypesDTO> GetAllRecords()
        {
            return clsTestTypesData.GetAllRecords();
        }
        public static TestTypesDTO GetApplicationTypeInfo(int ID)
        {
            return clsTestTypesData.GetTypeInfoByID(ID);
        }
        public static bool UpdateApplicationType(TestTypesDTO DTO)
        {
            return clsTestTypesData.UpdateApplicationType(DTO);
        }
    }
}
