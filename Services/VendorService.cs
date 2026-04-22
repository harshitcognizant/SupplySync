using System.Numerics;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SupplySync.DTOs.Vendor;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
	public class VendorService : IVendorService
	{
		private readonly IVendorRepository _vendorRepository;
		private readonly IMapper _mapper;

		public VendorService(IVendorRepository vendorRepository, IMapper mapper)
		{
			_vendorRepository = vendorRepository;
			_mapper = mapper;
		}

		/// <summary>
		///  Vendor Endpoints
		/// </summary>


		public async Task<VendorResponseDto> GetVendorById(int vendorId)
		{
			var vendor = await _vendorRepository.GetVendorById(vendorId);

			if (vendor == null) {
				throw new KeyNotFoundException("Vendor Not Found");
			}
			
			return _mapper.Map<VendorResponseDto>(vendor);
		}

		public async Task<List<VendorResponseDto>> GetAllVendorWithFilter(GetVendorFiltersRequestDto getVendorFiltersRequestDto)
		{
			List<Vendor> vendors = await _vendorRepository.GetAllVendorWithFilter(getVendorFiltersRequestDto);
			if (vendors.Count <= 0)
			{
				throw new KeyNotFoundException("No Vendors Available");
			}
			List<VendorResponseDto> vendorResponseDtos = _mapper.Map<List<VendorResponseDto>>(vendors);
			return vendorResponseDtos;
		}

		public async Task<VendorResponseDto> CreateVendor(CreateVendorApplicationDocumentDto createVendorRequestDto)
		{
			Vendor newVendor = _mapper.Map<Vendor>(createVendorRequestDto);
			// after mapping we will get vendor

			Vendor? vendor =  await _vendorRepository.CreateVendor(newVendor);
			if (vendor == null) {
				throw new ArgumentException("Vendor Not Created, some error occured");
			}
			// map with response dto
			VendorResponseDto vendorResponseDto = _mapper.Map<VendorResponseDto>(vendor);

			return vendorResponseDto;
		}

		public async Task<VendorResponseDto?> UpdateVendor(int vendorId, UpdateVendorRequestDto updateVendorRequestDto)
		{
			Vendor? existingVendor = await _vendorRepository.GetVendorById(vendorId);
			if (existingVendor == null || existingVendor.IsDeleted==true)
			{
				throw new KeyNotFoundException($"Vendor with ID {vendorId} not found.");
			}
			
			_mapper.Map(updateVendorRequestDto, existingVendor);

			existingVendor.UpdatedAt = DateTime.UtcNow;

			Vendor? updatedVendor = await _vendorRepository.UpdateVendor(existingVendor);

			if (updatedVendor == null)
			{
				throw new ArgumentException("Vendor Not Updated, some error occured");
			}

			VendorResponseDto vendorResponseDto = _mapper.Map<VendorResponseDto>(updatedVendor);

			return vendorResponseDto;
		}




		/// <summary>
		///  Vendor Document Endpoints
		/// </summary>

		public async Task<VendorDocumentResponseDto> CreateVendorDocument(CreateVendorDocumentRequestDto createVendorDocumentRequestDto)
		{
			VendorDocument newVendorDocument = _mapper.Map<VendorDocument>(createVendorDocumentRequestDto);

			newVendorDocument.FileURI = "fileUrl added";

			var vendorDocument = await _vendorRepository.CreateVendorDocument(newVendorDocument);
			if(vendorDocument == null)
			{
				throw new ArgumentException("Document Not Created, some error occured");
			}
			VendorDocumentResponseDto vendorDocumentResponseDto = _mapper.Map<VendorDocumentResponseDto>(vendorDocument);

			return vendorDocumentResponseDto;
		}

		public async Task<List<VendorDocumentResponseDto>> GetAllVendorDocument(int vendorId)
		{
			List<VendorDocument> documents = await _vendorRepository.GetAllVendorDocuments(vendorId);
			if (documents.Count <= 0)
			{
				throw new KeyNotFoundException("No Documents Available");
			}
			List<VendorDocumentResponseDto> vendorDocumentResponseDtos = _mapper.Map<List<VendorDocumentResponseDto>>(documents);
			return vendorDocumentResponseDtos;
		}

	}
}
