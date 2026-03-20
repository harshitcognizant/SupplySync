using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly AppDbContext _context;

        public DeliveryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Delivery> InsertAsync(Delivery entity)
        {
            await _context.Deliveries.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Delivery?> GetByIdAsync(int deliveryId)
        {
            return await _context.Deliveries
                .Include(x => x.PurchaseOrder)
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.DeliveryID == deliveryId);
        }

        public async Task<Delivery> UpdateAsync(Delivery entity)
        {
            _context.Deliveries.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int deliveryId)
        {
            var delivery = await _context.Deliveries.FindAsync(deliveryId);
            if (delivery == null) return false;

            delivery.IsDeleted = true;
            delivery.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Delivery>> ListAsync(
            int? poId,
            int? vendorId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var query = _context.Deliveries
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (poId.HasValue)
                query = query.Where(x => x.POID == poId.Value);

            if (vendorId.HasValue)
                query = query.Where(x => x.VendorID == vendorId.Value);

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