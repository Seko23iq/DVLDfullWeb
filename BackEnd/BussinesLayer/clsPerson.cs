using DataLayer;
using DtoLayer.Person;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static DataLayer.clsPersonData;

namespace BussinesLayer
{
    public class clsPerson
    {
        public static int _AddNewPerson(AddPersonDTO personDTO)
        {
            return clsPersonData.AddNewPersonData(personDTO);
        }
        public static bool UpdatePersonInfo(UpdatePersonDTO personDTO)
        {
            return clsPersonData.UpdatePerson(personDTO);
        }
        public static List<PeopleDTO> GetAllPeopleDTO()
        {
            return clsPersonData.GetAllPeopleDTO();
        }
        public static List<PeopleDTO> GetInfoBy(string filterType, string value)
        {
            switch (filterType)
            {
                case "PersonID":
                    return clsPersonData.GetInfoByData("PersonID", value);

                case "NationalNo":
                    return clsPersonData.GetInfoByData("NationalNo", value);

                case "Nationality":
                    return clsPersonData.GetInfoByData("Nationality", value);

                case "Name":
                    return clsPersonData.GetInfoByData("Name", value);

                case "Gender":
                    return clsPersonData.GetInfoByData("Gendor", value);

                case "Email":
                    return clsPersonData.GetInfoByData("Email", value);

                default:
                    throw new Exception("Invalid filter type");
            }
        }
        public static bool DeletePerson(int PersonID)
        {
            return clsPersonData.DeletePersonData(PersonID);
        }
        public static PeopleDTO GetPersonInfo(int PersonID)
        {
            return clsPersonData.GetPersonInfo(PersonID);
        }
        public static PeopleDTO GetPersonInfoByNationalNo(string NationalNo)
        {
            return clsPersonData.GetPersonInfoByNationalNo(NationalNo);
        }
        public static bool IsPersonExsitsBy(string DataType,string DataValue)
        {
           return clsPersonData.IsPersonExsitsByData(DataType, DataValue);
        }
    }
}
