using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
using api.Dto.Account;
using api.Models;

namespace api.Mappers
{
    public static class AccountMapper
    {
        public static User ToUserCreate(this RegistrationDto dto, string hashedPassword, int companyId)
        {
            return new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Fullname = dto.FullName,
                Phone = dto.Phone,
                Role = dto.Role,
                CompanyId = companyId,
                PasswordHash = hashedPassword
            };
        }

        public static UserDto ToUserDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
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