using System;
using System.Collections;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.Models;

namespace SupplySync.Tests.Controllers;

[TestFixture]
public class ReportsControllerTests : IDisposable
{
    private AppDbContext _context;
    private ReportsController _controller;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context    = new AppDbContext(options);
        _controller = new ReportsController(_context);
    }

    [TearDown]
    public void TearDown() => _context.Dispose();

    [Test]
    public async Task ProcurementSpending_ReturnsOnlyApprovedInvoices()
    {
        var vendor = new Vendor { VendorCode = "V01", CompanyName = "Acme", UserId = "u1", Status = VendorStatus.Approved };
        _context.Vendors.Add(vendor);
        _context.SaveChanges();

        var contract = new Contract { ContractNumber = "C01", VendorId = vendor.Id, Status = ContractStatus.Active, CreatedByUserId = "u1", StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(6) };
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        var po = new PurchaseOrder { PONumber = "PO-01", VendorId = vendor.Id, ContractId = contract.Id, Status = POStatus.Delivered, CreatedByUserId = "u1", ExpectedDeliveryDate = DateTime.UtcNow.AddDays(-5) };
        _context.PurchaseOrders.Add(po);
        _context.SaveChanges();

        var gr = new GoodsReceipt { GRNumber = "GR-01", PurchaseOrderId = po.Id, Status = GoodsReceiptStatus.Accepted, ReceivedByUserId = "u2" };
        _context.GoodsReceipts.Add(gr);
        _context.SaveChanges();

        _context.Invoices.AddRange(
            new Invoice { InvoiceNumber = "INV-01", VendorId = vendor.Id, PurchaseOrderId = po.Id, GoodsReceiptId = gr.Id, TotalAmount = 5000m, Status = InvoiceStatus.Approved,   SubmittedAt = DateTime.UtcNow },
            new Invoice { InvoiceNumber = "INV-02", VendorId = vendor.Id, PurchaseOrderId = po.Id, GoodsReceiptId = gr.Id, TotalAmount = 1000m, Status = InvoiceStatus.Submitted, SubmittedAt = DateTime.UtcNow }
        );
        _context.SaveChanges();

        var result  = await _controller.ProcurementSpending();
        var ok      = result as OkObjectResult;
        var value   = ok!.Value!;
        var total   = (decimal)value.GetType().GetProperty("total")!.GetValue(value)!;
        var records = (IList)value.GetType().GetProperty("records")!.GetValue(value)!;

        Assert.That(total,          Is.EqualTo(5000m));
        Assert.That(records.Count,  Is.EqualTo(1));
    }

    [Test]
    public async Task ProcurementSpending_WhenNoApprovedInvoices_ReturnsTotalZero()
    {
        var result = await _controller.ProcurementSpending();
        var ok     = result as OkObjectResult;
        var value  = ok!.Value!;
        var total  = (decimal)value.GetType().GetProperty("total")!.GetValue(value)!;

        Assert.That(total, Is.EqualTo(0m));
    }

    public void Dispose() => _context.Dispose();
}
