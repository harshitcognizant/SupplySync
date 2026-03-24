using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Finance;
using SupplySync.Models;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IPurchaseOrderRepository purchaseOrderRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _purchaseOrderRepository = purchaseOrderRepository;
            _mapper = mapper;
        }
        public async Task<int> CreateInvoiceAsync(CreateInvoiceRequestDto dto)
        {
            var po = await _purchaseOrderRepository.GetByIdAsync(dto.POID);
            if (po == null) throw new KeyNotFoundException("Purchase Order not found.");

            if (dto.Amount <= 0) throw new ArgumentException("Invoice amount must be greater than zero.");

            var invoice = _mapper.Map<Invoice>(dto);
            var created = await _invoiceRepository.InsertAsync(invoice);
            return created.InvoiceId;
        }

        public async Task UpdateInvoiceAsync(int id, UpdateInvoiceRequestDto dto)
        {
            var existing = await _invoiceRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Invoice with ID {id} not found.");

            if (!Enum.TryParse<InvoiceStatus>(dto.Status, true, out var validatedStatus))
            {
                throw new ArgumentException($"'{dto.Status}' is not a valid invoice status. " +
                                            $"Allowed: Submitted, UnderReview, Approved, Rejected, Paid.");
            }

            if (existing.Status == InvoiceStatus.Rejected && validatedStatus == InvoiceStatus.Approved)
                throw new InvalidOperationException("Rejected invoices cannot be approved.");

            _mapper.Map(dto, existing);
            existing.Status = validatedStatus;
            await _invoiceRepository.UpdateAsync(existing);
        }

        public async Task<InvoiceResponseDto?> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            return invoice == null ? null : _mapper.Map<InvoiceResponseDto>(invoice);
        }

        public async Task<IEnumerable<InvoiceListResponseDto>> GetInvoiceListAsync()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<InvoiceListResponseDto>>(invoices);
        }
    }
}
