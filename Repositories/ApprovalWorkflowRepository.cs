using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SupplySync.Repositories
{
    public class ApprovalWorkflowRepository : IApprovalWorkflowRepository
    {
        private readonly AppDbContext _context;
        public ApprovalWorkflowRepository(AppDbContext context) => _context = context;

        public async Task<ApprovalWorkflow> CreateAsync(ApprovalWorkflow model)
        {
            await _context.ApprovalWorkflows.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<ApprovalWorkflow?> GetByIdAsync(int id)
            => await _context.ApprovalWorkflows.FirstOrDefaultAsync(w => w.WorkflowID == id && !w.IsDeleted);

        public async Task<List<ApprovalWorkflow>> ListAsync()
            => await _context.ApprovalWorkflows.Where(w => !w.IsDeleted).ToListAsync();

        public async Task<ApprovalWorkflow> UpdateAsync(ApprovalWorkflow model)
        {
            _context.ApprovalWorkflows.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _context.ApprovalWorkflows.FirstOrDefaultAsync(w => w.WorkflowID == id && !w.IsDeleted);
            if (e == null) return;
            e.IsDeleted = true;
            e.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}