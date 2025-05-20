using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
using api.Models;

namespace api.Mappers
{
    public static class AccountMapper
    {
        public static User ToUserCreate(this RegistrationDto dto, string hashedPassword)
        {
            return new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Fullname = dto.FullName,
                Phone = dto.Phone,
                Role = dto.Role,
                CompanyId = dto.CompanyId,
                PasswordHash = hashedPassword
            };
        }

        public static User ToUserDto(this User user)
        {
            return new User
            {
                Username = user.Username,
                Email = user.Email,
                Fullname = user.Fullname,
                Phone = user.Phone,
                Role = user.Role,
                CompanyId = user.CompanyId,
            };
        }
    }
}