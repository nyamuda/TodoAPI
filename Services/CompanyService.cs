﻿using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos.Company;
using TodoAPI.Models;

namespace TodoAPI.Services
{
    public class CompanyService
    {
        private ApplicationDbContext _context;


        public CompanyService(ApplicationDbContext context)
        {
            _context = context;
        }

        //Get company by ID
        public async Task<Company> GetCompanyByID(int id)
        {
            Company? company = await _context.Companies.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (company is null) throw new KeyNotFoundException($"Company with ID {id} does not exist.");

            return company;
        }
       
       //Get all companies
        public async Task<List<Company>> GetCompanies()
        {
            return await _context.Companies.ToListAsync();
        }

        //Add company
        public async Task<Company> AddCompany(CompanyDto companyDto)
        {
            //company name is unique
            //check if there isn't a company that already has the given name
            var companyName = companyDto.Name.ToLower();
            Company? companyExists = await _context.Companies.FirstOrDefaultAsync(x => x.Name.Equals(companyName));
            if (companyExists is not null)
                throw new InvalidOperationException($"Company with name {companyDto.Name} already exists.");

            var company = new Company()
            {
                Name = companyName,
                Address = companyDto.Address,
                Email=companyDto.Email,
                Phone = companyDto.Phone,
                YearFounded = companyDto.YearFounded

            };
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        //Update company
        public async Task UpdateCompany(int id, CompanyDto companyDto)
        {
            //check to see if company with given ID exists
            Company company = await GetCompanyByID(id);

            //company name is unique
            //check if there isn't a company that already has the given name (aside from the one being updated)
            bool companyWithNewNameExists = await _context.Companies
                 .AnyAsync(x => x.Name.Equals(company.Name) && !x.Id.Equals(company.Id));

            if(companyWithNewNameExists)
                throw new InvalidOperationException($"Company with name {companyDto.Name} already exists.");

            //update the company details
            company.Name = companyDto.Name;
            company.Address=companyDto.Address;
            company.Email = companyDto.Email;
            company.Phone = companyDto.Phone;
            company.YearFounded = companyDto.YearFounded;

            await _context.SaveChangesAsync();

        }

        //Delete company by ID
        public async Task DeleteCompany(int id)
        {
            //check to see if company with given ID exists
            Company company = await GetCompanyByID(id);

            _context.Companies.Remove(company);

            await _context.SaveChangesAsync();

        }

        //Get company facts
        public async Task<CompanyFactsDto> GetCompanyFacts(int id)
        {
            //first get the company
            var company=await GetCompanyByID(id);

            //calculate the years in service based on the year founded
            int totalYearsInService = DateTime.Now.Year - company.YearFounded.Year;

            //total completed bookings made by the clients of the company
            int totalCompletedBookings = await _context.Bookings
                .Where(x => x.Status.Name.Equals("completed"))
                .CountAsync();

            //total happy users i.e
            //users who gave a rating of 4 or 5 for their completed bookings
            int totalHappyUsers = await _context.Bookings
                .Where(x => x.Feedback != null && x.Feedback.Rating > 4)
                .CountAsync();

            //Calculate overall rating from all bookings
            //first, get the total number of bookings that have received feedback
            int bookingsWithFeedback = await _context.Bookings.Where(x => x.Feedback != null).CountAsync();
            //second, get total ratings from all booking that have received feedback
            double totalRating = await _context.Bookings
                .Where(x => x.Feedback != null)
                .SumAsync(x => x.Feedback!.Rating);
            //finally, calculate the overall rating
            //round to 2 d.p
            double overallRating = Math.Round(totalRating / bookingsWithFeedback, 2, MidpointRounding.AwayFromZero);

            var companyFacts = new CompanyFactsDto()
            {
                Company = company,
                TotalYearsInService = totalYearsInService,
                TotalCompletedBookings = totalCompletedBookings,
                OverallRating = overallRating,
                TotalHappyCustomers = totalHappyUsers
            };

            return companyFacts;   
        }
    }
}
