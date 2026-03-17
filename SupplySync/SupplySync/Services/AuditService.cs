using AutoMapper;
using SupplySync.DTOs.Audit;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _auditRepository;
        private readonly IMapper _mapper;

        public AuditService(IAuditRepository auditRepository, IMapper mapper)
        {
            _auditRepository = auditRepository;
            _mapper = mapper;
        }

        public async Task<int> CreateAuditAsync(CreateAuditDto dto)
        {
            var audit = _mapper.Map<Audit>(dto);

            var created = await _auditRepository.InsertAsync(audit);
            return created.AuditID;
            
        }
    }
}