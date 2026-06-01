using BussinesLayer;
using DtoLayer.Detain;
using DtoLayer.Person;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetainReleaseLicenseController : ControllerBase
    {
        [HttpPost("DetainLicense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DetainLicense([FromBody] DtoLayer.Detain.DetainLicenseDTO dTO)
        {
            try
            {
                int DetainID = BussinesLayer.DetainLicenseBussines.AddDetainLicense(dTO);
                if (DetainID > 0)
                {
                    return Ok(new { Message = "License detained successfully.", DetainID });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to detain the license." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }


        [HttpGet("GetDetainInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetDetainInfo(int LicenseID)
        {
            if (LicenseID <= 0)
            {
                return BadRequest("Invalid License ID.");
            }

            try
            {
                DetainLicenseInfoDTO detainLicenseInfo = DetainLicenseBussines.GetDetainLicenseInfo(LicenseID);

                if (detainLicenseInfo == null)
                {
                    return NotFound($"License with ID {LicenseID} not found.");
                }

                return Ok(detainLicenseInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }


        [HttpPost("ReleaseLicense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ReleaseLicense([FromBody] ReleaseLicenseDTO dTO)
        {
            try
            {
                int ReleasedApplicationID = DetainLicenseBussines.ReleaseLicense(dTO);
                if (ReleasedApplicationID >= 0)
                {
                    return Ok(new { ReleasedApplicationID = ReleasedApplicationID });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to release the license." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }


        [HttpGet("GetAllDeatinedLicenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllDeatinedLicenses()
        {
            try
            {
                List<ListDetainedLicenseDTO> detainedLicenses = DetainLicenseBussines.GetAllRecords();

                if(detainedLicenses.Count <= 0)
                {
                    return NotFound("There is no detained licenses.");
                }
                return Ok(detainedLicenses);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, new { Message = "An error occurred while processing your request.", Error = ex.Message });
            }
        }
    }
}