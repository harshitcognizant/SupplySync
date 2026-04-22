using AutoMapper;
using SupplySync.DTOs.Admin;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplySync.Services
{
    public class VendorCategoryService : IVendorCategoryService
    {
        private readonly IVendorCategoryRepository _repo;
        private readonly IMapper _mapper;

        public VendorCategoryService(
            IVendorCategoryRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<VendorCategoryResponseDto> CreateAsync(
            CreateVendorCategoryRequestDto dto)
        {
            var model = new VendorCategoryConfig
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            var created = await _repo.CreateAsync(model);
            return _mapper.Map<VendorCategoryResponseDto>(created);
        }

        public async Task<List<VendorCategoryResponseDto>> ListAsync()
        {
            var items = await _repo.ListAsync();
            return items
                .Select(item => _mapper.Map<VendorCategoryResponseDto>(item))
                .ToList();
        }

        public async Task<VendorCategoryResponseDto?> UpdateAsync(
            int id,
            CreateVendorCategoryRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            existing.Name = dto.Name;
            existing.Description = dto.Description;
            existing.IsActive = dto.IsActive;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<VendorCategoryResponseDto>(updated);
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}
