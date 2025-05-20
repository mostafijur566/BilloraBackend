using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interface;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Company> AddCompanyAsync(Company company)
        {
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> CompanyExistsAsync(string businessName, string email)
        {
            return await _context.Companies
            .AnyAsync(c => c.BusinessName == businessName || c.Email == email);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users
            .AnyAsync(u => u.Username == username || u.Email == email);
        }
    }
}