using SupplySync.DTOs.Delivery;

namespace SupplySync.Services.Interfaces
{
    public interface IDeliveryService
    {
        Task<int> CreateAsync(CreateDeliveryRequestDto dto);
        Task<DeliveryResponseDto?> GetByIdAsync(int deliveryId);
        Task<DeliveryResponseDto?> UpdateAsync(int deliveryId, UpdateDeliveryRequestDto dto);
        Task<bool> DeleteAsync(int deliveryId);

        Task<DeliveryListResponseDto> ListAsync(
            int? poId,
            int? vendorId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}