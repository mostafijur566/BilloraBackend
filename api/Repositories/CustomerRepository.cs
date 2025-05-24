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

        public async Task<Customer?> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return null;
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task<List<Customer>?> GetAllCustomerAsync(int companyId, CustomerQueryObject query)
        {
            var customers = _context.Customers
                .Include(c => c.User)
                .Where(c => c.User != null && c.User.CompanyId == companyId)
                .AsQueryable();

            // Filter by name if provided
            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                customers = customers.Where(c => c.Name.Contains(query.Name));
            }

            // Filter by email if provided
            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                customers = customers.Where(c => c.Email != null && c.Email.Contains(query.Email));
            }

            // Filter by phone if provided
            if (!string.IsNullOrWhiteSpace(query.Phone))
            {
                customers = customers.Where(c => c.Phone.Contains(query.Phone));
            }

            // Filter by user id if provided
            if (query.UserId.HasValue)
            {
                customers = customers.Where(c => c.UserId == query.UserId);
            }

            // Filter by SortBy if provided
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "name":
                        customers = query.IsDescending ? customers.OrderByDescending(c => c.Name) : customers.OrderBy(c => c.Name);
                        break;
                    case "email":
                        customers = query.IsDescending ? customers.OrderByDescending(c => c.Email) : customers.OrderBy(c => c.Email);
                        break;
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await customers.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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