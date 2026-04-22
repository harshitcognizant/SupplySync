using AutoMapper;
using SupplySync.DTOs.Delivery;
using SupplySync.Models;
using System.Text.Json;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureDeliveryMappings()
        {
            CreateMap<CreateDeliveryRequestDto, Delivery>().ReverseMap();

            CreateMap<CreateDeliveryRequestDto, Delivery>()
                .ForMember(dest => dest.Item, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Item) && src.Items != null && src.Items.Count > 0 ? src.Items[0].Item : src.Item))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => (src.Quantity == 0 && src.Items != null && src.Items.Count > 0) ? src.Items.Sum(i => i.Quantity) : src.Quantity))
                .ForMember(dest => dest.ItemsJson, opt => opt.MapFrom(src => src.Items != null && src.Items.Count > 0 ? JsonSerializer.Serialize(src.Items) : null))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // set in controller/service
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Delivery, DeliveryResponseDto>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.ItemsJson != null ? JsonSerializer.Deserialize<List<CreateDeliveryRequestDto>>(src.ItemsJson) : null));
        }
    }
}