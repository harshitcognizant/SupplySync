using AutoMapper;
using SupplySync.DTOs.Admin;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureAdminMappings()
        {
            CreateMap<VendorCategoryConfig, VendorCategoryResponseDto>();
            CreateMap<CreateVendorCategoryRequestDto, VendorCategoryConfig>();

            CreateMap<ApprovalWorkflow, ApprovalWorkflowResponseDto>();
            CreateMap<CreateApprovalWorkflowRequestDto, ApprovalWorkflow>();
        }
    }
}
