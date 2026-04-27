using AutoMapper;
using Microsoft.AspNetCore.Http;
using SupplySync.Config;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Notification;
using SupplySync.DTOs.UserRoles;
using SupplySync.DTOs.Vendor;
using SupplySync.Models;
using Microsoft.EntityFrameworkCore;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Security;
using SupplySync.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SupplySync.Services
{
    public class VendorApplicationService : IVendorApplicationService
    {
        private readonly INotificationDispatcher _notificationDispatcher;
        private readonly IVendorApplicationRepository _applicationRepo;
        private readonly IVendorRepository _vendorRepo;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserRoleService _userRoleService;
        private readonly AppDbContext _context;

        public VendorApplicationService(
            INotificationDispatcher notificationDispatcher,
            IVendorApplicationRepository applicationRepo,
            IVendorRepository vendorRepo,
            INotificationService notificationService,
            IAuditLogService auditLogService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUserRoleService userRoleService,
            AppDbContext context)
        {
            _notificationDispatcher = notificationDispatcher;
            _applicationRepo = applicationRepo;
            _vendorRepo = vendorRepo;
            _notificationService = notificationService;
            _context = context;
            _auditLogService = auditLogService;
            _mapper = mapper;
            _userRoleService = userRoleService;
            _httpContextAccessor = httpContextAccessor;
        }

        // ✅ CREATE APPLICATION
        public async Task<VendorApplicationResponseDto> CreateApplicationAsync(
     CreateVendorApplicationRequestDto dto)
        {
            // ✅ HARD VALIDATION (NO BAD DATA EVER)
            if (dto.UserID <= 0)
                throw new InvalidOperationException("User must be logged in to apply as vendor.");

            // ✅ Prevent duplicate applications
            if (_context.VendorApplications.Any(x => x.UserId == dto.UserID && !x.IsDeleted))
                throw new InvalidOperationException("Vendor application already exists for this user.");

            var application = new VendorApplication
            {
                UserId = dto.UserID, // ✅ THIS FIXES EVERYTHING
                Name = dto.Name,
                ContactInfo = dto.ContactInfo,
                Category = dto.Category,
                Status = VendorStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false,
                Documents = dto.Documents?.Select(d => new VendorApplicationDocument
                {
                    DocType = d.DocType,
                    FileURI = d.FileURI,
                    UploadedDate = DateTime.UtcNow,
                    VerificationStatus = VendorDocumentVerificationStatus.Pending
                }).ToList()
            };

            var createdApplication = await _applicationRepo.CreateAsync(application);

            // ✅ Notify procurement
            await _notificationService.SendAsync(
                new CreateBulkNotificationRequestDto
                {
                    Message = $"New vendor application received: {createdApplication.Name}",
                    Category = NotificationCategory.System,
                    RoleTypes = new()
                    {
                RoleType.ProcurementOfficer
                    }
                });

            // ✅ Audit log
            await _auditLogService.WriteAsync(
                dto.UserID,
                null,
                "VendorApplication.Submitted",
                $"Application:{createdApplication.ApplicationID}");

            // ✅ Template-based notification
            await _notificationDispatcher.SendNotificationAsync(
                NotificationEvent.VendorApplicationSubmitted,
                new Dictionary<string, object>
                {
                    ["VendorName"] = createdApplication.Name
                }
            );

            return _mapper.Map<VendorApplicationResponseDto>(createdApplication);
        }




        // ✅ LIST PENDING APPLICATIONS
        public async Task<List<VendorApplicationResponseDto>> ListPendingAsync()
        {
            var applications = await _applicationRepo
                .ListByStatusAsync(VendorStatus.Pending);

            return _mapper.Map<List<VendorApplicationResponseDto>>(applications);
        }

        // ✅ GET APPLICATION BY ID
        public async Task<VendorApplicationResponseDto?> GetByIdAsync(int applicationId)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId);
            if (application == null) return null;

            return _mapper.Map<VendorApplicationResponseDto>(application);
        }

        // ✅ APPROVE APPLICATION
        public async Task<VendorResponseDto?> ApproveApplicationAsync(int applicationId)
        {
            // 1️⃣ Load application
            var app = await _context.VendorApplications
                .FirstOrDefaultAsync(x => x.ApplicationID == applicationId);

            if (app == null || app.Status != VendorStatus.Pending)
                return null;

            if (app.UserId <= 0)
                throw new InvalidOperationException("Vendor application is not linked to a valid user.");

            // 2️⃣ Prevent duplicate vendor
            if (_context.Vendors.Any(v => v.UserId == app.UserId))
                throw new InvalidOperationException("Vendor already exists for this user.");

            // 3️⃣ Create Vendor using AutoMapper
            var vendor = _mapper.Map<Vendor>(app);

            vendor.Status = VendorStatus.Approved;
            vendor.UserId = app.UserId;
            vendor.CreatedAt = DateTime.UtcNow;

            _context.Vendors.Add(vendor);

            // 4️⃣ Update application status
            app.Status = VendorStatus.Approved;
            _context.VendorApplications.Update(app);

            // 5️⃣ Update user role (this MUST NOT call SaveChanges internally)
            await _userRoleService.UpdateUserRoleAsync(
                app.UserId,
                new UpdateUserRoleRequestDto
                {
                    CurrentRoleType = RoleType.VendorApplicant,
                    NewRoleType = RoleType.VendorUser
                });

            // ✅ SINGLE SAVE (THIS IS THE KEY FIX)
            await _context.SaveChangesAsync();

            // 6️⃣ Notification (non‑blocking)
            try
            {
                await _notificationService.SendAsync(new CreateBulkNotificationRequestDto
                {
                    Message = "Your vendor application has been approved.",
                    Category = NotificationCategory.System,
                    RoleTypes = new List<RoleType> { RoleType.VendorUser }
                });
            }
            catch { }

            return _mapper.Map<VendorResponseDto>(vendor);
        }








        // ✅ REJECT APPLICATION
        public async Task<bool> RejectApplicationAsync(int applicationId, string reason)
        {
            var application = await _applicationRepo.GetByIdAsync(applicationId);
            if (application == null) return false;

            application.Status = VendorStatus.Blacklisted;
            application.UpdatedAt = DateTime.UtcNow;

            await _applicationRepo.UpdateAsync(application);

            var user = _httpContextAccessor.HttpContext?.User;
            await _auditLogService.WriteAsync(
                user?.GetUserId(),
                user?.Identity?.Name,
                "VendorApplication.Rejected",
                $"Application:{applicationId}, Reason:{reason}");

            return true;
        }
    }
}
