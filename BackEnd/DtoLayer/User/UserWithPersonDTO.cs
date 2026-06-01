using DtoLayer.Person;
using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.User
{
    public class UserWithPersonDTO
    {
        public UserDTO User { get; set; }
        public PeopleDTO Person { get; set; }
    }
}
