using TodoAPI.Data;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class CompanyService
    {
        private ApplicationDbContext _context;


        public CompanyService(ApplicationDbContext context)
        {
            _context=context;
        }

        //Get company by ID
        public async Task<Company> GetCompanyByID(int id)
        {



        }
    }
}
