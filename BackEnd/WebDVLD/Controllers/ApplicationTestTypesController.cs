using BussinesLayer;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DtoLayer.TestTypes;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationTestTypesController : ControllerBase
    {
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<TestTypesDTO>> GetAllRecords()
        {
            List<TestTypesDTO> testTypes = clsTestTypes.GetAllRecords();

            if(testTypes == null || testTypes.Count == 0)
            {
                return NotFound("No test types found.");
            }
            return Ok(testTypes);
        }


        [HttpGet("ApplictionTestTypeInfo/{TestTypeID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<TestTypesDTO> GetPeopleInfo(int TestTypeID)
        {
            if (TestTypeID <= 0)
            {
                return BadRequest("Invalid TestType ID.");
            }

            try
            {
                TestTypesDTO ApplicationType = clsTestTypes.GetApplicationTypeInfo(TestTypeID);

                if (ApplicationType == null)
                {
                    return NotFound($"Application Test Type with ID {TestTypeID} not found.");
                }

                return Ok(ApplicationType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }


        [HttpPut("Update/{TestTypeID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateApplicationType(int TestTypeID, [FromBody] TestTypesDTO dTO)
        {
            if (dTO == null)
                return BadRequest("Invalid data.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            dTO.ID = TestTypeID;
            bool isUpdated = clsTestTypes.UpdateApplicationType(dTO);
            if (!isUpdated)
            {
                return BadRequest("Could not update the application test type. Please check the data and try again.");
            }
            return Ok("Application type updated successfully.");
        }
    }
}
