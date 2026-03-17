using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly AppDbContext _context;

        public AuditRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Audit> InsertAsync(Audit entity)
        {
            await _context.Audits.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Audit?> GetByIdAsync(int auditId)
        {
            return await _context.Audits
                .Where(x => !x.IsDeleted && x.AuditID == auditId)
                .FirstOrDefaultAsync();
        }

        public async Task<Audit> UpdateAsync(Audit entity)
        {
            _context.Audits.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int auditId)
        {
            var audit = await _context.Audits.FindAsync(auditId);
            if (audit == null) return false;

            audit.IsDeleted = true;
            audit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Audit>> ListAsync(
            int? complianceOfficerId,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            var query = _context.Audits
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (complianceOfficerId.HasValue)
                query = query.Where(x => x.ComplianceOfficerID == complianceOfficerId.Value);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status.ToString() == status);

            if (fromDate.HasValue)
                query = query.Where(x => x.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Date <= toDate.Value);

            return await query.OrderByDescending(x => x.Date).ToListAsync();
        }
    }
}