using Microsoft.AspNetCore.Identity;
using SupplySync.API.Data;
using SupplySync.API.DTOs.Auth;
using SupplySync.API.Helpers;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;

namespace SupplySync.API.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly JwtHelper _jwtHelper;
    private readonly AppDbContext _context;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtHelper jwtHelper,
        AppDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtHelper = jwtHelper;
        _context = context;
    }

    public async Task<(bool Success, string Message, object? Errors)> RegisterAsync(RegisterDto dto)
    {
        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, "Registration failed.", result.Errors);

        await _userManager.AddToRoleAsync(user, dto.Role);
        return (true, "User created successfully", null);
    }

    public async Task<(bool Success, string Message, object? Errors)> VendorRegisterAsync(VendorRegisterDto dto)
    {
        var user = new ApplicationUser
        {
            FullName = dto.FullName,
            Email = dto.Email,
            UserName = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return (false, "Registration failed.", result.Errors);

        await _userManager.AddToRoleAsync(user, "Vendor");

        var vendorCode = "VEN-" + DateTime.UtcNow.Ticks.ToString()[^6..];

        var vendor = new Vendor
        {
            VendorCode = vendorCode,
            CompanyName = dto.CompanyName,
            ContactEmail = dto.Email,
            ContactPhone = dto.ContactPhone,
            Address = dto.Address,
            TaxNumber = dto.TaxNumber,
            LicenseNumber = dto.LicenseNumber,
            DocumentPath = dto.DocumentPath,
            Status = VendorStatus.Pending,
            UserId = user.Id
        };

        _context.Vendors.Add(vendor);

        // Notify procurement officers
        var procurementUsers = await _userManager.GetUsersInRoleAsync("ProcurementOfficer");
        foreach (var po in procurementUsers)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = po.Id,
                Message = $"New vendor {dto.CompanyName} has registered and is pending approval.",
                Type = "VendorRegistration"
            });
        }

        await _context.SaveChangesAsync();
        return (true, "Vendor registered successfully. Awaiting approval.", null);
    }

    public async Task<(bool Success, string Message, AuthResponseDto? Data)> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !user.IsActive)
            return (false, "Invalid credentials or account inactive.", null);

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
            return (false, "Invalid credentials.", null);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtHelper.GenerateToken(user, roles);

        // Get vendorId if vendor
        int? vendorId = null;
        if (roles.Contains("Vendor"))
        {
            var vendor = _context.Vendors.FirstOrDefault(v => v.UserId == user.Id);
            vendorId = vendor?.Id;
        }

        var response = new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email!,
            Role = roles.FirstOrDefault() ?? "",
            VendorId = vendorId
        };

        return (true, "Login successful.", response);
    }
}
