using SupplySync.DTOs.Vendor;
using SupplySync.Models;

namespace SupplySync.Mappers
{
	public partial class MapperProfile
	{
		public void ConfigureVendorMappings()
		{
			// Create Vendor
			CreateMap<CreateVendorRequestDto, Vendor>().ReverseMap();
			CreateMap<VendorResponseDto, Vendor>().ReverseMap();

			// Create Vendor Document
			CreateMap<CreateVendorDocumentRequestDto, VendorDocument>().ReverseMap();
			CreateMap<VendorDocumentResponseDto, VendorDocument>().ReverseMap();

			// Update Vendor
			CreateMap<UpdateVendorRequestDto, Vendor>().ReverseMap();

		}
	}
}
