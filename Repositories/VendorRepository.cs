using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Vendor;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;

namespace SupplySync.Repositories
{
	public class VendorRepository : IVendorRepository
	{
		private readonly AppDbContext _appDbContext;

		public VendorRepository(AppDbContext appDbContext)
		{
			_appDbContext = appDbContext;
		}

		public async Task<Vendor?> GetVendorById(int vendorId)
		{
			var vendor = await _appDbContext.Vendors.FirstOrDefaultAsync(x=> x.VendorID == vendorId);
			return vendor;
		}

		public async Task<List<Vendor>> GetAllVendorWithFilter(GetVendorFiltersRequestDto filters)
		{
			var query = _appDbContext.Vendors.AsQueryable();

			if (!string.IsNullOrWhiteSpace(filters.Name))
			{
				query = query.Where(x => x.Name.Contains(filters.Name));
			}

			if (filters.Category.HasValue)
			{
				query = query.Where(v => v.Category == filters.Category);
			}

			if (filters.Status.HasValue)
			{
				query = query.Where(v => v.Status == filters.Status);
			}


			return await query
						.Skip((filters.Page - 1) * filters.PageSize)
						.Take(filters.PageSize)
						.ToListAsync();
		}

		public async Task<Vendor?> CreateVendor(Vendor vendor)
		{
			await _appDbContext.Vendors.AddAsync(vendor);
			await _appDbContext.SaveChangesAsync();
			return vendor;
		}
		public async Task<Vendor> UpdateVendor(Vendor vendor)
		{
			_appDbContext.Vendors.Update(vendor);
			await _appDbContext.SaveChangesAsync();
			return vendor;
		}


		public async Task<VendorDocument?> GetVendorDocumentById(int vendorDocumentId)
		{
			var vendorDocument = await _appDbContext.VendorDocuments.FirstOrDefaultAsync(x => x.DocumentID == vendorDocumentId);
			return vendorDocument;
		}

		public async Task<VendorDocument> CreateVendorDocument(VendorDocument newVendorDocument)
		{
			await _appDbContext.VendorDocuments.AddAsync(newVendorDocument);
			await _appDbContext.SaveChangesAsync();
			return newVendorDocument;
		}

		public async Task<List<VendorDocument>> GetAllVendorDocuments(int vendorId)
		{
			return await _appDbContext.VendorDocuments.Where(x => x.VendorID == vendorId).ToListAsync();

		}
        public async Task<List<Vendor>> ListByStatusAsync(VendorStatus status)
        {
            return await _appDbContext.Vendors
                                      .Where(v => !v.IsDeleted && v.Status == status)
                                      .OrderBy(v => v.Name)
                                      .ToListAsync();
        }

    }

}
