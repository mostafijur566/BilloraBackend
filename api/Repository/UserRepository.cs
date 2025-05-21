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

            // ✅ Log before update
            Console.WriteLine($"[BEFORE] DB IsActive: {existingUser.IsActive}");

            existingUser.IsActive = userModel.IsActive;

            // ✅ Log after update
            Console.WriteLine($"[AFTER] Updated IsActive: {existingUser.IsActive}");

            await _context.SaveChangesAsync();
            return existingUser;
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