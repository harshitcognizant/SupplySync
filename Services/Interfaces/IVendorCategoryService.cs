using SupplySync.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Services.Interfaces
{
    public interface IVendorCategoryService
    {
        Task<VendorCategoryResponseDto> CreateAsync(CreateVendorCategoryRequestDto dto);
        Task<List<VendorCategoryResponseDto>> ListAsync();
        Task<VendorCategoryResponseDto?> UpdateAsync(int id, CreateVendorCategoryRequestDto dto);
        Task DeleteAsync(int id);
    }
}