using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto;
using api.Mappers;
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
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> CompanyExistsAsync(string businessName, string email)
        {
            return await _context.Companies
            .AnyAsync(c => c.BusinessName == businessName || c.Email == email);
        }

        public async Task<Company?> DeleteAsync(int id)
        {
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
            {
                return null;
            }

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<string?> SaveLogoAsync(IFormFile? logo)
        {
            if (logo == null)
                return null;

            var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logos");
            if (!Directory.Exists(uploadsDir))
                Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(logo.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logo.CopyToAsync(stream);
            }

            return $"/logos/{fileName}";
        }

        public async Task<Company?> UpdateCompanyAsync(int id, CompanyUpdateDto dto)
        {
            
            var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == id);
            if (company == null)
                return null;

            // Update simple properties
            company.BusinessName = dto.BusinessName;
            company.ContactName = dto.ContactName;
            company.Email = dto.Email;
            company.Phone = dto.Phone;
            company.Address = dto.Address;
            company.UpdatedAt = DateTime.UtcNow;
            
            // Handle logo update
            if (dto.Logo != null)
            {
                // Delete previous logo file if exists
                if (!string.IsNullOrEmpty(company.LogoUrl))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", company.LogoUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                // Save new logo using helper
                company.LogoUrl = await SaveLogoAsync(dto.Logo) ?? "";
            }
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            return await _context.Users
            .AnyAsync(u => u.Username == username || u.Email == email);
        }
    }
}