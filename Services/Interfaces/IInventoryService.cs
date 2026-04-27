using SupplySync.DTOs.InventoryandWarehouse;

namespace SupplySync.Services.Interfaces
{
    public interface IInventoryService
    {
        Task<int> CreateAsync(CreateInventoryRequestDto dto);
        Task<InventoryResponseDto?> GetByIdAsync(int inventoryId);
        Task<InventoryResponseDto?> UpdateAsync(int inventoryId, UpdateInventoryRequestDto dto);
        Task<bool> DeleteAsync(int inventoryId);

        Task<List<InventoryListResponseDto>> ListAsync(
            int? warehouseId,
            string? item,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate
        );

        Task IssueStockAsync(IssueInventoryRequestDto dto);
    }
}
