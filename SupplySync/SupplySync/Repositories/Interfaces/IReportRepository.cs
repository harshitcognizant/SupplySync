using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<Report> InsertAsync(Report entity);
        Task<Report?> GetByIdAsync(int id);
        Task<Report> UpdateAsync(Report entity);

        // Soft-delete
        Task<bool> SoftDeleteAsync(int id);

        // List with filters
        Task<List<Report>> ListAsync(
            string? scope,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}