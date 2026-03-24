using SupplySync.Models;
using SupplySync.DTOs.Finance;
using SupplySync.Constants.Enums;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigurePaymentMappings()
        {
            //Create
            CreateMap<CreatePaymentRequestDto, Payment>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => PaymentStatus.Initiated))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            //Update
            CreateMap<UpdatePaymentRequestDto, Payment> ()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            //GetOne
            CreateMap<Payment, PaymentResponseDto>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));
            //Get All
            CreateMap<Payment, PaymentListResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));

        }
    }
}