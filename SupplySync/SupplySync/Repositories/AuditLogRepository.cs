using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.DTOs.AuditLog;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly AppDbContext _context;

        public AuditLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<AuditLog> Items, int TotalCount)> GetPagedAsync(AuditLogFiltersDto filters)
        {
            var query = _context.AuditLogs
                .Include(a => a.User)
                .AsQueryable();

            // 1. Basic Filters
            query = query.Where(a => !a.IsDeleted);

            if (filters.UserID.HasValue)
                query = query.Where(a => a.UserID == filters.UserID.Value);

            if (!string.IsNullOrWhiteSpace(filters.Action))
                query = query.Where(a => a.Action == filters.Action);

            if (!string.IsNullOrWhiteSpace(filters.Resource))
            {
                var resource = filters.Resource.Trim();
                query = query.Where(a => a.Resource.Contains(resource));
            }

            // 2. Date Range Filters
            if (filters.FromUtc.HasValue)
                query = query.Where(a => a.Timestamp >= filters.FromUtc.Value);

            if (filters.ToUtc.HasValue)
                query = query.Where(a => a.Timestamp <= filters.ToUtc.Value);

            // 3. Global Search
            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var s = filters.Search.Trim();
                query = query.Where(a =>
                    (a.Action != null && a.Action.Contains(s)) ||
                    (a.Resource != null && a.Resource.Contains(s)) ||
                    (a.User != null && a.User.Name != null && a.User.Name.Contains(s))
                );
            }

            // 4. Sorting
            query = filters.Desc
                ? query.OrderByDescending(a => a.Timestamp).ThenByDescending(a => a.AuditID)
                : query.OrderBy(a => a.Timestamp).ThenBy(a => a.AuditID);

            // 5. Execution & Pagination
            var totalCount = await query.CountAsync();

            var page = filters.PageNumber < 1 ? 1 : filters.PageNumber;
            var size = filters.PageSize < 1 ? 20 : filters.PageSize;

            var items = await query.Skip((page - 1) * size)
                                   .Take(size)
                                   .ToListAsync();

            return (items, totalCount);
        }

        public async Task<AuditLog> InsertAsync(AuditLog log)
        {
            await _context.AuditLogs.AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }
    }
}