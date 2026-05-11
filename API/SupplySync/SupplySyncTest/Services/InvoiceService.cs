// ============================================================
//  FILE 15 — InvoiceService
//  Method tested: ReviewInvoiceAsync()
// ============================================================

using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SupplySync.API.DTOs.Invoice;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;
using Xunit;

namespace SupplySync.Tests.Services;

public class InvoiceServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IGenericRepository<Payment>> _paymentRepoMock;
    private readonly Mock<IVendorRepository> _vendorRepoMock;
    private readonly Mock<IPurchaseOrderRepository> _poRepoMock;
    private readonly Mock<IGoodsReceiptRepository> _grRepoMock;
    private readonly Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly InvoiceService _service;

    public InvoiceServiceTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _paymentRepoMock = new Mock<IGenericRepository<Payment>>();
        _vendorRepoMock = new Mock<IVendorRepository>();
        _poRepoMock = new Mock<IPurchaseOrderRepository>();
        _grRepoMock = new Mock<IGoodsReceiptRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock = new Mock<IMapper>();

        _service = new InvoiceService(
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
            _vendorRepoMock.Object,
            _poRepoMock.Object,
            _grRepoMock.Object,
            _notificationRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task ReviewInvoiceAsync_WhenInvoiceNotFound_ReturnsFailure()
    {
        // Arrange
        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(99))
            .ReturnsAsync((Invoice?)null);

        // Act
        var (success, message) =
            await _service.ReviewInvoiceAsync(99, new InvoiceReviewDto(), "user-fo");

        // Assert
        Assert.False(success);
        Assert.Equal("Invoice not found.", message);
    }

    [Fact]
    public async Task ReviewInvoiceAsync_WhenNotSubmitted_ReturnsFailure()
    {
        // Arrange
        var invoice = new Invoice { Id = 1, Status = InvoiceStatus.Approved };

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1))
            .ReturnsAsync(invoice);

        // Act
        var (success, message) =
            await _service.ReviewInvoiceAsync(1, new InvoiceReviewDto(), "user-fo");

        // Assert
        Assert.False(success);
        Assert.Equal("Only submitted invoices can be reviewed.", message);
    }

    [Fact]
    public async Task ReviewInvoiceAsync_WhenGRRejected_ReturnsFailure()
    {
        // Arrange
        var gr = new GoodsReceipt { Status = GoodsReceiptStatus.Rejected };
        var vendor = new Vendor { UserId = "vendor-user" };
        var invoice = new Invoice
        {
            Id = 1,
            Status = InvoiceStatus.Submitted,
            GoodsReceipt = gr,
            Vendor = vendor
        };

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1))
            .ReturnsAsync(invoice);

        var dto = new InvoiceReviewDto { Status = "Approved" };

        // Act
        var (success, message) =
            await _service.ReviewInvoiceAsync(1, dto, "user-fo");

        // Assert
        Assert.False(success);
        Assert.Contains("rejected the delivery", message);
    }

    [Fact]
    public async Task ReviewInvoiceAsync_WhenGRAccepted_ApprovesAndNotifiesVendor()
    {
        // Arrange
        var gr = new GoodsReceipt { Status = GoodsReceiptStatus.Accepted };
        var vendor = new Vendor { UserId = "vendor-user", CompanyName = "Acme" };
        var invoice = new Invoice
        {
            Id = 1,
            InvoiceNumber = "INV-00000001",
            Status = InvoiceStatus.Submitted,
            GoodsReceipt = gr,
            Vendor = vendor
        };

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1))
            .ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.Update(It.IsAny<Invoice>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>()))
            .Returns(Task.CompletedTask);
        _invoiceRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var dto = new InvoiceReviewDto { Status = "Approved" };

        // Act
        var (success, message) =
            await _service.ReviewInvoiceAsync(1, dto, "user-fo");

        // Assert
        Assert.True(success);
        Assert.Equal(InvoiceStatus.Approved, invoice.Status);
        _notificationRepoMock.Verify(
            r => r.AddAsync(It.Is<Notification>(n => n.Type == "InvoiceReviewed")),
            Times.Once);
    }
}