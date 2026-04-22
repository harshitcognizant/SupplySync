using SupplySync.Constants.Enums;
using SupplySync.Models;
 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Repositories.Interfaces
{
    public interface IVendorApplicationRepository
    {
        Task<VendorApplication> CreateAsync(VendorApplication application);
        Task<VendorApplication?> GetByIdAsync(int id);
        Task UpdateAsync(VendorApplication application);
        Task<List<VendorApplication>> ListByStatusAsync(VendorStatus status);
    }
}