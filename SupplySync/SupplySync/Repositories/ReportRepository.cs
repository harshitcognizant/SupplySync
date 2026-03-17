using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Report> InsertAsync(Report entity)
        {
            await _context.Reports.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Where(x => !x.IsDeleted && x.ReportID == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Report> UpdateAsync(Report entity)
        {
            _context.Reports.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;

            report.IsDeleted = true;
            report.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Report>> ListAsync(
            string? scope,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.Reports
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(scope))
                query = query.Where(x => x.Scope.ToString() == scope);

            if (fromDate.HasValue)
                query = query.Where(x => x.GeneratedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.GeneratedDate <= toDate.Value);

            return await query.OrderByDescending(x => x.GeneratedDate).ToListAsync();
        }
    }
}