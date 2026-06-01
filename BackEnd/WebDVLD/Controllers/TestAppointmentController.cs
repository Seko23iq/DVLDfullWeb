using BussinesLayer;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using DtoLayer.Application;
using DtoLayer.Test;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestAppointmentController : ControllerBase
    {

        [HttpGet("ByApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllTestAppointments(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            var appointments = TestsBussines.GetAllTestAppointmentRecords(LocalDrivingLicenseApplicationID, TestTypeID);
            if (appointments == null || appointments.Count == 0)
                return NotFound("No test appointments found.");
            return Ok(appointments);
        }

        [HttpGet("ApplicationInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetApplicationInformation(int LocalDrivingLicenseApplicationID)
        {
            if(LocalDrivingLicenseApplicationID <= 0)
            {
                return NotFound("Invalid Local Driving License Application ID");
            }

            ApplicantInformationTestDTO dTO = TestsBussines.GetApplicantInformation(LocalDrivingLicenseApplicationID);

            if(dTO == null)
            {
                return NotFound("There is no Applicant With this local Driving License Applicaiton ID");
            }

            return Ok(dTO);
        }

       
        [HttpGet("TakerTestInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTakerTestInformation(int TestAppointmentID)
        {
            if (TestAppointmentID <= 0)
            {
                return NotFound("Invalid Test Appointment ID");
            }

            TakerTestInfoDTO dTO = TestsBussines.GetTakerTestInfoInformation(TestAppointmentID);

            if (dTO == null)
            {
                return NotFound("There is no Applicant With this Test Appointment ID");
            }

            return Ok(dTO);
        }


        [HttpPost("AddNewTestAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult AddNewTestAppointmentRecord([FromBody] AddApplicationDTO dTO)
        {
            if (dTO == null)
                return BadRequest("Invalid data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int newId = TestsBussines.AddNewTestAppointmentRecord(dTO);

            if (newId == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { newID = newId });
        }

        [HttpPost("TakeTest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult TakeTest([FromBody] TakeTestDTO dTO)
        {
            if(dTO == null)
            {
                return BadRequest("Invalid data.");
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int newId = TestsBussines.TakeTest(dTO);

            if(newId == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { newID = newId });
        }


        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult EditTest([FromBody] UpdateTestAppointmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool isUpdated = TestsBussines.EditTestAppointment(dto);

            if (!isUpdated)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }

            return Ok("Updated successfully.");
        }


        [HttpPost("HasActiveTestAppointment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult HasActiveTestAppointment([FromBody] LocalAndTestTypeidDTO dTO)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool hasAppointment = TestsBussines.HasTestAppointment(dTO);

            return Ok(hasAppointment);

        }

    }
}
