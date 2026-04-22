using System.Linq;
using SupplySync.Config;
using SupplySync.Constants.Enums;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SupplySync.Repositories
{
    public class VendorApplicationRepository : IVendorApplicationRepository
    {
        private readonly AppDbContext _context;
        public VendorApplicationRepository(AppDbContext context) => _context = context;

        public async Task<VendorApplication> CreateAsync(VendorApplication application)
        {
            await _context.VendorApplications.AddAsync(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<VendorApplication?> GetByIdAsync(int id)
        {
            return await _context.VendorApplications
                                 .Include(a => a.Documents)
                                 .FirstOrDefaultAsync(a => a.ApplicationID == id && !a.IsDeleted);
        }

        public async Task UpdateAsync(VendorApplication application)
        {
            _context.VendorApplications.Update(application);
            await _context.SaveChangesAsync();
        }

        public async Task<List<VendorApplication>> ListByStatusAsync(VendorStatus status)
        {
            return await _context.VendorApplications
                                 .Include(a => a.Documents)
                                 .Where(a => !a.IsDeleted && a.Status == status)
                                 .OrderByDescending(a => a.CreatedAt)
                                 .ToListAsync();
        }
    }
}