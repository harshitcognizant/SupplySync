using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SupplySync.Repositories
{
    public class VendorCategoryRepository : IVendorCategoryRepository
    {
        private readonly AppDbContext _context;
        public VendorCategoryRepository(AppDbContext context) => _context = context;

        public async Task<VendorCategoryConfig> CreateAsync(VendorCategoryConfig model)
        {
            await _context.VendorCategories.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<VendorCategoryConfig?> GetByIdAsync(int id)
            => await _context.VendorCategories.FirstOrDefaultAsync(v => v.VendorCategoryID == id && !v.IsDeleted);

        public async Task<List<VendorCategoryConfig>> ListAsync()
            => await _context.VendorCategories.Where(v => !v.IsDeleted).ToListAsync();

        public async Task<VendorCategoryConfig> UpdateAsync(VendorCategoryConfig model)
        {
            _context.VendorCategories.Update(model);
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var e = await _context.VendorCategories.FirstOrDefaultAsync(v => v.VendorCategoryID == id && !v.IsDeleted);
            if (e == null) return;
            e.IsDeleted = true;
            e.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}