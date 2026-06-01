using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        [HttpGet("GetAllDrivers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllDrivers()
        {
            try
            {
                var drivers = BussinesLayer.clsDriver.GetAllDrivers();
                return Ok(drivers);
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return StatusCode(500, "An error occurred while retrieving drivers.");
            }
        }
    }
}
