using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Vendor;

namespace SupplySync.Services.Interfaces
{
	public interface IVendorService
	{
		Task<VendorResponseDto> CreateVendor(CreateVendorApplicationDocumentDto createVendorRequestDto);
		Task<VendorDocumentResponseDto> CreateVendorDocument(CreateVendorDocumentRequestDto createVendorDocumentRequestDto);
		Task<List<VendorDocumentResponseDto>> GetAllVendorDocument(int vendorId);
		Task<List<VendorResponseDto>> GetAllVendorWithFilter(GetVendorFiltersRequestDto getVendorFiltersRequestDto);
		Task<VendorResponseDto> GetVendorById(int vendorId);
		Task<VendorResponseDto> UpdateVendor(int vendorId, UpdateVendorRequestDto updateVendorRequestDto);
	}
}
