using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helper;
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

        public async Task<List<User>?> GetAllUserAsync(int companyId, int userId, UserQueryObject query)
        {
            var users = _context.Users
                .Where(u => u.CompanyId == companyId &&
                    u.Id != userId).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Username))
                users = users.Where(u => u.Username.Contains(query.Username));

            if (!string.IsNullOrWhiteSpace(query.Email))
                users = users.Where(u => u.Email.Contains(query.Email));

            if (!string.IsNullOrWhiteSpace(query.Fullname))
                users = users.Where(u => u.Fullname.Contains(query.Fullname));

            if (!string.IsNullOrWhiteSpace(query.Role))
                users = users.Where(u => u.Role.ToLower() == query.Role.ToLower());

            if (query.IsActive.HasValue)
                users = users.Where(u => u.IsActive == query.IsActive.Value);

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "username":
                        users = query.IsDescending ? users.OrderByDescending(u => u.Username) : users.OrderBy(u => u.Username);
                        break;
                    case "email":
                        users = query.IsDescending ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email);
                        break;
                    case "fullname":
                        users = query.IsDescending ? users.OrderByDescending(u => u.Fullname) : users.OrderBy(u => u.Fullname);
                        break;
                    case "createdat":
                        users = query.IsDescending ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt);
                        break;
                    default:
                        users = users.OrderBy(u => u.Username);
                        break;
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await users.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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