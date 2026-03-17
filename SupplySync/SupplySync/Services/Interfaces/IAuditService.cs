using SupplySync.DTOs.Audit;
using SupplySync.Models;

namespace SupplySync.Services.Interfaces
{
    public interface IAuditService
    {
        Task<int> CreateAuditAsync(CreateAuditDto dto);
    }
}
