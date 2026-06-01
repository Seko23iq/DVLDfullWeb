using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DamageApplicationController : ControllerBase
    {
        [HttpPost("AddDamageLicense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddDamageLicense(DtoLayer.DamageApplication.DamageDTO dto)
        {
            try
            {
                var (applicationID, licenseID) = BussinesLayer.clsDamage.AddDamageLicense(dto);

                if (licenseID == -1)
                    return BadRequest("Failed to create license.");

                return Ok(new
                {
                    applicationID = applicationID,  // ← result.applicationID
                    replacedLicenseID = licenseID       // ← result.replacedLicenseID
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }
    }
}
