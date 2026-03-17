using AutoMapper;
using SupplySync.DTOs.Audit;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repo;
        private readonly IMapper _mapper;

        public AuditService(IAuditRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateAuditRequestDto dto)
        {
            var entity = _mapper.Map<Audit>(dto);
            var created = await _repo.InsertAsync(entity);
            return created.AuditID;
        }

        public async Task<AuditResponseDto?> GetByIdAsync(int auditId)
        {
            var entity = await _repo.GetByIdAsync(auditId);
            return entity == null ? null : _mapper.Map<AuditResponseDto>(entity);
        }

        public async Task<AuditResponseDto?> UpdateAsync(int auditId, UpdateAuditRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(auditId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            return _mapper.Map<AuditResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int auditId)
        {
            return await _repo.SoftDeleteAsync(auditId);
        }

        public async Task<List<AuditListResponseDto>> ListAsync(
            int? complianceOfficerId,
            string? status,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            var list = await _repo.ListAsync(complianceOfficerId, status, fromDate, toDate);
            return _mapper.Map<List<AuditListResponseDto>>(list);
        }
    }
}