using AutoMapper;
using SupplySync.DTOs.Delivery;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _repo;
        private readonly IMapper _mapper;

        public DeliveryService(IDeliveryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateDeliveryRequestDto dto)
        {
            var entity = _mapper.Map<Delivery>(dto);
            var created = await _repo.InsertAsync(entity);
            return created.DeliveryID;
        }

        public async Task<DeliveryResponseDto?> GetByIdAsync(int deliveryId)
        {
            var entity = await _repo.GetByIdAsync(deliveryId);
            return entity == null ? null : _mapper.Map<DeliveryResponseDto>(entity);
        }

        public async Task<DeliveryResponseDto?> UpdateAsync(int deliveryId, UpdateDeliveryRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(deliveryId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            return _mapper.Map<DeliveryResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int deliveryId)
        {
            return await _repo.SoftDeleteAsync(deliveryId);
        }

        public async Task<DeliveryListResponseDto> ListAsync(
            int? poId,
            int? vendorId,
            string? status,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var list = await _repo.ListAsync(poId, vendorId, status, fromDate, toDate);

            return new DeliveryListResponseDto
            {
                Deliveries = _mapper.Map<List<DeliveryResponseDto>>(list),
                TotalCount = list.Count
            };
        }
    }
}