using BussinesLayer;
using DtoLayer.Person;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebDVLD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        [HttpGet("AllCountries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<string>> GetAllCountry()
        {
            List<string> list = clsCountry.GetAllCountry();

            if (list == null || list.Count == 0)
                return NotFound("No Country found.");

            return Ok(list);
        }

    }
}
