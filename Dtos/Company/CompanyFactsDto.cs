using System.ComponentModel.DataAnnotations;
using TodoAPI.Models;
namespace TodoAPI.Dtos.Company
{
    public class CompanyFactsDto
    {
        public Models.Company Company { get; set; } = default!;
       
        public int TotalYearsInService { get; set; }
       
        public int TotalBookings { get; set; }

       
        public double OverallRating { get; set; }

        
        public int TotalHappyCustomers { get; set; }
    }
}
