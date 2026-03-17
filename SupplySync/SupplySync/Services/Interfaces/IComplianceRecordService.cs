using SupplySync.DTOs.Compliance;

namespace SupplySync.Services.Interfaces
{
    public interface IComplianceRecordService
    {
        Task<int> CreateAsync(CreateComplianceRecordRequestDto dto);
        Task<ComplianceRecordResponseDto?> GetByIdAsync(int id);
        Task<ComplianceRecordResponseDto?> UpdateAsync(int id, UpdateComplianceRecordRequestDto dto);
        Task<bool> DeleteAsync(int id);

        Task<List<ComplianceRecordListResponseDto>> ListAsync(
            int? contractId,
            string? type,
            string? result,
            DateOnly? fromDate,
            DateOnly? toDate
        );
    }
}