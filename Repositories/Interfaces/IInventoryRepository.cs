using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        Task<Inventory> InsertAsync(Inventory entity);
        Task<Inventory?> GetByIdAsync(int inventoryId);
        Task<Inventory> UpdateAsync(Inventory entity);

        // Soft-delete
        Task<bool> SoftDeleteAsync(int inventoryId);

        // List with filters
        Task<List<Inventory>> ListAsync(
            int? warehouseId,
            string? item,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate
        );

        // Find a single inventory record by warehouse + item (used when receiving goods)
        Task<Inventory?> GetByWarehouseAndItemAsync(int warehouseId, string item);
    }
}