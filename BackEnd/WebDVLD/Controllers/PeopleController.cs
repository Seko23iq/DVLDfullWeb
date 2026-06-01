using BussinesLayer;
using DataLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DtoLayer.Person;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly string UploadsFolder = @"D:\imagesDVLDproject\people";
        private const string ImageUrlPrefix = "/people-images/";
        private async Task<string> SaveImageAsync(IFormFile image)
        {
            Directory.CreateDirectory(UploadsFolder);
            string uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            string filePath = Path.Combine(UploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"{ImageUrlPrefix}{uniqueFileName}";
        }


        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<PeopleDTO>> GetAllPeople()
        {
            List<PeopleDTO> list = clsPerson.GetAllPeopleDTO();

            if (list == null || list.Count == 0)
                return NotFound("No people found.");

            return Ok(list);
        }

        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<IEnumerable<PeopleDTO>> GetInfoBy(string filterType, string value)
        {
            if (string.IsNullOrEmpty(filterType) || string.IsNullOrEmpty(value))
                return BadRequest("FilterType or Value is required.");

            List<PeopleDTO> list = clsPerson.GetInfoBy(filterType, value);

            if (list == null || list.Count == 0)
                return NotFound("No matching data found.");

            return Ok(list);
        }

        [HttpDelete("{personID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePerson(int personID)
        {
            if(personID <= 0)
                return BadRequest("Invalid Person ID.");

            if (!clsPerson.IsPersonExsitsBy("PersonID", personID.ToString()))
                return NotFound($"Person with ID {personID} not found.");

            if (clsPerson.DeletePerson(personID))
                return Ok(new { message = $"Person with ID {personID} deleted." });
           
            return BadRequest("Could not delete the person. They might be linked to other records.");
        }

        [HttpGet("PeopleInfo/{personID}", Name = "GetPersonById")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<PeopleDTO> GetPeopleInfo(int personID)
        {
            if (personID <= 0)
            {
                return BadRequest("Invalid Person ID.");
            }

            try
            {
                PeopleDTO person = clsPerson.GetPersonInfo(personID);

                if (person == null)
                {
                    return NotFound($"Person with ID {personID} not found.");
                }

                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }
        
        
        [HttpGet("PeopleInfoByNationalNo/{NationalNo}", Name = "GetPersonByNationalNo")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<PeopleDTO> GetPeopleInfoByNationalNo(string NationalNo)
        {
            if (string.IsNullOrWhiteSpace(NationalNo))
            {
                return BadRequest("Invalid Person ID.");
            }

            try
            {
                PeopleDTO person = clsPerson.GetPersonInfoByNationalNo(NationalNo);

                if (person == null)
                {
                    return NotFound($"Person with NationalNo {NationalNo} not found.");
                }

                return Ok(person);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error occurred while processing your request.");
            }
        }

        [HttpPost("AddPerson")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CreatePerson([FromForm] AddPersonDTO dto, IFormFile Image)
        {
            if(dto == null)
            {
                return BadRequest("Invalid Data!");
            }

            if(Image == null || Image.Length <= 0)
            {
                return BadRequest("Invalid Data!");
            }

            dto.ImagePath = await SaveImageAsync(Image);

            int newID = clsPerson._AddNewPerson(dto);

            if (newID == -1)
                return StatusCode(500, new { message = "Failed to add person." });

            //return Ok(new { personId = newID, imagePath = dto.ImagePath });
            return CreatedAtRoute("GetPersonById", new { personID = newID }, dto);
        }

        [HttpPut("UpdatePerson/{id}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePerson(int id, [FromForm] UpdatePersonDTO dto, IFormFile? Image)
        {
            if (dto == null || id <= 0 )
                return BadRequest("Invalid data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!clsPerson.IsPersonExsitsBy("PersonID", id.ToString()))
                return NotFound($"Person with ID {id} not found.");


            if (Image != null && Image.Length > 0)
                dto.ImagePath = await SaveImageAsync(Image);

            dto.PersonID = id;

            bool isUpdated = clsPerson.UpdatePersonInfo(dto);

            if (!isUpdated)
                return StatusCode(500, new { message = "A problem occurred while updating person." });

            return Ok(new
            {
                message = "Person updated successfully",
                personId = id,
                imagePath = dto.ImagePath
            });

            //dto.ImagePath = $"/people-images/{uniqueFileName}"
        }
    }
}
