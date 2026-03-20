using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Middleware;
using SupplySync.Models; 
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services;
using SupplySync.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IVendorService, VendorService>();
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// --------------------
// CONTROLLERS & API
// --------------------

builder.Services
	.AddControllers()
	.AddJsonOptions(opts =>
	{
		opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
	});


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

options.TokenValidationParameters = new TokenValidationParameters
{
	ValidateIssuerSigningKey = true,
	IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),
	ValidateIssuer = true,
	ValidIssuer = issuer,
	ValidateAudience = true,
	ValidAudience = audience,
	ValidateLifetime = true,
	ClockSkew = TimeSpan.FromSeconds(30),

	// 👇 Important for [Authorize(Roles = "...")]
	RoleClaimType = ClaimTypes.Role,
	NameClaimType = ClaimTypes.NameIdentifier
};
});
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

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

