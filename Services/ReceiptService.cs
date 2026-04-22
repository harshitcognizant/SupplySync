using AutoMapper;
using SupplySync.DTOs.InventoryandWarehouse;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SupplySync.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IReceiptRepository _receiptRepository;
        private readonly IWarehouseRepository _warehouseRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;

        public ReceiptService(
            IReceiptRepository receiptRepository,
            IWarehouseRepository warehouseRepository,
            IInventoryRepository inventoryRepository,
            IDeliveryRepository deliveryRepository,
            INotificationService notificationService,
            IAuditLogService auditLogService,
            IMapper mapper)
        {
            _receiptRepository = receiptRepository;
            _warehouseRepository = warehouseRepository;
            _inventoryRepository = inventoryRepository;
            _delivery_repository_guard(deliveryRepository);
            _deliveryRepository = deliveryRepository;
            _notificationService = notificationService;
            _auditLog_service_guard(auditLogService);
            _auditLogService = auditLogService;
            _mapper = mapper;
        }

        // helper methods to keep constructor assignment lines simple (no behavior change)
        private void _delivery_repository_guard(IDeliveryRepository repo) { }
        private void _auditLog_service_guard(IAuditLogService svc) { }

        public async Task<int> CreateAsync(CreateReceiptRequestDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentException("Receipt data is required.");
            }

            // Validate warehouse exists
            Warehouse? warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseID);
            if (warehouse == null || warehouse.IsDeleted == true)
            {
                throw new ArgumentException($"Warehouse with ID {dto.WarehouseID} not available.");
            }

            // Validate delivery exists
            var delivery = await _deliveryRepository.GetByIdAsync(dto.DeliveryID);
            if (delivery == null || delivery.IsDeleted)
            {
                throw new ArgumentException($"Delivery with ID {dto.DeliveryID} not available.");
            }

            // Map and persist receipt
            Receipt newReceipt = _mapper.Map<Receipt>(dto);
            newReceipt.CreatedAt = DateTime.UtcNow;
            newReceipt.IsDeleted = false;

            Receipt receipt = await _receiptRepository.InsertAsync(newReceipt);

            if (receipt == null)
            {
                throw new ArgumentException("Receipt not created. An error occurred.");
            }

            // Update inventory: try to find existing inventory row by warehouse + item
            // Prefer delivery.Item; fall back to receipt.Quantity if needed.
            var itemName = delivery.Item ?? string.Empty;
            var qty = receipt.Quantity;

            if (!string.IsNullOrWhiteSpace(itemName) && qty > 0)
            {
                var existingInv = await _inventoryRepository.GetByWarehouseAndItemAsync(dto.WarehouseID, itemName);
                if (existingInv != null)
                {
                    existingInv.Quantity += qty;
                    existingInv.UpdatedAt = DateTime.UtcNow;
                    await _inventory_repository_update(existingInv);
                }
                else
                {
                    var inv = new Inventory
                    {
                        WarehouseID = dto.WarehouseID,
                        Item = itemName,
                        Quantity = qty,
                        DateAdded = DateOnly.FromDateTime(DateTime.UtcNow),
                        IsDeleted = false,
                        Status = InventoryStatus.InStock,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _inventoryRepository.InsertAsync(inv);
                }
            }

            // Audit (best-effort)
            try
            {
                await _auditLogService.WriteAsync(
                    null,
                    null,
                    "Receipt.Created",
                    $"Receipt:{receipt.ReceiptID} Delivery:{receipt.DeliveryID} Warehouse:{receipt.WarehouseID}");
            }
            catch
            {
                // non-fatal
            }

            // Notify Procurement Officer(s) that goods were received (best-effort)
            try
            {
                var notifyDto = new CreateBulkNotificationRequestDto
                {
                    Message = $"Receipt created for Delivery #{receipt.DeliveryID} (Receipt #{receipt.ReceiptID}) at Warehouse #{receipt.WarehouseID}. Item: {itemName}, Qty: {qty}",
                    Category = NotificationCategory.System,
                    RoleTypes = new List<Constants.Enums.RoleType> { Constants.Enums.RoleType.ProcurementOfficer }
                };
                await _notificationService.SendAsync(notifyDto);
            }
            catch
            {
                // keep flow robust
            }

            return receipt.ReceiptID;
        }

        // small wrapper to call inventory update to keep code style consistent
        private async Task _inventory_repository_update(Inventory inv)
        {
            await _inventoryRepository.UpdateAsync(inv);
        }

        public async Task<ReceiptResponseDto?> GetByIdAsync(int receiptId)
        {
            if (receiptId <= 0)
            {
                throw new ArgumentException("Receipt ID must be greater than 0.");
            }

            Receipt? receipt = await _receiptRepository.GetByIdAsync(receiptId);

            if (receipt == null || receipt.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Receipt with ID {receiptId} not found.");
            }

            return _mapper.Map<ReceiptResponseDto>(receipt);
        }

        public async Task<ReceiptResponseDto?> UpdateAsync(int receiptId, UpdateReceiptRequestDto dto)
        {
            if (receiptId <= 0)
            {
                throw new ArgumentException("Receipt ID must be greater than 0.");
            }

            if (dto == null)
            {
                throw new ArgumentException("Receipt data is required.");
            }

            Receipt? existingReceipt = await _receiptRepository.GetByIdAsync(receiptId);

            if (existingReceipt == null || existingReceipt.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Receipt with ID {receiptId} not found.");
            }

            // Validate warehouse if being updated
            if (dto.WarehouseID.HasValue && dto.WarehouseID != existingReceipt.WarehouseID)
            {
                Warehouse? warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseID.Value);
                if (warehouse == null || warehouse.IsDeleted == true)
                {
                    throw new ArgumentException($"Warehouse with ID {dto.WarehouseID} not available.");
                }
            }

            _mapper.Map(dto, existingReceipt);
            existingReceipt.UpdatedAt = DateTime.UtcNow;

            Receipt? updatedReceipt = await _receiptRepository.UpdateAsync(existingReceipt);

            if (updatedReceipt == null)
            {
                throw new ArgumentException("Receipt data not updated. An error occurred.");
            }

            return _mapper.Map<ReceiptResponseDto>(updatedReceipt);
        }

        public async Task<bool> DeleteAsync(int receiptId)
        {
            if (receiptId <= 0)
            {
                throw new ArgumentException("Receipt ID must be greater than 0.");
            }

            Receipt? receipt = await _receipt_repository_get(receiptId);

            if (receipt == null || receipt.IsDeleted == true)
            {
                throw new KeyNotFoundException($"Receipt with ID {receiptId} not found.");
            }

            bool result = await _receiptRepository.SoftDeleteAsync(receiptId);

            if (!result)
            {
                throw new ArgumentException("Receipt not deleted. An error occurred.");
            }

            return true;
        }

        private async Task<Receipt?> _receipt_repository_get(int receiptId) => await _receiptRepository.GetByIdAsync(receiptId);

        public async Task<List<ReceiptListResponseDto>> ListAsync(
            int? warehouseId,
            int? deliveryId,
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

            List<Receipt> receipts = await _receiptRepository.ListAsync(warehouseId, deliveryId, status, fromDate, toDate);

            if (receipts.Count <= 0)
            {
                throw new KeyNotFoundException("No receipts available.");
            }

            return _mapper.Map<List<ReceiptListResponseDto>>(receipts);
        }
    }
}