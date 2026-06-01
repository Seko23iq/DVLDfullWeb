using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.User
{
    public class UserAddDTO
    {
        public int PersonID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int IsActive { get; set; } = 1;

        public UserAddDTO()
        {
        }

        public UserAddDTO(int PersonID, string UserName, string Password, int IsActive)
        {
            this.PersonID = PersonID;
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;
        }
   
    }
}
