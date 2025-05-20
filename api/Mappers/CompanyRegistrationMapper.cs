using System;
using api.Dto;  // Adjust namespace to where your DTOs are
using api.Models;

namespace api.Mappers
{
    public static class CompanyRegistrationMapper
    {
        public static Company ToCompany(this CompanyRegisterDto dto)
        {
            return new Company
            {
                BusinessName = dto.BusinessName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                TaxId = dto.TaxId,
                LogoUrl = dto.LogoUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static User ToOwnerUser(this CompanyRegisterDto dto, int companyId)
        {
            return new User
            {
                CompanyId = companyId,
                Username = dto.Username,
                Email = dto.UserEmail,
                Fullname = dto.Fullname,
                Role = string.IsNullOrEmpty(dto.Role) ? "Owner" : dto.Role,
                Phone = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
