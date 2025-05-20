using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IUserRepository
    {
        Task<User?> GetUserAsync(int id);
        Task<User?> UpdateAsync(int id, User userModel);
        Task<List<User>?> GetAllUserAsync(int companyId, int userId);
    }
}