using AutoMapper;
using SupplySync.DTOs.InventoryandWarehouse;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public InventoryService(IInventoryRepository inventoryRepository, IWarehouseRepository warehouseRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateInventoryRequestDto dto)
        {
            var existing = await _inventoryRepository.GetByWarehouseAndItemAsync(dto.WarehouseID, dto.Item);

            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                existing.UpdatedAt = DateTime.UtcNow;

                await _inventoryRepository.UpdateAsync(existing);

                return existing.InventoryID;
            }

            var inventory = new Inventory
            {
                WarehouseID = dto.WarehouseID,
                Item = dto.Item,
                Quantity = dto.Quantity,
                DateAdded = dto.DateAdded,
                Status = dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            await _inventoryRepository.InsertAsync(inventory);

            return inventory.InventoryID;
        }


        public async Task<InventoryResponseDto?> GetByIdAsync(int inventoryId)
        {
            if (inventoryId <= 0)
            {
                throw new ArgumentException("Inventory ID must be greater than 0.");
            }

            Inventory? inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (inventory == null || inventory.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found.");
            }

            return _mapper.Map<InventoryResponseDto>(inventory);
        }

        public async Task IssueStockAsync(IssueInventoryRequestDto dto)
        {
            var inventory = await _inventoryRepository
                .GetByWarehouseAndItemAsync(dto.WarehouseID, dto.Item);

            if (inventory == null)
                throw new Exception("Item not found in inventory.");

            if (inventory.Quantity < dto.Quantity)
                throw new Exception("Not enough stock.");

            // 🔥 Reduce stock
            inventory.Quantity -= dto.Quantity;
            inventory.UpdatedAt = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(inventory);
        }

        public async Task<InventoryResponseDto?> UpdateAsync(int inventoryId, UpdateInventoryRequestDto dto)
        {
            if (inventoryId <= 0)
            {
                throw new ArgumentException("Inventory ID must be greater than 0.");
            }

            if (dto == null)
            {
                throw new ArgumentException("Inventory data is required.");
            }

            Inventory? existingInventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (existingInventory == null || existingInventory.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found.");
            }

            // Validate warehouse if being updated
            if (dto.WarehouseID.HasValue && dto.WarehouseID != existingInventory.WarehouseID)
            {
                Warehouse? warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseID.Value);
                if (warehouse == null || warehouse.IsDeleted == true)
                {
                    throw new ArgumentException($"Warehouse with ID {dto.WarehouseID} not available.");
                }
            }

            _mapper.Map(dto, existingInventory);
            existingInventory.UpdatedAt = DateTime.UtcNow;

            Inventory? updatedInventory = await _inventoryRepository.UpdateAsync(existingInventory);

            if (updatedInventory == null)
            {
                throw new ArgumentException("Inventory data not updated. An error occurred.");
            }

            return _mapper.Map<InventoryResponseDto>(updatedInventory);
        }

        public async Task<bool> DeleteAsync(int inventoryId)
        {
            if (inventoryId <= 0)
            {
                throw new ArgumentException("Inventory ID must be greater than 0.");
            }

            Inventory? inventory = await _inventoryRepository.GetByIdAsync(inventoryId);

            if (inventory == null || inventory.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Inventory with ID {inventoryId} not found.");
            }

            bool result = await _inventoryRepository.SoftDeleteAsync(inventoryId);

            if (!result)
            {
                throw new ArgumentException("Inventory not deleted. An error occurred.");
            }

            return true;
        }

        public async Task<List<InventoryListResponseDto>> ListAsync(
            int? warehouseId,
            string? item,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            // Validate warehouse if provided
            if (warehouseId.HasValue)
            {
                Warehouse? warehouse = await _warehouseRepository.GetByIdAsync(warehouseId.Value);
                if (warehouse == null || warehouse.IsDeleted == true)
                {
                    throw new KeyNotFoundException($"Warehouse with ID {warehouseId} not available.");
                }
            }

            List<Inventory> inventories = await _inventoryRepository.ListAsync(warehouseId, item, status, fromDate, toDate);

            if (inventories.Count <= 0)
            {
                throw new KeyNotFoundException("No inventory items available.");
            }

            return _mapper.Map<List<InventoryListResponseDto>>(inventories);
        }
    }
}
