using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs;
using SupplySync.DTOs.Vendor;
using SupplySync.Models;

namespace SupplySync.Repositories.Interfaces
{
	public interface IVendorRepository
	{
		public Task<Vendor?> GetVendorById(int vendorId);
		public Task<Vendor> CreateVendor(Vendor newVendor);
		public Task<Vendor> UpdateVendor(Vendor vendor);
		public Task<VendorDocument?> GetVendorDocumentById(int vendorDocumentId);
		public Task<VendorDocument> CreateVendorDocument(VendorDocument newVendorDocument);
		Task<List<VendorDocument>> GetAllVendorDocuments(int vendorId);
		Task<List<Vendor>> GetAllVendorWithFilter(GetVendorFiltersRequestDto getVendorFiltersRequestDto);
	}
}
