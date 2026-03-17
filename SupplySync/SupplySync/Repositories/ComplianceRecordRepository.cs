using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class ComplianceRecordRepository : IComplianceRecordRepository
    {
        private readonly AppDbContext _context;

        public ComplianceRecordRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ComplianceRecord> InsertAsync(ComplianceRecord entity)
        {
            await _context.ComplianceRecords.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ComplianceRecord?> GetByIdAsync(int id)
        {
            return await _context.ComplianceRecords
                .Where(x => !x.IsDeleted && x.ComplianceID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<ComplianceRecord> UpdateAsync(ComplianceRecord entity)
        {
            _context.ComplianceRecords.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var record = await _context.ComplianceRecords.FindAsync(id);
            if (record == null) return false;

            record.IsDeleted = true;
            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ComplianceRecord>> ListAsync(
            int? contractId,
            string? type,
            string? result,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            var query = _context.ComplianceRecords
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (contractId.HasValue)
                query = query.Where(x => x.ContractID == contractId.Value);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(x => x.Type.ToString() == type);

            if (!string.IsNullOrWhiteSpace(result))
                query = query.Where(x => x.Result.ToString() == result);

            if (fromDate.HasValue)
                query = query.Where(x => x.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Date <= toDate.Value);

            return await query.OrderByDescending(x => x.Date).ToListAsync();
        }
    }
}