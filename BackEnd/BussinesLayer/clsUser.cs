using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DtoLayer.User;

namespace BussinesLayer
{
    public class clsUser
    {
        public static bool LoginIn(string UserName, string Password)
        {
            return clsUserData.LoginInData(UserName, Password);
        }
        public static List<UserDTO> GetAllUsers()
        {
            return clsUserData.GetAllUsersDTO();
        }
        public static UserDTO GetInfoByUserID(int UserID)
        {
            return clsUserData.GetInfoByUserID(UserID);
        }
        public static List<UserDTO> GetInfoByData(string DataType, string Datavalue)
        {
            switch (DataType)
            {
                case "UserID":
                    return clsUserData.GetInfoByUserID_DataLayer(int.Parse(Datavalue));
                case "PersonID":
                    return clsUserData.GetInfoByPersonID_DataLayer(int.Parse(Datavalue));
                case "FullName":
                    return clsUserData.GetInfoByFullName_DataLayer(Datavalue);
                case "UserName":
                    return clsUserData.GetInfoByUserName_DataLayer(Datavalue);
                case "IsActive":
                return clsUserData.GetInfoByUserIsActive_DataLayer(int.Parse(Datavalue));
                default:
                    return clsUserData.GetAllUsersDTO();
            }
        }
        public static List<UserDTO> GetInfoByUserIDList(int UserID)
        {
            return clsUserData.GetInfoByUserID_DataLayer(UserID);
        }
        public static List<UserDTO> GetInfoByPersonID(int PersonID)
        {
            return clsUserData.GetInfoByPersonID_DataLayer(PersonID);
        }
      
        public static List<UserDTO> GetInfoByFullName(string FullName)
        {
            return clsUserData.GetInfoByFullName_DataLayer(FullName);
        }
        public static List<UserDTO> GetInfoByUserName(string UserName)
        {
            return clsUserData.GetInfoByUserName_DataLayer(UserName);
        }

        public static List<UserDTO> GetInfoByUserIsActive(int Status)
        {
            return clsUserData.GetInfoByUserIsActive_DataLayer(Status);
        }
        public static bool IsUserExists(int UserID)
        {
            return clsUserData.IsUserExists(UserID);
        }
        public static bool IsUserExistsByPersonID(int PersonID)
        {
            return clsUserData.IsUserExistsByPersonID(PersonID);
        }
        public static bool DeleteUserByUserId(int UserID)
        {
            return clsUserData.DeleteUserData(UserID);
        }
        public static int AddNewUser(UserAddDTO userAddDTO)
        {
            return clsUserData.AddNewUser(userAddDTO);
        }
        public static bool UpdateUser(UserUpdateDTO userUpdateDTO)
        {
            return clsUserData.UpdateUserData(userUpdateDTO);
        }
        public static string GetImagePathByUsername(string Username)
        {
            return clsUserData.GetImagePathByUsername(Username);
        }
        public static int GetUserIDbyUsername(string Username)
        {
            return clsUserData.GetUserIDByUsername(Username);
        }

        public static int GetActiveByUsername(string Username)
        {
            return clsUserData.GetActiveByUsername(Username);
        }












        public static bool UpdatePassword(string UpdatedPassword, string UserID)
        {
            return clsUserData.UpdatePassword(UpdatedPassword, UserID);
        }
        public static string CurrentPassword(string UserID)
        {
            return clsUserData.CurrentPassword(UserID);
        }
        //public static string UserIDbyUsername(string Username)
        //{
        //    return clsUserData.GetUserIDByUsername(Username);
        //}
        public static string PersonIDbyUsername(string Username)
        {
            return clsUserData.GetPersonIDByUsername(Username);
        }
        public static bool IsActiveByUsername(string Username)
        {
            return clsUserData.GetIsActiveByUsername(Username);
        }
        public static bool IsPersonUserByPersonID(string PersonID)
        {
            if (clsUserData.IsPersonUserByPersonIDData(PersonID) == true)
                return true;
            else
                return false;
        }
    }
}
