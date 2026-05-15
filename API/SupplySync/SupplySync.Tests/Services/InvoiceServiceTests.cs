using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using SupplySync.API.DTOs.Invoice;
using SupplySync.API.Interfaces;
using SupplySync.API.Models;
using SupplySync.API.Services;

namespace SupplySync.Tests.Services;

[TestFixture]
public class InvoiceServiceTests
{
    private Mock<IInvoiceRepository> _invoiceRepoMock;
    private Mock<IGenericRepository<Payment>> _paymentRepoMock;
    private Mock<IVendorRepository> _vendorRepoMock;
    private Mock<IPurchaseOrderRepository> _poRepoMock;
    private Mock<IGoodsReceiptRepository> _grRepoMock;
    private Mock<IGenericRepository<Notification>> _notificationRepoMock;
    private Mock<UserManager<ApplicationUser>> _userManagerMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _service;

    [SetUp]
    public void SetUp()
    {
        _invoiceRepoMock      = new Mock<IInvoiceRepository>();
        _paymentRepoMock      = new Mock<IGenericRepository<Payment>>();
        _vendorRepoMock       = new Mock<IVendorRepository>();
        _poRepoMock           = new Mock<IPurchaseOrderRepository>();
        _grRepoMock           = new Mock<IGoodsReceiptRepository>();
        _notificationRepoMock = new Mock<IGenericRepository<Notification>>();
        _mapperMock           = new Mock<IMapper>();

        var store        = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

        // Default: no finance officers (override per test if needed)
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync("FinanceOfficer")).ReturnsAsync(new List<ApplicationUser>());

        _service = new InvoiceService(
            _invoiceRepoMock.Object,
            _paymentRepoMock.Object,
            _vendorRepoMock.Object,
            _poRepoMock.Object,
            _grRepoMock.Object,
            _notificationRepoMock.Object,
            _userManagerMock.Object,
            _mapperMock.Object);
    }

    [Test]
    public async Task ReviewInvoiceAsync_WhenInvoiceNotFound_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(99)).ReturnsAsync((Invoice?)null);

        var (success, message) = await _service.ReviewInvoiceAsync(99, new InvoiceReviewDto(), "user-fo");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Invoice not found."));
    }

    [Test]
    public async Task ReviewInvoiceAsync_WhenNotSubmitted_ReturnsFailure()
    {
        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1)).ReturnsAsync(new Invoice { Id = 1, Status = InvoiceStatus.Approved });

        var (success, message) = await _service.ReviewInvoiceAsync(1, new InvoiceReviewDto(), "user-fo");

        Assert.That(success, Is.False);
        Assert.That(message, Is.EqualTo("Only submitted invoices can be reviewed."));
    }

    [Test]
    public async Task ReviewInvoiceAsync_WhenGRRejected_ReturnsFailure()
    {
        var invoice = new Invoice
        {
            Id           = 1,
            Status       = InvoiceStatus.Submitted,
            GoodsReceipt = new GoodsReceipt { Status = GoodsReceiptStatus.Rejected },
            Vendor       = new Vendor { UserId = "vendor-u" }
        };

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1)).ReturnsAsync(invoice);

        var (success, message) = await _service.ReviewInvoiceAsync(1, new InvoiceReviewDto { Status = "Approved" }, "user-fo");

        Assert.That(success,            Is.False);
        Assert.That(message, Does.Contain("rejected the delivery"));
    }

    [Test]
    public async Task ReviewInvoiceAsync_WhenGRNotAccepted_ReturnsFailure()
    {
        // Only Accepted GR is valid now — PartiallyAccepted removed
        var invoice = new Invoice
        {
            Id           = 1,
            Status       = InvoiceStatus.Submitted,
            GoodsReceipt = new GoodsReceipt { Status = GoodsReceiptStatus.Accepted },
            Vendor       = new Vendor { UserId = "vendor-u" }
        };
        // Change GR to a non-accepted, non-rejected state (simulate pending)
        invoice.GoodsReceipt.Status = (GoodsReceiptStatus)99; // unknown status

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1)).ReturnsAsync(invoice);

        var (success, message) = await _service.ReviewInvoiceAsync(1, new InvoiceReviewDto { Status = "Approved" }, "user-fo");

        Assert.That(success, Is.False);
        Assert.That(message, Does.Contain("has not verified"));
    }

    [Test]
    public async Task ReviewInvoiceAsync_WhenGRAccepted_ApprovesAndNotifiesVendor()
    {
        var invoice = new Invoice
        {
            Id            = 1,
            InvoiceNumber = "INV-001",
            Status        = InvoiceStatus.Submitted,
            GoodsReceipt  = new GoodsReceipt { Status = GoodsReceiptStatus.Accepted },
            Vendor        = new Vendor { UserId = "vendor-u", CompanyName = "Acme" }
        };

        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(1)).ReturnsAsync(invoice);
        _invoiceRepoMock.Setup(r => r.Update(It.IsAny<Invoice>()));
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);
        _invoiceRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);

        var (success, _) = await _service.ReviewInvoiceAsync(1, new InvoiceReviewDto { Status = "Approved" }, "user-fo");

        Assert.That(success,      Is.True);
        Assert.That(invoice.Status, Is.EqualTo(InvoiceStatus.Approved));
        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n => n.Type == "InvoiceReviewed")), Times.Once);
    }

    [Test]
    public async Task SubmitInvoiceAsync_NotifiesAllFinanceOfficers()
    {
        var vendor = new Vendor { Id = 1, UserId = "vendor-u", CompanyName = "Acme", Status = VendorStatus.Approved };
        var po     = new PurchaseOrder { Id = 1, VendorId = 1, PONumber = "PO-001", Vendor = vendor };
        var gr     = new GoodsReceipt  { Id = 1, PurchaseOrderId = 1, Status = GoodsReceiptStatus.Accepted };

        _vendorRepoMock.Setup(r => r.GetByUserIdAsync("vendor-u")).ReturnsAsync(vendor);
        _poRepoMock.Setup(r => r.GetPOWithDetailsAsync(1)).ReturnsAsync(po);
        _grRepoMock.Setup(r => r.GetGRWithDetailsAsync(1)).ReturnsAsync(gr);
        _invoiceRepoMock.Setup(r => r.IsDuplicateInvoiceAsync(1, 1)).ReturnsAsync(false);
        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>())).Returns(Task.CompletedTask);
        _invoiceRepoMock.Setup(r => r.SaveAsync()).Returns(Task.CompletedTask);
        _invoiceRepoMock.Setup(r => r.GetInvoiceWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new Invoice { Vendor = vendor, PurchaseOrder = po });
        _mapperMock.Setup(m => m.Map<InvoiceDto>(It.IsAny<Invoice>())).Returns(new InvoiceDto());
        _notificationRepoMock.Setup(r => r.AddAsync(It.IsAny<Notification>())).Returns(Task.CompletedTask);

        // Two finance officers
        _userManagerMock.Setup(u => u.GetUsersInRoleAsync("FinanceOfficer")).ReturnsAsync(new List<ApplicationUser>
        {
            new ApplicationUser { Id = "fo-1" },
            new ApplicationUser { Id = "fo-2" }
        });

        var dto = new CreateInvoiceDto { PurchaseOrderId = 1, GoodsReceiptId = 1, TotalAmount = 5000m };
        var (success, _, _) = await _service.SubmitInvoiceAsync(dto, "vendor-u");

        Assert.That(success, Is.True);
        // Should send 1 notification per finance officer = 2 total
        _notificationRepoMock.Verify(r => r.AddAsync(It.Is<Notification>(n => n.Type == "InvoiceSubmitted")), Times.Exactly(2));
    }
}
