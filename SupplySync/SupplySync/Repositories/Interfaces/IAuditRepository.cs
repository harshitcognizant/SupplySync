using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IAuditRepository
    {
        Task<Audit> InsertAsync(Audit entity);
        Task<Audit?> GetByIdAsync(int auditId);
        Task<Audit> UpdateAsync(Audit entity);

        // Soft-delete
        Task<bool> SoftDeleteAsync(int auditId);

        // List with filters
        Task<List<Audit>> ListAsync(
            int? complianceOfficerId,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate
        );
    }
}