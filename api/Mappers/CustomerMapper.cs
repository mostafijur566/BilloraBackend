using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Customer;
using api.Models;

namespace api.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customerModel)
        {
            return new CustomerDto
            {
                Id = customerModel.Id,
                UserId = customerModel.UserId,
                Name = customerModel.Name,
                Email = customerModel.Email,
                Phone = customerModel.Phone,
                Address = customerModel.Address,
                ContactPerson = customerModel.ContactPerson
            };
        }
        public static Customer ToCustomerFromCreate(this CreateCustomerDto customerDto, int userId)
        {
            return new Customer
            {
                UserId = userId,
                Name = customerDto.Name,
                Email = customerDto.Email,
                Phone = customerDto.Phone,
                Address = customerDto.Address,
                ContactPerson = customerDto.ContactPerson
            };
        }
    }
}