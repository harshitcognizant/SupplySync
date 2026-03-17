using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IComplianceRecordRepository
    {
        Task<ComplianceRecord> InsertAsync(ComplianceRecord entity);
        Task<ComplianceRecord?> GetByIdAsync(int id);
        Task<ComplianceRecord> UpdateAsync(ComplianceRecord entity);

        // Soft-delete handler
        Task<bool> SoftDeleteAsync(int id);

        // List with Simple Filters + Soft Delete
        Task<List<ComplianceRecord>> ListAsync(
            int? contractId,
            string? type,
            string? result,
            DateOnly? fromDate,
            DateOnly? toDate
        );
    }
}