using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos.Company;
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
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var company = await _companyService.GetCompanyByID(id);

                return Ok(company);
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });

            }
            catch (Exception ex)
            {
                var response = new
                {
                    Message = _errorMessageService.UnexpectedErrorMessage(),
                    Details = ex.Message
                };

                return StatusCode(500, response);

            }

        }

        // POST api/<CompaniesController>
        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Post(CompanyDto companyDto)
        {
            try
            {
                var company = _companyService.AddCompany(companyDto);

                return CreatedAtAction(nameof(Get), new { id = company.Id }, company);

            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Message = _errorMessageService.UnexpectedErrorMessage(),
                    Details = ex.Message
                };

                return StatusCode(500, response);
            }

        }

        // PUT api/<CompaniesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, CompanyDto companyDto)
        {
            try
            {
                await _companyService.UpdateCompany(id, companyDto);

                return NoContent();

            }

            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                var response = new
                {
                    Message = _errorMessageService.UnexpectedErrorMessage(),
                    Details = ex.Message
                };

                return StatusCode(500, response);
            }
        }

        // DELETE api/<CompaniesController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _companyService.DeleteCompany(id);

                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }

            catch(Exception ex)
            {
                var response = new
                {
                    Message = _errorMessageService.UnexpectedErrorMessage(),
                    Details = ex.Message
                };

                return StatusCode(500, response);
            }
        }
    }
}
