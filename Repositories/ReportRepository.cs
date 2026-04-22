using Microsoft.EntityFrameworkCore; 
using SupplySync.DTOs.Report;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Constants.Enums;
using SupplySync.Config; // Added for Enum comparison

namespace SupplySync.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        // --- CRUD Operations ---

        public async Task<Report> InsertAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .FirstOrDefaultAsync(r => r.ReportID == id && !r.IsDeleted);
        }

        public async Task<Report> UpdateAsync(Report report)
        {
            _context.Entry(report).State = EntityState.Modified; // Better practice for updates
            await _context.SaveChangesAsync();
            return report;
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;

            report.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Report>> ListAsync(string? scope, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Reports.Where(r => !r.IsDeleted);

            // Fix: Parse string scope to Enum if it exists
            if (!string.IsNullOrEmpty(scope) && Enum.TryParse<ReportScope>(scope, true, out var scopeEnum))
                query = query.Where(r => r.Scope == scopeEnum);

            if (fromDate.HasValue)
                query = query.Where(r => r.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(r => r.CreatedAt <= toDate.Value);

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        // --- Analytics Implementation ---

        public async Task<List<VendorPerformanceDto>> GetVendorPerformanceAsync(DateTime? fromUtc, DateTime? toUtc, int topN = 50)
        {
            return await _context.Vendors
                .Select(v => new VendorPerformanceDto
                {
                    VendorID = v.VendorID, // Added missing ID mapping
                    VendorName = v.Name,
                    // Note: Actual logic would involve joining with POs and calculating averages
                    AverageDeliveryDelayDays = 0.0
                })
                .Take(topN)
                .ToListAsync();
        }

        public async Task<ProcurementSpendingDto> GetTotalProcurementSpendingAsync(DateTime fromUtc, DateTime toUtc)
        {
            

            return new ProcurementSpendingDto
            {
                PeriodStart = fromUtc,
                PeriodEnd = toUtc,
                TotalSpent = 5000 // Fixed property name to match DTO
            };
        }

        // Fix: Added missing implementations from IReportRepository
        public async Task<List<DeliveryDelayDto>> GetDeliveryDelaysAsync(DateTime? fromUtc, DateTime? toUtc, int max = 100)
        {
            // Implementation placeholder
            return new List<DeliveryDelayDto>();
        }

        public async Task<List<InventoryLevelDto>> GetInventoryLevelsAsync()
        {
            // Implementation placeholder
            return new List<InventoryLevelDto>();
        }

        public async Task<List<InvoiceTurnaroundDto>> GetInvoiceApprovalTurnaroundAsync(DateTime? fromUtc, DateTime? toUtc)
        {
            // Implementation placeholder
            return new List<InvoiceTurnaroundDto>();
        }
    }
}