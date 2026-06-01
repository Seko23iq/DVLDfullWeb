using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RenewLicenseController : ControllerBase
    {
        [HttpGet("IsLicenseExpire/{LicenseID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult IsLicenseExpire(int LicenseID)
        {
            try
            {
                bool isExpired = BussinesLayer.RenewLicenseBussines.IsLicenseExpire(LicenseID);

                return Ok(isExpired);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "An error occurred while checking the license expiration.");
            }
        }


        [HttpPost("AddRenewLicense")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddRenewLicense(DtoLayer.ReNewDrivingApplication.AddRenewLicenseDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data: DTO cannot be null.");

            try
            {
                (int newApplicationID, int newLicenseID) = BussinesLayer.RenewLicenseBussines.AddRenewLicense(dto);

                if (newLicenseID <= 0 || newApplicationID <= 0)
                    return StatusCode(500, "An error occurred while adding the renewed license.");

                return CreatedAtAction(nameof(AddRenewLicense), new
                {
                    applicationID = newApplicationID,
                    licenseID = newLicenseID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while adding the renewed license.",
                    Details = ex.Message
                });
            }
        }
    }
}
