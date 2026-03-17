using AutoMapper;
using SupplySync.DTOs.Compliance;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class ComplianceRecordService : IComplianceRecordService
    {
        private readonly IComplianceRecordRepository _repo;
        private readonly IMapper _mapper;

        public ComplianceRecordService(IComplianceRecordRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateComplianceRecordRequestDto dto)
        {
            var entity = _mapper.Map<ComplianceRecord>(dto);
            var created = await _repo.InsertAsync(entity);
            return created.ComplianceID;
        }

        public async Task<ComplianceRecordResponseDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ComplianceRecordResponseDto>(entity);
        }

        public async Task<ComplianceRecordResponseDto?> UpdateAsync(int id, UpdateComplianceRecordRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            return _mapper.Map<ComplianceRecordResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.SoftDeleteAsync(id);
        }

        public async Task<List<ComplianceRecordListResponseDto>> ListAsync(
            int? contractId,
            string? type,
            string? result,
            DateOnly? fromDate,
            DateOnly? toDate)
        {
            var list = await _repo.ListAsync(contractId, type, result, fromDate, toDate);
            return _mapper.Map<List<ComplianceRecordListResponseDto>>(list);
        }
    }
}