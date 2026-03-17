using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.Middleware;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services;
using SupplySync.Services.Interfaces;

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

// --------------------
// SERVICES
// --------------------
builder.Services.AddScoped<IComplianceRecordService, ComplianceRecordService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IReportService, ReportService>();

// --------------------
// CONTROLLERS & API
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// --------------------
// PIPELINE
// --------------------
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();

