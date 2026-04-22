
using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Finance;
using SupplySync.DTOs.Notification;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using SupplySync.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SupplySync.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly INotificationService _notificationService;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPaymentService _paymentService;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IPurchaseOrderRepository purchaseOrderRepository,
            IMapper mapper,
            INotificationService notificationService,
            IAuditLogService auditLogService,
            IHttpContextAccessor httpContextAccessor,
            IPaymentService paymentService)
        {
            _invoiceRepository = invoiceRepository;
            _purchase_order_repository_guard(purchaseOrderRepository);
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
            _notification_service_guard(notificationService);
            _notificationService = notificationService;
            _auditLog_service_guard(auditLogService);
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        // small guards to keep constructor assignments clear (no behavioral change)
        private void _notification_service_guard(INotificationService svc) { }
        private void _auditLog_service_guard(IAuditLogService svc) { }
        private void _purchase_order_repository_guard(IPurchaseOrderRepository repo) { }

        public async Task<int> CreateInvoiceAsync(CreateInvoiceRequestDto dto)
        {
            var po = await _purchaseOrderRepository.GetByIdAsync(dto.POID);
            if (po == null) throw new KeyNotFoundException("Purchase Order not found.");

            if (dto.Amount <= 0) throw new ArgumentException("Invoice amount must be greater than zero.");

            var invoice = _mapper.Map<Invoice>(dto);
            invoice.Status = InvoiceStatus.Submitted;
            invoice.CreatedAt = DateTime.UtcNow;
            invoice.IsDeleted = false;

            var created = await _invoiceRepository.InsertAsync(invoice);

            // Audit (best-effort) — use ClaimsPrincipal extension to get user id
            try
            {
                var user = _httpContextAccessor.HttpContext?.User;
                await _auditLogService.WriteAsync(
                    user?.GetUserId(),
                    user?.Identity?.Name,
                    "Invoice.Submitted",
                    $"Invoice:{created.InvoiceId} PO:{created.POID}");
            }
            catch
            {
                // Do not block on audit failure
            }

            // Notify Finance Officers (best-effort)
            try
            {
                var notifyDto = new CreateBulkNotificationRequestDto
                {
                    Message = $"New invoice #{created.InvoiceId} submitted for PO #{created.POID}. Amount: {created.Amount:C}.",
                    Category = NotificationCategory.Payment,
                    ContractID = po.ContractID,
                    RoleTypes = new List<SupplySync.Constants.Enums.RoleType> { RoleType.FinanceOfficer }
                };

                await _notificationService.SendAsync(notifyDto);
            }
            catch
            {
                // Keep flow robust if notification fails
            }

            return created.InvoiceId;
        }

        public async Task UpdateInvoiceAsync(int id, UpdateInvoiceRequestDto dto)
        {
            var existing = await _invoiceRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Invoice with ID {id} not found.");

            if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var validatedStatus))
            {
                throw new ArgumentException($"'{dto.Status}' is not a valid invoice status.");
            }

            // If approving, run approval checks
            if (validatedStatus == InvoiceStatus.Approved && existing.Status != InvoiceStatus.Approved)
            {
                // Validate Purchase Order exists with related contract & deliveries
                var po = await _purchaseOrderRepository.GetByIdAsync(existing.POID)
                    ?? throw new InvalidOperationException("Linked Purchase Order not found for this invoice.");

                if (po.Contract == null || po.Contract.IsDeleted || po.Contract.Status != ContractStatus.Active)
                    throw new InvalidOperationException("PO's contract is not available or not active.");

                // Require at least one delivery recorded for the PO (delivered quantity > 0)
                var hasDelivery = po.Deliveries?.Any(d => !d.IsDeleted && d.Quantity > 0) ?? false;

                if (!hasDelivery)
                    throw new InvalidOperationException("No confirmed delivery found for the referenced Purchase Order.");
                // Basic contract pricing check: invoice must not exceed contract value (simple guard).
                if (existing.Amount > po.Contract.Value)
                    throw new InvalidOperationException("Invoice amount exceeds contract value.");
            }

            _mapper.Map(dto, existing);
            existing.Status = validatedStatus;
            existing.UpdatedAt = DateTime.UtcNow;
            await _invoice_repository_update(existing);

            // On approval, create payment record and notify vendor/finance
            if (validatedStatus == InvoiceStatus.Approved)
            {
                try
                {
                    // Create payment (use IPaymentService). PaymentService will validate invoice.Status == Approved.
                    var paymentDto = new CreatePaymentRequestDto
                    {
                        InvoiceId = existing.InvoiceId,
                        Amount = existing.Amount,
                        Date = DateTime.UtcNow,
                        Method = "NEFT" // default method; adjust if you need different flow
                    };

                    var paymentId = await _paymentService.CreatePaymentAsync(paymentDto);

                    // Notify vendor that invoice was approved and payment initiated
                    try
                    {
                        var notifyToVendor = new CreateBulkNotificationRequestDto
                        {
                            Message = $"Invoice #{existing.InvoiceId} was approved and payment #{paymentId} was created.",
                            Category = NotificationCategory.Payment,
                            ContractID = po_safe_contractid(existing),
                            RoleTypes = new List<RoleType> { RoleType.VendorUser }
                        };
                        await _notificationService.SendAsync(notifyToVendor);
                    }
                    catch { /* non-fatal */ }

                    // Notify finance that payment record was created (optional)
                    try
                    {
                        var notifyToFinance = new CreateBulkNotificationRequestDto
                        {
                            Message = $"Payment #{paymentId} created for Invoice #{existing.InvoiceId}.",
                            Category = NotificationCategory.Payment,
                            ContractID = po_safe_contractid(existing),
                            RoleTypes = new List<RoleType> { RoleType.FinanceOfficer }
                        };
                        await _notification_service_send_safe(notifyToFinance);
                    }
                    catch { /* non-fatal */ }
                }
                catch
                {
                    // If payment creation fails do not roll back the invoice update here (business choice).
                    // You might want to surface a more specific error depending on your needs.
                }
            }
        }

        // helper to update via repository (keeps style consistent)
        private async Task _invoice_repository_update(Invoice invoice) => await _invoiceRepository.UpdateAsync(invoice);

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice == null ? null : _mapper.Map<InvoiceResponseDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceListResponseDto>> GetInvoiceListAsync()
        {
            var invoices = await _invoice_repository_get_all();
            return _mapper.Map<IEnumerable<InvoiceListResponseDto>>(invoices);
        }

        private async Task<System.Collections.Generic.IEnumerable<Invoice>> _invoice_repository_get_all() => await _invoiceRepository.GetAllAsync();

        // safe helpers to get contract id from PO relationship if available
        private int? po_safe_contractid(Invoice inv)
        {
            try
            {
                var po = _purchaseOrderRepository.GetByIdAsync(inv.POID).GetAwaiter().GetResult();
                return po?.ContractID;
            }
            catch
            {
                return null;
            }
        }

        // small wrapper for notification send that swallows non-fatal exceptions
        private async Task _notification_service_send_safe(CreateBulkNotificationRequestDto dto)
        {
            try
            {
                await _notificationService.SendAsync(dto);
            }
            catch
            {
                // non-fatal
            }
        }
    }
}