using AutoMapper;
using SupplySync.Constants.Enums;
using SupplySync.DTOs.Finance;
using SupplySync.Models;
using SupplySync.Repositories;
using SupplySync.Repositories.Interfaces;
using SupplySync.Services.Interfaces;

namespace SupplySync.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public PaymentService(IPaymentRepository paymentRepository, IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<int> CreatePaymentAsync(CreatePaymentRequestDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId);

            if (invoice == null)
                throw new Exception("Invoice not found");

            if (invoice.Status != InvoiceStatus.Approved)
                throw new Exception("Invoice must be approved");

            var existingPayment = await _paymentRepository.GetByIdAsync(dto.InvoiceId);
            if (existingPayment != null)
                throw new Exception("Payment already exists");

            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                Amount = dto.Amount,
                Date = dto.Date,
                //Method = Enum.Parse<PaymentMethod>(dto, true),
                Status = PaymentStatus.Success,
                CreatedAt = DateTime.UtcNow
            };

            await _paymentRepository.InsertAsync(payment);

            // ✅ FINAL STATE CHANGE HERE ONLY
            invoice.Status = InvoiceStatus.Paid;
            await _invoiceRepository.UpdateAsync(invoice);

            return payment.PaymentId;
        }


        public async Task UpdatePaymentAsync(int id, UpdatePaymentRequestDto dto)
        {
            var existing = await _paymentRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found.");

            if (!Enum.TryParse<PaymentStatus>(dto.Status, true, out var status))
                throw new ArgumentException($"'{dto.Status}' is not a valid payment status. " +
                                            $"Allowed: Initiated, Success, Failed, Reversed");

            if (!Enum.TryParse<PaymentMethod>(dto.Method, true, out var method))
                throw new ArgumentException($"'{dto.Method}' is not a valid payment method. " +
                                            $"Allowed: NEFT, RTGS, IMPS, Cheque, UPI");

            _mapper.Map(dto, existing);
            existing.Status = status;
            existing.Method = method;
            existing.Amount = dto.Amount;
            if (existing.Status == PaymentStatus.Success)
            {
                var invoice = await _invoiceRepository.GetByIdAsync(existing.InvoiceId);
                // Business rule: mark invoice as Paid if full payment
                if (dto.Amount == invoice.Amount)
                {
                    invoice.Status = InvoiceStatus.Paid;
                    await _invoiceRepository.UpdateAsync(invoice);
                }
                else if (dto.Amount < invoice.Amount)
                {
                    invoice.Status = InvoiceStatus.PartiallyPaid;
                    await _invoiceRepository.UpdateAsync(invoice);
                }
            }
            await _paymentRepository.UpdateAsync(existing);
        }

        public async Task<PaymentResponseDto?> GetPaymentByIdAsync(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            return payment == null ? null : _mapper.Map<PaymentResponseDto>(payment);
        }

        public async Task<IEnumerable<PaymentListResponseDto>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PaymentListResponseDto>>(payments);

        }
    }
}
