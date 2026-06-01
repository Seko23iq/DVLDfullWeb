using BussinesLayer;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DtoLayer.ApplicationTypes;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationTypesController : ControllerBase
    {
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<ApplicationTypesDTO>> GetAllRecords()
        {
            List<ApplicationTypesDTO> manageApplicationTypesDTOs = clsApplicationTypes.GetAllRecords();

            if (manageApplicationTypesDTOs == null || manageApplicationTypesDTOs.Count == 0)
            {
                return NotFound("There is no records");
            }

            return Ok(manageApplicationTypesDTOs);
        }

        [HttpGet("ApplictionTypeInfo/{ApplictionTypeID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ApplicationTypesDTO> GetPeopleInfo(int ApplictionTypeID)
        {
            if (ApplictionTypeID <= 0)
            {
                return BadRequest("Invalid Person ID.");
            }

            try
            {
                ApplicationTypesDTO ApplicationType = clsApplicationTypes.GetApplicationTypeInfo(ApplictionTypeID);

                if (ApplicationType == null)
                {
                    return NotFound($"Application Type with ID {ApplictionTypeID} not found.");
                }

                return Ok(ApplicationType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }


        [HttpPut("Update/{ApplicationTypeID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult UpdateApplicationType(int ApplicationTypeID,[FromBody] ApplicationTypesDTO dTO)
        {
            if (dTO == null)
                return BadRequest("Invalid data.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            bool isUpdated = clsApplicationTypes.UpdateApplicationType(dTO);
            if (!isUpdated)
            {
                return BadRequest("Could not update the application type. Please check the data and try again.");
            }
            return Ok("Application type updated successfully.");
        }
    }
}
