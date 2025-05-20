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
    }
}