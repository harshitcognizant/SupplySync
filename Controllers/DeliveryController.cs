using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config;
using SupplySync.DTOs.Delivery;
using SupplySync.Models;
using SupplySync.Services.Interfaces;
using SupplySync.DTOs.Notification;
using SupplySync.Constants.Enums;
using SupplySync.Security;

namespace SupplySync.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/deliveries")]
    public class DeliveryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;

        public DeliveryController(AppDbContext context, IMapper mapper, INotificationService notificationService, IAuditLogService auditLogService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        [Authorize(Roles = "Admin,WarehouseManager,VendorUser")]
        [HttpPost]
        public async Task<ActionResult<DeliveryResponseDto>> CreateDelivery(CreateDeliveryRequestDto request)
        {
            // Verify PO exists before creating delivery
            var po = await _context.PurchaseOrders
                .Include(p => p.Contract)
                .FirstOrDefaultAsync(x => x.POID == request.POID && !x.IsDeleted);
            if (po == null) return BadRequest("Invalid Purchase Order ID.");

            // Optionally ensure PO is approved / active contract
            if (po.Contract == null || po.Contract.IsDeleted)
                return BadRequest("Purchase order's contract not available.");

            var delivery = _mapper.Map<Delivery>(request);
            delivery.CreatedAt = DateTime.UtcNow;
            delivery.IsDeleted = false;

            _context.Deliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Audit log (best-effort)
            try
            {
                var user = HttpContext.User;
                await _auditLogService.WriteAsync(
                    user?.GetUserId(),
                    user?.Identity?.Name,
                    "Delivery.Submitted",
                    $"Delivery:{delivery.DeliveryID} PO:{delivery.POID}");
            }
            catch
            {
                // non-fatal
            }

            // Notify Warehouse Managers
            try
            {
                var notifyDto = new CreateBulkNotificationRequestDto
                {
                    ContractID = po.ContractID,
                    Message = $"Delivery submitted for PO #{delivery.POID} by Vendor #{delivery.VendorID}.",
                    Category = NotificationCategory.System,
                    RoleTypes = new List<RoleType> { RoleType.WarehouseManager }
                };
                await _notificationService.SendAsync(notifyDto);
            }
            catch
            {
                // non-fatal
            }

            return Ok(_mapper.Map<DeliveryResponseDto>(delivery));
        }

        // existing endpoints unchanged...
    }
}