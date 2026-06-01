using BussinesLayer;
using ContactsDataAccessLayer;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DtoLayer.LocalDrivingApplication;
using DtoLayer.LicenseHistory;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalDrivingApplicationController : ControllerBase
    {
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LocalDrivingApplicationDTO>> GetAllRecords()
        {
            List<LocalDrivingApplicationDTO> applications = LocalDrivingApplicationBussines.GetAllRecords();
            if (applications == null || applications.Count == 0)
            {
                return NotFound("No local driving applications found.");
            }
            return Ok(applications);
        }


        [HttpGet("AllLocalLicenseHistoryRecordsByPersonID/{PersonID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LocalLicenseHistoryDTO>> GetAllLicenseHistroyRecords(int PersonID)
        {
            List<LocalLicenseHistoryDTO> applications = LocalDrivingApplicationBussines.GetAllLicenseHistroyRecords(PersonID);
            if (applications == null || applications.Count == 0)
            {
                return NotFound("No local driving applications found.");
            }
            return Ok(applications);
        }

        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LocalDrivingApplicationDTO>> GetInfoBy(LocalDrivingApplicationData.ApplicationFilterType filterType, string value)
        {
            if (string.IsNullOrEmpty(value))
                return BadRequest("FilterType or Value is required.");

            List<LocalDrivingApplicationDTO> list = LocalDrivingApplicationBussines.GetApplications(filterType, value);

            if (list == null || list.Count == 0)
                return NotFound("No matching data found.");

            return Ok(list);
        }

        [HttpGet("LicenseClasses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<string>> GetAllLicenseClasses()
        {
            List<string> LicenseClassesList = LocalDrivingApplicationBussines.GetLicenseClasses();
            if (LicenseClassesList == null || LicenseClassesList.Count == 0)
            {
                return NotFound("No found!");
            }
            return Ok(LicenseClassesList);
        }


        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateLocalApplication([FromForm] AddNewLocalDrivingLicenseDTO dto)
        {
            if (dto == null)
                return BadRequest("Invalid data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int newId = LocalDrivingApplicationBussines.AddNewLocalDrivingRecord(dto);

            if (newId == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { newID = newId });
        }


        [HttpGet("UserID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> GetUserIDByUsername(string Username)
        {
            if (string.IsNullOrEmpty(Username))
                return BadRequest("Username is required.");
            int userID = LocalDrivingApplicationBussines.GetUserIDByUsername(Username);
            if (userID == -1)
                return NotFound("User not found.");
            return Ok(userID);
        }


        [HttpGet("PassedTest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<int> GetPassedTestByLocalApplicationID(int LocalDrivingLicenseApplicationID)
        {
            if (LocalDrivingLicenseApplicationID <= 0)
                return BadRequest("Local Driving License Application ID is required.");
            int PassedTest = LocalDrivingApplicationBussines.HowManyPassTestForThisLDL(LocalDrivingLicenseApplicationID);
            if (PassedTest == -1)
                return NotFound("Erro found.");
            return Ok(PassedTest);
        }

        [HttpGet("PaidFeesLocalApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<float> GetPaidFeesLocalApplication()
        {
            float PaidFees = LocalDrivingApplicationBussines.GetPaidFeesLocalApplication();
            if (PaidFees < 0)
                return NotFound("PaidFees not found.");
            return Ok(PaidFees);
        }

        [HttpGet("ApplicationBasicInfo")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetApplicationBasicInfo(int LocalDrivingLicenseApplicationID)
        {
            if (LocalDrivingLicenseApplicationID <= 0)
                return BadRequest("Local Driving License Application ID is required.");
            ApplicationBasicInfoDTO basicInfo = LocalDrivingApplicationBussines.GetApplicationBasicInfoDTO(LocalDrivingLicenseApplicationID);
            if (basicInfo == null)
                return NotFound("Application not found.");
            return Ok(basicInfo);
        }

        [HttpPost("IssueDrivingLicenseFirstTime")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> IssueDrivingLicenseFirstTime([FromBody] IssueDrivingLicenseFirstTimeDTO dTO)
        {
            if (dTO == null)
                return BadRequest("Invalid data.");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            int newId = LocalDrivingApplicationBussines.issueDrivingLicenseFirst(dTO);
            if (newId == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { newID = newId });
        }

        [HttpGet("GetLicenseIDforApplicationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetLicenseIDforApplicationID(int ApplicationID)
        {
            if (ApplicationID <= 0)
                return BadRequest("Invalid data.");

            int LicenseID = LocalDrivingApplicationBussines.GetLicenseIDforApplicationID(ApplicationID);

           
            return Ok(new { licenseID = LicenseID });
        }

        [HttpGet("GetPersonIDforLocalApplicationID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<int> GetPersonIDforLocalApplicationID(int LocalApplicationID)
        {
            if (LocalApplicationID <= 0)
                return BadRequest("Invalid data.");

            int PersonID = LocalDrivingApplicationBussines.GetPersonIDforLocalApplicationID(LocalApplicationID);

            if (PersonID == -1)
            {
                return StatusCode(500, "A problem occurred while handling your request.");
            }
            return Ok(new { personID = PersonID });
        }

        [HttpGet("license-info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLicenseInfo(int LicenseID)
        {
            if (LicenseID <= 0)
                return BadRequest("License ID required.");
            LicenseInfoDTO licenseInfo = LocalDrivingApplicationBussines.GetDrivingLicenseInfo(LicenseID);
            if (licenseInfo == null)
                return NotFound("License info not found.");
            return Ok(licenseInfo);
        }

        [HttpDelete("{localDrivingLicenseApplicationID}/delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteApplication(int localDrivingLicenseApplicationID)
        {
            if (localDrivingLicenseApplicationID <= 0)
            {
                return BadRequest("Invalid application ID.");
            }

            bool isDeleted =
                LocalDrivingApplicationBussines
                .DeleteLocalDrivingLicenseApplication(localDrivingLicenseApplicationID);

            if (!isDeleted)
            {
                return NotFound("Application not found.");
            }

            return Ok(new
            {
                message = "Application deleted successfully."
            });
        }


        [HttpPut("{localDrivingLicenseApplicationID}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CancelApplication(int localDrivingLicenseApplicationID)
        {
            if (localDrivingLicenseApplicationID <= 0)
            {
                return BadRequest("Invalid application ID.");
            }

            bool isCanceled =
                LocalDrivingApplicationBussines
                .CancelLocalDrivingApplication(localDrivingLicenseApplicationID);

            if (!isCanceled)
            {
                return NotFound("Application not found.");
            }

            return Ok(new
            {
                message = "Application canceled successfully."
            });
        }

        [HttpGet("GetAllLocalLicense/{localDrivingLicenseApplicationID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<LocalLicenseDTO>> GetAllLicenseRecordsData(string localDrivingLicenseApplicationID)
        {
            if(string.IsNullOrWhiteSpace(localDrivingLicenseApplicationID))
            {
                return BadRequest("Invalid Local Driving License Application ID.");
            }

            List<LocalLicenseDTO> applications = LocalDrivingApplicationBussines.GetAllLicenseRecordsData(localDrivingLicenseApplicationID);
            if (applications == null || applications.Count == 0)
            {
                return NotFound("No local driving applications found.");
            }
            return Ok(applications);
        }


        [HttpGet("HasLicenseWithThisClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> HasLicenseWithThisClass(int PersonID, int LicenseClassID)
        {
            if (PersonID <= 0 || LicenseClassID <= 0)
                return BadRequest("Person ID and License Class ID are required.");
            bool hasLicense = LocalDrivingApplicationBussines.HasLicenseWithThisClass(PersonID, LicenseClassID);
            return Ok(hasLicense);
        }
        [HttpGet("HasLicenseIssueWithThisClass")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<bool> HasLicenseIssueWithThisClass(int PersonID, int LicenseClassID)
        {
            if (PersonID <= 0 || LicenseClassID <= 0)
                return BadRequest("Person ID and License Class ID are required.");
            bool hasLicense = LocalDrivingApplicationBussines.HasLicenseIssueWithThisClass(PersonID, LicenseClassID);
            return Ok(hasLicense);
        }
    }
}
