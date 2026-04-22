using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SupplySync.Config;
using SupplySync.Middleware;
using SupplySync.Models;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Security;
using SupplySync.Services;
using SupplySync.Services.Interfaces;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
 
var builder = WebApplication.CreateBuilder(args);

// --------------------
// DB CONTEXT
// --------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AppDb")));

// --------------------
// AUTOMAPPER
// --------------------
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddOpenApi();

// --------------------
// REPOSITORIES
// --------------------
builder.Services.AddScoped<IComplianceRecordRepository, ComplianceRecordRepository>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IVendorRepository, VendorRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IReceiptRepository, ReceiptRepository>();
builder.Services.AddScoped<IVendorCategoryRepository, VendorCategoryRepository>();
builder.Services.AddScoped<IApprovalWorkflowRepository, ApprovalWorkflowRepository>();


// Register new vendor-application repository
builder.Services.AddScoped<IVendorApplicationRepository, VendorApplicationRepository>();

// --------------------
// SERVICES
// --------------------
builder.Services.AddScoped<IComplianceRecordService, ComplianceRecordService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserRoleService, UserRoleService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IVendorCategoryService, VendorCategoryService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IApprovalWorkflowService, ApprovalWorkflowService>();
builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// Register vendor-application service
builder.Services.AddScoped<IVendorApplicationService, VendorApplicationService>();

// Authorization: single-role requirement handler
builder.Services.AddSingleton<IAuthorizationHandler, SingleRoleHandler>();

builder.Services.AddControllers(options =>
{
    // Global: require authenticated users to have exactly one role.
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new SingleRoleRequirement())
        .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
})
.AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// JWT Authentication (unchanged)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSection = builder.Configuration.GetSection("Jwt");
    var key = jwtSection["Key"] ?? throw new InvalidOperationException("Configuration error: 'Jwt:Key' is not set.");
    var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Configuration error: 'Jwt:Issuer' is not set.");
    var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Configuration error: 'Jwt:Audience' is not set.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --------------------
// PIPELINE
// --------------------
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();