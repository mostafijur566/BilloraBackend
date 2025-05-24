using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IAccountRepository
    {
        Task<User?> GetUserByUsernameOrEmailAsync(string usernameOrEmail);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task<Company?> CompanyExistsAsync(int id);
        Task<bool> ForgotPasswordAsync(string email, string otp, DateTime expiresAt);
        Task<User?> ResetPasswordAsync(User user, string password);
    }
}