using SupplySync.DTOs.Report;
using SupplySync.Models; // Ensure this is included for the Report entity

namespace SupplySync.Repositories.Interfaces
{
    public interface IReportRepository
    {
        // Analytics
        Task<List<VendorPerformanceDto>> GetVendorPerformanceAsync(DateTime? fromUtc, DateTime? toUtc, int topN = 50);
        Task<List<DeliveryDelayDto>> GetDeliveryDelaysAsync(DateTime? fromUtc, DateTime? toUtc, int max = 100);
        Task<ProcurementSpendingDto> GetTotalProcurementSpendingAsync(DateTime fromUtc, DateTime toUtc);
        Task<List<InventoryLevelDto>> GetInventoryLevelsAsync();
        Task<List<InvoiceTurnaroundDto>> GetInvoiceApprovalTurnaroundAsync(DateTime? fromUtc, DateTime? toUtc);

        // CRUD for the 'Report' meta-entity (The missing pieces)
        Task<Report> InsertAsync(Report report);
        Task<Report?> GetByIdAsync(int id);
        Task<Report> UpdateAsync(Report report);
        Task<bool> SoftDeleteAsync(int id);
        Task<List<Report>> ListAsync(string? scope, DateTime? fromDate, DateTime? toDate);
    }
}