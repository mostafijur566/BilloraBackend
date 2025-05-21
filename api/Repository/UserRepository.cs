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
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ChangeUserStatusAsync(int id, User userModel)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.IsActive = userModel.IsActive;

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task<DeleteUserResultDto> DeleteUserAsync(int id, string currentUserRole)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return new DeleteUserResultDto
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            // Rule: Owner can delete Admins, but Admin cannot delete Owner or Admin
            if (user.Role == "Owner")
            {
                return new DeleteUserResultDto
                {
                    Success = false,
                    ErrorMessage = "Cannot delete Owner user"
                };
            }
            // Rule: Only Owner can delete Admin users
            if (user.Role == "Admin" && currentUserRole != "Owner")
            {
                return new DeleteUserResultDto
                {
                    Success = false,
                    ErrorMessage = "You don't have permission to delete this Admin user"
                };
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return new DeleteUserResultDto
            {
                Success = true,
                DeletedUser = user
            };
        }

        public async Task<List<User>?> GetAllUserAsync(int companyId, int userId)
        {
            return await _context.Users.Where(u => u.CompanyId == companyId && u.Id != userId).ToListAsync();
        }

        public Task<User?> GetUserAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public Task<User?> GetUserBydIdAsync(int id)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> UpdateAsync(int id, User userModel)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            if (existingUser == null)
            {
                return null;
            }

            existingUser.Email = userModel.Email;
            existingUser.Fullname = userModel.Fullname;
            existingUser.Phone = userModel.Phone;

            await _context.SaveChangesAsync();
            return existingUser;
        }
    }
}