using SupplySync.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplySync.Repositories.Interfaces
{
    public interface IApprovalWorkflowRepository
    {
        Task<ApprovalWorkflow> CreateAsync(ApprovalWorkflow model);
        Task<ApprovalWorkflow?> GetByIdAsync(int id);
        Task<List<ApprovalWorkflow>> ListAsync();
        Task<ApprovalWorkflow> UpdateAsync(ApprovalWorkflow model);
        Task DeleteAsync(int id);
    }
}