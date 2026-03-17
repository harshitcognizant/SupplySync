using AutoMapper;
using SupplySync.DTOs.Report;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;
        private readonly IMapper _mapper;

        public ReportService(IReportRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreateReportRequestDto dto)
        {
            var entity = _mapper.Map<Report>(dto);
            var created = await _repo.InsertAsync(entity);
            return created.ReportID;
        }

        public async Task<ReportResponseDto?> GetByIdAsync(int reportId)
        {
            var entity = await _repo.GetByIdAsync(reportId);
            return entity == null ? null : _mapper.Map<ReportResponseDto>(entity);
        }

        public async Task<ReportResponseDto?> UpdateAsync(int reportId, UpdateReportRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(reportId);
            if (existing == null) return null;

            _mapper.Map(dto, existing);
            var updated = await _repo.UpdateAsync(existing);

            return _mapper.Map<ReportResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int reportId)
        {
            return await _repo.SoftDeleteAsync(reportId);
        }

        public async Task<List<ReportListResponseDto>> ListAsync(
            string? scope,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var list = await _repo.ListAsync(scope, fromDate, toDate);
            return _mapper.Map<List<ReportListResponseDto>>(list);
        }
    }
}