using Microsoft.EntityFrameworkCore;
using SupplySync.Models;

namespace SupplySync.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Audit> Audits => Set<Audit>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<ComplianceRecord> ComplianceRecords => Set<ComplianceRecord>();
        public DbSet<Contract> Contracts => Set<Contract>();
        public DbSet<ContractTerm> ContractTerms => Set<ContractTerm>();
        public DbSet<Delivery> Deliveries => Set<Delivery>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Vendor> Vendors => Set<Vendor>();
        public DbSet<VendorDocument> VendorDocuments => Set<VendorDocument>();
        public DbSet<Warehouse> Warehouses => Set<Warehouse>();

        // NEW:
        public DbSet<VendorCategoryConfig> VendorCategories => Set<VendorCategoryConfig>();
        public DbSet<ApprovalWorkflow> ApprovalWorkflows => Set<ApprovalWorkflow>();
        public DbSet<VendorApplication> VendorApplications => Set<VendorApplication>();

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}