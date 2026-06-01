using BussinesLayer;
using DtoLayer.InternationalAppliaction;
using DtoLayer.LicenseHistory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternationalApplicationController : ControllerBase
    {
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllInternationalLicenseRecords()
        {
            try
            {
                var dataTable = InternationalLicenseBussines.GetAllAllInternationalLicenseRecords();
                return Ok(dataTable);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

        [HttpPost("AddNewInternationalLicense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddNewInternationalLicense(DtoLayer.InternationalAppliaction.AddInternationalApplicationDTO dTO)
        {
            try
            {
                NewInternationalLicenseResultDTO result = BussinesLayer.InternationalLicenseBussines.AddNewInternationalLicense(dTO);
                if (result != null)
                {
                    return Ok(new { Message = "International license application added successfully.", result });
                }
                else
                {
                    return BadRequest(new { Message = "Failed to add international license application." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }

        [HttpGet("info/{InternationApplicationID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetInternationApplicationInfo(int InternationApplicationID)
        {
            if(InternationApplicationID <= 0)
            {
                return BadRequest("Invalid Internation Application ID");
            }
            try
            {
                var dto = InternationalLicenseBussines.GetInternationApplicationInfo(InternationApplicationID);

                if (dto == null)
                {
                    return NotFound("Info not found");
                }

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing the request.",
                    Error = ex.Message
                });
            }
        }


        [HttpGet("active-license/{DriverID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetActiveInternationalLicenseIDByDriverID(int DriverID)
        {
            if(DriverID <= 0)
            {
                return BadRequest("Invalid Driver ID.");
            }

            try
            {
                int licenseID = InternationalLicenseBussines.GetActiveInternationalLicenseIDByDriverID(DriverID);
                if (licenseID > 0)
                {
                    return Ok(new { ActiveInternationalLicenseID = licenseID });
                }
                else
                {
                    return NotFound(new { Message = "No active international license found for the specified driver." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while processing the request.", Error = ex.Message });
            }
        }


        [HttpGet("GetPersonIDByLicenseID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetPersonIDByLicenseID(int LicenseID)
        {
            if (LicenseID <= 0)
                return BadRequest("Invalid data.");

            int PersonID = InternationalLicenseBussines.GetPersonIDByLicenseID(LicenseID);

            if (PersonID == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { personID = PersonID });
        }

        [HttpGet("HasInternationalLicense")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<bool> HasInternationalLicense(int DriverID)
        {
            if (DriverID <= 0)
                return BadRequest("Invalid data.");

            bool HasInternationalLicense = InternationalLicenseBussines.HasInternationalLicense(DriverID);

           
            return Ok(new { HasInternationalLicense = HasInternationalLicense });
        }


        [HttpGet("GetAllInternationalLicenseHistroyRecordsByPersonID/{PersonID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LocalLicenseHistoryDTO>> GetAllInternationalLicenseHistroyRecordsByPersonID(int PersonID)
        {
            List<InternationalLicenseHistoryDTO> applications = InternationalLicenseBussines.GetAllInternationalLicenseHistroyRecordsByPersonID(PersonID);
            if (applications == null || applications.Count == 0)
            {
                return NotFound("No local driving applications found.");
            }
            return Ok(applications);
        }

    }
}

