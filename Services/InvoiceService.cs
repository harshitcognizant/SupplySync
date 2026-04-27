using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Finance;
using SupplySync.DTOs.Notification;
using SupplySync.Models;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;
using SupplySync.Security; 

namespace SupplySync.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IMapper _mapper;
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
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
            _notificationService = notificationService;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
            _paymentService = paymentService;
        }

        private void _notification_service_guard(INotificationService svc) { }
        private void _auditLog_service_guard(IAuditLogService svc) { }
        private void _purchase_order_repository_guard(IPurchaseOrderRepository repo) { }


        public async Task<int> CreateInvoiceAsync(CreateInvoiceRequestDto dto)
        {
            var po = await _purchaseOrderRepository.GetByIdAsync(dto.POID);
            if (po == null) throw new Exception("PO not found");

            var invoice = _mapper.Map<Invoice>(dto);
            invoice.Status = InvoiceStatus.Submitted;
            invoice.CreatedAt = DateTime.UtcNow;

            var created = await _invoiceRepository.InsertAsync(invoice);

            // 🔔 Notify Finance
            try
            {
                await _notificationService.SendAsync(new CreateBulkNotificationRequestDto
                {
                    Message = $"New invoice #{created.InvoiceId} submitted.",
                    Category = NotificationCategory.Payment,
                    ContractID = po.ContractID,
                    RoleTypes = new List<RoleType> { RoleType.FinanceOfficer }
                });
            } catch { }

            return created.InvoiceId;
        }

        public async Task UpdateInvoiceAsync(int id, UpdateInvoiceRequestDto dto)
        {
            var existing = await _invoiceRepository.GetByIdAsync(id);
            if (existing == null) throw new Exception("Invoice not found");

            if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var newStatus))
                throw new Exception("Invalid status");

            if (existing.Status == InvoiceStatus.Paid)
                throw new Exception("Invoice already paid");

            var po = await _purchaseOrderRepository.GetByIdAsync(existing.POID);

            // ===============================
            // ✅ APPROVAL FLOW
            // ===============================
            if (newStatus == InvoiceStatus.Approved)
            {
                existing.Status = InvoiceStatus.Approved;
                existing.UpdatedAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(existing);

                // 🔍 Audit log
                var user = _httpContextAccessor.HttpContext?.User;
                await _auditLogService.WriteAsync(
                    user?.GetUserId(),
                    user?.Identity?.Name,
                    "Invoice.Approved",
                    $"Invoice:{existing.InvoiceId}"
                );

                // 🔥 CREATE PAYMENT
                var paymentId = await _paymentService.CreatePaymentAsync(new CreatePaymentRequestDto
                {
                    InvoiceId = existing.InvoiceId,
                    Amount = existing.Amount,
                    Date = DateTime.UtcNow,
                    //Method = "NEFT"
                });

                // 🔔 Notify Vendor
                try
                {
                    await _notificationService.SendAsync(new CreateBulkNotificationRequestDto
                    {
                        Message = $"Invoice #{existing.InvoiceId} approved. Payment #{paymentId} created.",
                        Category = NotificationCategory.Payment,
                        ContractID = po?.ContractID,
                        RoleTypes = new List<RoleType> { RoleType.VendorUser }
                    });
                }
                catch
                {

                }

                // 🔔 Notify Finance
                try
                {
                    await _notificationService.SendAsync(new CreateBulkNotificationRequestDto
                    {
                        Message = $"Payment #{paymentId} created for Invoice #{existing.InvoiceId}.",
                        Category = NotificationCategory.Payment,
                        ContractID = po?.ContractID,
                        RoleTypes = new List<RoleType> { RoleType.FinanceOfficer }
                    });
                }
                catch { }
            }
            else
            {
                existing.Status = newStatus;
                existing.UpdatedAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(existing);
            }
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return _mapper.Map<InvoiceResponseDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceListResponseDto>> GetInvoiceListAsync()
        {
            var list = await _invoiceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InvoiceListResponseDto>>(list);
        }
    }
}