using AutoMapper;
using SupplySync.DTOs.Vendor;
using SupplySync.Models;

namespace SupplySync.Mappers
{
    public partial class MapperProfile
    {
        private void ConfigureVendorApplicationMappings()
        {
            CreateMap<CreateVendorApplicationRequestDto, VendorApplication>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<CreateVendorDocumentRequestDto, VendorApplicationDocument>()
                .ForMember(dest => dest.UploadedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<VendorApplication, VendorApplicationResponseDto>();
            CreateMap<VendorApplicationDocument, DTOs.Vendor.VendorDocumentResponseDto>();
        }
    }
}