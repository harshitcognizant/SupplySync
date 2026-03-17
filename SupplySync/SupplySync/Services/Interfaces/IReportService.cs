using SupplySync.DTOs.Report;

namespace SupplySync.Services.Interfaces
{
    public interface IReportService
    {
        Task<int> CreateAsync(CreateReportRequestDto dto);
        Task<ReportResponseDto?> GetByIdAsync(int reportId);
        Task<ReportResponseDto?> UpdateAsync(int reportId, UpdateReportRequestDto dto);
        Task<bool> DeleteAsync(int reportId);

        Task<List<ReportListResponseDto>> ListAsync(
            string? scope,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}