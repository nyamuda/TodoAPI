using Microsoft.AspNetCore.Mvc;
using TodoAPI.Models;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private CompanyService _companyService;
        private ErrorMessageService _errorMessageService;


        public CompaniesController(CompanyService companyService, ErrorMessageService errorMessageService)
        {
            _companyService = companyService;
            _errorMessageService = errorMessageService;
        }



        // GET: api/<CompaniesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<Company> companies = await _companyService.GetCompanies();

                return Ok(companies);
            }

            catch(Exception ex)
            {
                var response = new
                {
                    Message = _errorMessageService.UnexpectedErrorMessage(),
                    Details = ex.Message
                };

                return StatusCode(500,response);
            }
        }

        // GET api/<CompaniesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
           
        }

        // POST api/<CompaniesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<CompaniesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CompaniesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
