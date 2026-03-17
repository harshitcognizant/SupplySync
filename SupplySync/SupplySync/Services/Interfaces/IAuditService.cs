using SupplySync.DTOs.Audit;

namespace SupplySync.Services.Interfaces
{
    public interface IAuditService
    {
        Task<int> CreateAsync(CreateAuditRequestDto dto);
        Task<AuditResponseDto?> GetByIdAsync(int auditId);
        Task<AuditResponseDto?> UpdateAsync(int auditId, UpdateAuditRequestDto dto);
        Task<bool> DeleteAsync(int auditId);

        Task<List<AuditListResponseDto>> ListAsync(
            int? complianceOfficerId,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate
        );
    }
}