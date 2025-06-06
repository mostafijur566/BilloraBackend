using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.User;
using api.Models;

namespace api.Mappers
{
    public static class UserMapper
    {
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
                IsActive = user.IsActive
            };
        }

        public static User ToUserFormUpdate(this UserUpdateDto userDto)
        {
            return new User
            {
                Email = userDto.Email,
                Fullname = userDto.Fullname,
                Phone = userDto.Phone,
            };
        }
        
         public static User ToUserChangeStatus(this ChangeUserStatusDto user)
        {
            return new User
            {
                IsActive = user.IsActive
            };
        }
    }
}