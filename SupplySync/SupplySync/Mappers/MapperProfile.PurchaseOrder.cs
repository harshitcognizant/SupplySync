using SupplySync.Constants.Enums;
using SupplySync.DTOs.PurchaseOrder;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        public void ConfigurePurchaseOrderMappings()
        {
            CreateMap<CreatePurchaseOrderRequestDto, PurchaseOrder>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertPOStatus(src.Status)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdatePurchaseOrderRequestDto, PurchaseOrder>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertPOStatus(src.Status)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<PurchaseOrder, PurchaseOrderResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Mapping for the list wrapper if needed, 
            // though usually handled manually in controller to set TotalCount
            CreateMap<PurchaseOrder, PurchaseOrderListResponseDto>();
        }

        private static POStatus ConvertPOStatus(string status)
        {
            return Enum.TryParse<POStatus>(status, true, out var parsed)
                ? parsed
                : POStatus.Draft;
        }
    }
}