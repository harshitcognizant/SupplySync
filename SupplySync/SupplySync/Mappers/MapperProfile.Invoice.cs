using SupplySync.Constants.Enums;
using SupplySync.DTOs.Finance;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureInvoiceMappings()
        {
            //Create
            CreateMap<CreateInvoiceRequestDto, Invoice>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => InvoiceStatus.Submitted))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false));

            //Update
            CreateMap<UpdateInvoiceRequestDto, Invoice>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Detailed Mapping
            CreateMap<Invoice, InvoiceResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : "N/A"));

            // List Mapping
            CreateMap<Invoice, InvoiceListResponseDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : "N/A"))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));
        }
    }
}
