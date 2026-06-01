using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Numerics;
using DataLayer;
using BussinesLayer;
using DtoLayer.User;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        [HttpPost("SignIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult SignIn([FromBody] LoginDTO dTO)
        {
            if (dTO == null)
                return BadRequest("Invalid data.");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            bool isLogin = clsUser.LoginIn(dTO.UserName, dTO.Password);

            if (!isLogin)
            {
                return Unauthorized("Invalid username or password.");
            }
            return Ok("ok");
        }

        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> GetAllUsers()
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<UserDTO> userDTOs = clsUser.GetAllUsers();

            if (userDTOs == null ||  userDTOs.Count <= 0)
            {
                return NotFound("No users found.");
            }

            return Ok(userDTOs);
        }


        [HttpGet("UserInfo/{UserID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDTO> GetUserInfo(int UserID)
        {
            if (UserID <= 0)
            {
                return BadRequest("Invalid UserID.");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserDTO userDTO = clsUser.GetInfoByUserID(UserID);

            if (userDTO == null)
            {
                return NotFound($"No user found with UserID: {UserID}");
            }

            var person = clsPerson.GetPersonInfo(userDTO.PersonID);

            var result = new UserWithPersonDTO
            {
                User = userDTO,
                Person = person
            };

            return Ok(result);
        }


        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UserDTO>> FilterUsers(
     int? UserID,
     int? PersonID,
     string? FullName,
     string? UserName,
     int? IsActive)
        {
            List<UserDTO> list = new List<UserDTO>();

            if (UserID.HasValue)
            {
                list = clsUser.GetInfoByUserIDList(UserID.Value);
            } 
            else if (PersonID.HasValue)
            {
                list = clsUser.GetInfoByPersonID(PersonID.Value);
            }
            else if (!string.IsNullOrEmpty(FullName))
            {
                list = clsUser.GetInfoByFullName(FullName);
            }
            else if (!string.IsNullOrEmpty(UserName))
            {
                list = clsUser.GetInfoByUserName(UserName);
            }
            else if (IsActive.HasValue)
            {
                list = clsUser.GetInfoByUserIsActive(IsActive.Value);
            }
            else
            {
                return BadRequest("Please provide at least one filter.");
            }

            if (list == null || list.Count == 0)
                return NotFound("No matching data found.");

            return Ok(list);
        }



        [HttpDelete("Delete/{userID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteUser(int userID)
        {
            if (userID <= 0)
                return BadRequest("Invalid User ID");

            if (!clsUser.IsUserExists(userID))
                return NotFound("User not found");

            try
            {
                bool isDeleted = clsUser.DeleteUserByUserId(userID);

                if (isDeleted)
                    return Ok("User deleted successfully");

                return BadRequest("Failed to delete user");
            }
            catch (Exception ex)
            {
                // log error here
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost("AddUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddUser([FromBody] UserAddDTO userAddDTO)
        {
            if(userAddDTO == null)
            {
                return BadRequest("No user info");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(clsUser.IsUserExistsByPersonID(userAddDTO.PersonID))
            {
                return BadRequest("User alerady exsits");
            }

            int newUserID = clsUser.AddNewUser(userAddDTO);

            if(newUserID <= 0)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            else
            {
                return Ok("User added successfully");
            }


        }



        [HttpPut("UpdateUser/{userID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateUser(int userID,[FromBody] UserUpdateDTO userUpdateDTO)
        {
            if(userUpdateDTO == null)
            {
                return BadRequest("Invalid data!");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            userUpdateDTO.UserID = userID;
            bool isUpdated = clsUser.UpdateUser(userUpdateDTO);


            if (!isUpdated)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }

            return Ok(userUpdateDTO);
        }



        [HttpGet("ImagePath")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetImagePath(string Username)
        {
            if(string.IsNullOrEmpty(Username))
            {
                return BadRequest("Invalid Username");
            }

            string ImagePath = clsUser.GetImagePathByUsername(Username);

            if(string.IsNullOrEmpty(ImagePath))
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }

            return Ok(ImagePath);

        }



        [HttpGet("UserID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetUserID(string Username)
        {
            if (string.IsNullOrEmpty(Username))
            {
                return BadRequest("Invalid Username");
            }

            int UserID = clsUser.GetUserIDbyUsername(Username);

            if (UserID <= 0)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }

            return Ok(UserID);

        }



        [HttpGet("Active")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetActiveStatus(string Username)
        {
            if (string.IsNullOrEmpty(Username))
            {
                return BadRequest("Invalid Username");
            }

            int isActive = clsUser.GetActiveByUsername(Username);

            if (isActive < 0 || isActive > 1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }

            return Ok(isActive);

        }
    }
}
