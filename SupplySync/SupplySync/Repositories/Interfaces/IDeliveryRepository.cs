using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IDeliveryRepository
    {
        Task<Delivery> InsertAsync(Delivery entity);
        Task<Delivery?> GetByIdAsync(int deliveryId);
        Task<Delivery> UpdateAsync(Delivery entity);
        Task<bool> SoftDeleteAsync(int deliveryId);
        Task<List<Delivery>> ListAsync(
            int? poId,
            int? vendorId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}