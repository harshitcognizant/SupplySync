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
                .ForMember(dest => dest.ApplicationID, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserID))
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<CreateVendorDocumentRequestDto, VendorApplicationDocument>()
                .ForMember(dest => dest.UploadedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<VendorApplication, VendorApplicationResponseDto>();
            CreateMap<VendorApplicationDocument, DTOs.Vendor.VendorDocumentResponseDto>();


            CreateMap<VendorApplication, Vendor>()
                .ForMember(dest => dest.VendorID, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());
        }
    }
}