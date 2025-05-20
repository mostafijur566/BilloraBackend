using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface ICompanyRepository
    {
        Task<bool> CompanyExistsAsync(string businessName, string email);
        Task<bool> UserExistsAsync(string username, string email);
        Task<Company> AddCompanyAsync(Company company);
        Task<User> AddUserAsync(User user);
    }
}