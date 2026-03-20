using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        Task<PurchaseOrder> InsertAsync(PurchaseOrder entity);
        Task<PurchaseOrder?> GetByIdAsync(int poId);
        Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity);
        Task<bool> SoftDeleteAsync(int poId);
        Task<List<PurchaseOrder>> ListAsync(
            int? contractId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate
        );
    }
}