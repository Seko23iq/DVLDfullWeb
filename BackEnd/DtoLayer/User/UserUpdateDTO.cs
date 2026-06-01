using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.User
{
    public class UserUpdateDTO
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int IsActive { get; set; }

        public UserUpdateDTO() { }
        public UserUpdateDTO(int UserID, string UserName, string Password, int IsActive)
        {
            this.UserID = UserID;
            this.UserName = UserName;
            this.Password = Password;
            this.IsActive = IsActive;
        }

    }
}
