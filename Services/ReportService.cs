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

        // ----- Analytics / Aggregations -----
        public async Task<List<VendorPerformanceDto>> VendorPerformanceAsync(DateTime? fromUtc, DateTime? toUtc, int topN = 50)
            => await _repo.GetVendorPerformanceAsync(fromUtc, toUtc, topN);

        public async Task<List<DeliveryDelayDto>> DeliveryDelaysAsync(DateTime? fromUtc, DateTime? toUtc, int max = 100)
            => await _repo.GetDeliveryDelaysAsync(fromUtc, toUtc, max);

        public async Task<ProcurementSpendingDto> TotalProcurementSpendingAsync(DateTime fromUtc, DateTime toUtc)
            => await _repo.GetTotalProcurementSpendingAsync(fromUtc, toUtc);

        public async Task<List<InventoryLevelDto>> InventoryLevelsAsync()
            => await _repo.GetInventoryLevelsAsync();

        public async Task<List<InvoiceTurnaroundDto>> InvoiceApprovalTurnaroundAsync(DateTime? fromUtc, DateTime? toUtc)
            => await _repo.GetInvoiceApprovalTurnaroundAsync(fromUtc, toUtc);

        // ----- CRUD for Report entity -----
        public async Task<int> CreateAsync(CreateReportRequestDto dto)
        {
            var entity = _mapper.Map<Report>(dto);
            entity.CreatedAt = DateTime.UtcNow;
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
            existing.UpdatedAt = DateTime.UtcNow;
            var updated = await _repo.UpdateAsync(existing);
            return _mapper.Map<ReportResponseDto>(updated);
        }

        public async Task<bool> DeleteAsync(int reportId)
        {
            return await _repo.SoftDeleteAsync(reportId);
        }

        public async Task<List<ReportListResponseDto>> ListAsync(string? scope, DateTime? fromDate, DateTime? toDate)
        {
            var list = await _repo.ListAsync(scope, fromDate, toDate);
            return _mapper.Map<List<ReportListResponseDto>>(list);
        }
    }
}