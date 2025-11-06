using AutoMapper;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnomateApp.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDto>> GetAllAsync()
        {
            var customers = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CustomerDto>>(customers);
        }

        public async Task<CustomerDto?> GetByIdAsync(int id)
        {
            var customer = await _repository.GetByIdAsync(id);
            return _mapper.Map<CustomerDto?>(customer);
        }

        public async Task<CustomerDto> CreateAsync(CreateCustomerDto dto)
        {
            var entity = _mapper.Map<Customer>(dto);
            await _repository.AddAsync(entity);
            return _mapper.Map<CustomerDto>(entity);
        }

        public async Task<bool> UpdateAsync(UpdateCustomerDto dto)
        {
            var existing = await _repository.GetByIdAsync(dto.CustomerId);
            if (existing == null)
                return false;

            _mapper.Map(dto, existing);
            _repository.Update(existing);  // ✅ No await — just updates EF tracking

            await _repository.SaveChangesAsync(); // ✅ async DB commit
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                return false;

            _repository.Delete(entity); // ✅ No await

            await _repository.SaveChangesAsync(); // ✅ async DB commit
            return true;
        }
    }
}
