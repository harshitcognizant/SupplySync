using SupplySync.DTOs.Admin;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Services.Interfaces
{
    public interface IApprovalWorkflowService
    {
        Task<ApprovalWorkflowResponseDto> CreateAsync(CreateApprovalWorkflowRequestDto dto);
        Task<List<ApprovalWorkflowResponseDto>> ListAsync();
        Task DeleteAsync(int id);
    }
}