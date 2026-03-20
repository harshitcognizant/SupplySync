// /SupplySync/Services/AuditLogService.cs
using AutoMapper;
using SupplySync.DTOs.Audit;
using SupplySync.DTOs.AuditLog;
using SupplySync.DTOs.AuditLogs;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class AuditLogService : IAuditLogService
	{
		private readonly IAuditLogRepository _repo;
		private readonly IMapper _mapper;

		public AuditLogService(IAuditLogRepository repo, IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<AuditLogListResponseDto> GetAuditLogsAsync(AuditLogFiltersDto filters)
		{
			var (items, total) = await _repo.GetPagedAsync(filters);
			return new AuditLogListResponseDto
			{
				Items = _mapper.Map<List<AuditLogResponseDto>>(items),
				PageNumber = filters.PageNumber,
				PageSize = filters.PageSize,
				TotalCount = total
			};
		}

		public async Task WriteAsync(int? userId, string? userName, string action, string resource)
		{
			var now = DateTime.UtcNow;

			var log = new AuditLog
			{
				UserID = userId,                    // null for system actions
				Action = action,
				Resource = resource,
				Timestamp = now,                      // you can omit to let DB default fill it
				IsDeleted = false,
				CreatedAt = now,                      // you can omit to let DB default fill it
				UpdatedAt = now
			};

			await _repo.InsertAsync(log);
		}
	}
}
