using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        private readonly AppDbContext _context;

        public PurchaseOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PurchaseOrder> InsertAsync(PurchaseOrder entity)
        {
            await _context.PurchaseOrders.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int poId)
        {
            return await _context.PurchaseOrders
                .Include(x => x.Contract)
                .Include(x => x.Deliveries)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.POID == poId);
        }

        public async Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity)
        {
            _context.PurchaseOrders.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int poId)
        {
            var po = await _context.PurchaseOrders.FindAsync(poId);
            if (po == null) return false;

            po.IsDeleted = true;
            po.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PurchaseOrder>> ListAsync(
            int? contractId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.PurchaseOrders
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (contractId.HasValue)
                query = query.Where(x => x.ContractID == contractId.Value);

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