using AutoMapper;
using SupplySync.DTOs.Admin;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplySync.Services
{
    public class ApprovalWorkflowService : IApprovalWorkflowService
    {
        private readonly IApprovalWorkflowRepository _repo;
        private readonly IMapper _mapper;

        public ApprovalWorkflowService(IApprovalWorkflowRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ApprovalWorkflowResponseDto> CreateAsync(CreateApprovalWorkflowRequestDto dto)
        {
            var model = new ApprovalWorkflow
            {
                Resource = dto.Resource,
                ApproverRole = dto.ApproverRole
            };
            var created = await _repo.CreateAsync(model);
            return _mapper.Map<ApprovalWorkflowResponseDto>(created);
        }

        public async Task<List<ApprovalWorkflowResponseDto>> ListAsync()
        {
            var items = await _repo.ListAsync();
            return items.Select(_mapper.Map<ApprovalWorkflowResponseDto>).ToList();
        }

        public async Task DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
        }
    }
}

