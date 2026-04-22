using SupplySync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Repositories.Interfaces
{
    public interface IVendorCategoryRepository
    {
        Task<VendorCategoryConfig> CreateAsync(VendorCategoryConfig model);
        Task<VendorCategoryConfig?> GetByIdAsync(int id);
        Task<List<VendorCategoryConfig>> ListAsync();
        Task<VendorCategoryConfig> UpdateAsync(VendorCategoryConfig model);
        Task DeleteAsync(int id);
    }
}