using AutoMapper;
using SupplySync.DTOs.PurchaseOrder;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _repo;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreatePurchaseOrderRequestDto dto)
        {
            var entity = _mapper.Map<PurchaseOrder>(dto);
            var created = await _repo.InsertAsync(entity);
            return created.POID;
        }

        public async Task<PurchaseOrderResponseDto?> GetByIdAsync(int poId)
        {
            var entity = await _repo.GetByIdAsync(poId);
            return entity == null ? null : _mapper.Map<PurchaseOrderResponseDto>(entity);
        }

        public async Task<PurchaseOrderResponseDto?> UpdateAsync(int poId, UpdatePurchaseOrderRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(poId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            return _mapper.Map<PurchaseOrderResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int poId)
        {
            return await _repo.SoftDeleteAsync(poId);
        }

        public async Task<PurchaseOrderListResponseDto> ListAsync(
            int? contractId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var list = await _repo.ListAsync(contractId, status, fromDate, toDate);

            return new PurchaseOrderListResponseDto
            {
                PurchaseOrders = _mapper.Map<List<PurchaseOrderResponseDto>>(list),
                TotalCount = list.Count
            };
        }
    }
}