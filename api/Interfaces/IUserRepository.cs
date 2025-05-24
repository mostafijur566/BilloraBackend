using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helper;
using api.Models;

namespace api.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(int id);
        Task<User?> UpdateAsync(int id, User userModel);
        Task<List<User>?> GetAllUserAsync(int companyId, int userId, UserQueryObject query);
        Task<User?> GetUserBydIdAsync(int id);
        Task<User?> ChangeUserStatusAsync(int id, User userModel);
        Task<DeleteUserResultDto> DeleteUserAsync(int id, string currentUserRole);
    }
}