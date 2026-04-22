using SupplySync.DTOs.Report;
using SupplySync.Models;

namespace SupplySync.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<VendorPerformanceDto>> VendorPerformanceAsync(DateTime? fromUtc, DateTime? toUtc, int topN = 50);
        Task<List<DeliveryDelayDto>> DeliveryDelaysAsync(DateTime? fromUtc, DateTime? toUtc, int max = 100);
        Task<ProcurementSpendingDto> TotalProcurementSpendingAsync(DateTime fromUtc, DateTime toUtc);
        Task<List<InventoryLevelDto>> InventoryLevelsAsync();
        Task<List<InvoiceTurnaroundDto>> InvoiceApprovalTurnaroundAsync(DateTime? fromUtc, DateTime? toUtc);
        Task<int> CreateAsync(CreateReportRequestDto dto);

        Task<ReportResponseDto?> GetByIdAsync(int reportId);

        Task<ReportResponseDto?> UpdateAsync(int reportId, UpdateReportRequestDto dto);

        Task<bool> DeleteAsync(int reportId);
        Task<List<ReportListResponseDto>> ListAsync(string? scope, DateTime? fromDate, DateTime? toDate);
    }
}