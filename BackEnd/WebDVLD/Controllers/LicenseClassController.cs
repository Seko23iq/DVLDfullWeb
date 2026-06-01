using BussinesLayer;
using DataLayer;
using DtoLayer;
using DtoLayer.Person;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LicenseClassController : ControllerBase
    {

        [HttpGet("LicenseClassInfo/{ClassName}", Name = "GetLicenseClassInfoById")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<LicenseClassInfoDTO> GetLicenseClassInfo(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName))
            {
                return BadRequest("Invalid License Class Name.");
            }

            try
            {
                LicenseClassInfoDTO dTO = LicenseClass.GetLicenseClassInfo(ClassName);

                if (dTO == null)
                {
                    return NotFound($"License Class with name {ClassName} not found.");
                }

                return Ok(dTO);
            }
            catch (Exception ex)
            {
                //return StatusCode(500, ex.Message); // مؤقتاً
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }


    }
}
