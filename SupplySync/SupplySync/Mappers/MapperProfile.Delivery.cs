using SupplySync.Constants.Enums;
using SupplySync.DTOs.Delivery;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        public void ConfigureDeliveryMappings()
        {
            CreateMap<CreateDeliveryRequestDto, Delivery>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertDeliveryStatus(src.Status)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            CreateMap<UpdateDeliveryRequestDto, Delivery>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ConvertDeliveryStatus(src.Status)))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Delivery, DeliveryResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Delivery, DeliveryListResponseDto>();
        }

        private static DeliveryStatus ConvertDeliveryStatus(string status)
        {
            return Enum.TryParse<DeliveryStatus>(status, true, out var parsed)
                ? parsed
                : DeliveryStatus.Shipped;
        }
    }
}