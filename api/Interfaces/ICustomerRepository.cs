using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helper;
using api.Models;

namespace api.Interface
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateCustomerAsync(Customer customerModel);
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer?> GetCustomerByPhoneAsync(int companyId, string phone);
        Task<List<Customer>?> GetAllCustomerAsync(int companyId, CustomerQueryObject query);
        Task<Customer?> UpdateCustomerAsync(int id, Customer customerModel);
        Task<Customer?> DeleteCustomerAsync(int id);
    }
}