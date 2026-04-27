using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SupplySync.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly AppDbContext _context;

        public InventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Inventory> InsertAsync(Inventory entity)
        {
            await _context.Inventories.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<Inventory?> GetByIdAsync(int inventoryId)
        {
            return await _context.Inventories
                .Where(x => !x.IsDeleted && x.InventoryID == inventoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<Inventory> UpdateAsync(Inventory entity)
        {
            _context.Inventories.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> SoftDeleteAsync(int inventoryId)
        {
            var inventory = await _context.Inventories.FindAsync(inventoryId);
            if (inventory == null) return false;

            inventory.IsDeleted = true;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Inventory>> ListAsync(
            int? warehouseId,
            string? item,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            var query = _context.Inventories
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            if (warehouseId.HasValue)
                query = query.Where(x => x.WarehouseID == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(item))
                query = query.Where(x => x.Item.Contains(item));

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status.ToString() == status);

            if (fromDate.HasValue)
                query = query.Where(x => x.DateAdded >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.DateAdded <= toDate.Value);

            return await query.OrderByDescending(x => x.DateAdded).ToListAsync();
        }

        public async Task<Inventory?> GetByWarehouseAndItemAsync(int warehouseId, string item)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(i => i.WarehouseID == warehouseId && 
                i.Item == item && !i.IsDeleted
                );
        }
    }
}