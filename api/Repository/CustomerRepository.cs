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
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        public CustomerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Customer> CreateCustomerAsync(Customer customerModel)
        {
            await _context.Customers.AddAsync(customerModel);
            await _context.SaveChangesAsync();
            return customerModel;
        }

        public async Task<List<Customer>?> GetAllCustomerAsync(int companyId)
        {
            return await _context.Customers
                .Include(c => c.User)
                .Where(c => c.User != null && c.User.CompanyId == companyId)
                .ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return null;
            }

            return customer;
        }

        public async Task<Customer?> GetCustomerByPhoneAsync(int companyId, string phone)
        {
            return await _context.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c =>
                    c.Phone.Trim() == phone.Trim() &&
                    c.User != null && c.User.CompanyId == companyId);
        }

        public async Task<Customer?> UpdateCustomerAsync(int id, Customer customerModel)
        {
            var existingCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (existingCustomer == null)
            {
                return null;
            }

            existingCustomer.Name = customerModel.Name;
            existingCustomer.Email = customerModel.Email;
            existingCustomer.Phone = customerModel.Phone;
            existingCustomer.Address = customerModel.Address;
            existingCustomer.ContactPerson = customerModel.ContactPerson;

            existingCustomer.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return existingCustomer;
        }
    }
}