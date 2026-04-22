using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.Config; 
using SupplySync.DTOs.PurchaseOrder;
using SupplySync.Models;
using SupplySync.Services.Interfaces;
using SupplySync.DTOs.Notification;
using SupplySync.Constants.Enums;
using SupplySync.Security;

namespace SupplySync.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/purchase-orders")]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;

        public PurchaseOrderController(
            AppDbContext context,
            IMapper mapper,
            INotificationService notificationService,
            IAuditLogService auditLogService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
        }

        [Authorize(Roles = "Admin,ProcurementOfficer")]
        [HttpPost]
        public async Task<ActionResult<PurchaseOrderResponseDto>> CreatePurchaseOrder(CreatePurchaseOrderRequestDto request)
        {
            // Validate contract exists and is active
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(c => c.ContractID == request.ContractID && !c.IsDeleted);

            if (contract == null)
                return BadRequest(new { Message = "Contract not found." });

            if (contract.Status != ContractStatus.Active)
                return BadRequest(new { Message = "Purchase orders may only be created for active contracts." });

            var purchaseOrder = _mapper.Map<PurchaseOrder>(request);
            purchaseOrder.CreatedAt = DateTime.UtcNow;
            purchaseOrder.IsDeleted = false;

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            // Audit: who created the PO
            var user = HttpContext.User;
            try
            {
                await _auditLogService.WriteAsync(
                    user?.GetUserId(),
                    user?.Identity?.Name,
                    "PurchaseOrder.Created",
                    $"PO:{purchaseOrder.POID} Contract:{purchaseOrder.ContractID}");
            }
            catch
            {
                // non-fatal
            }

            // Notify vendor (best-effort). Uses VendorUser role as target because users are resolved by role.
            try
            {
                var notifyDto = new CreateBulkNotificationRequestDto
                {
                    ContractID = purchaseOrder.ContractID,
                    Message = $"New Purchase Order created: PO #{purchaseOrder.POID} for Contract #{purchaseOrder.ContractID} — Item: {purchaseOrder.Item}, Qty: {purchaseOrder.Quantity}",
                    Category = NotificationCategory.System,
                    RoleTypes = new List<RoleType> { RoleType.VendorUser }
                };

                await _notificationService.SendAsync(notifyDto);
            }
            catch
            {
                // keep flow robust if notification fails
            }

            var response = _mapper.Map<PurchaseOrderResponseDto>(purchaseOrder);
            return CreatedAtAction(nameof(GetPurchaseOrder), new { poId = response.POID }, response);
        }

        [Authorize(Roles = "Admin,ProcurementOfficer,FinanceOfficer")]
        [HttpPut("{poId}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> UpdatePurchaseOrder(int poId, UpdatePurchaseOrderRequestDto request)
        {
            var po = await _context.PurchaseOrders.FirstOrDefaultAsync(x => x.POID == poId && !x.IsDeleted);
            if (po == null) return NotFound();

            _mapper.Map(request, po);
            po.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(_mapper.Map<PurchaseOrderResponseDto>(po));
        }

        [Authorize(Roles = "Admin,ProcurementOfficer,VendorUser,FinanceOfficer,ComplianceOfficer")]
        [HttpGet("{poId}")]
        public async Task<ActionResult<PurchaseOrderResponseDto>> GetPurchaseOrder(int poId)
        {
            var po = await _context.PurchaseOrders
                .Include(x => x.Contract)
                .FirstOrDefaultAsync(x => x.POID == poId && !x.IsDeleted);

            if (po == null) return NotFound();

            return Ok(_mapper.Map<PurchaseOrderResponseDto>(po));
        }

        [Authorize(Roles = "Admin,ProcurementOfficer,WarehouseManager,FinanceOfficer,ComplianceOfficer")]
        [HttpGet]
        public async Task<ActionResult<PurchaseOrderListResponseDto>> ListPurchaseOrders([FromQuery] string? itemFilter)
        {
            var query = _context.PurchaseOrders.Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(itemFilter))
                query = query.Where(x => x.Item.Contains(itemFilter));

            var pos = await query.ToListAsync();

            return Ok(new PurchaseOrderListResponseDto
            {
                PurchaseOrders = _mapper.Map<List<PurchaseOrderResponseDto>>(pos),
                TotalCount = pos.Count
            });
        }
    }
}