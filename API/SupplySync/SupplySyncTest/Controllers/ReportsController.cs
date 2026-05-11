// ============================================================
//  FILE 19 — ReportsController
//  Method tested: ProcurementSpending()
// ============================================================
//
//  ReportsController queries AppDbContext directly (no service
//  layer), so we use an in-memory EF Core database for testing.
//
//  NuGet packages required:
//    Microsoft.EntityFrameworkCore.InMemory
// ============================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplySync.API.Controllers;
using SupplySync.API.Data;
using SupplySync.API.Models;
using Xunit;

namespace SupplySync.Tests.Controllers;

public class ReportsControllerTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _controller = new ReportsController(_context);
    }

    // ── helpers ──────────────────────────────────────────────

    private Vendor CreateVendor(string name)
    {
        var vendor = new Vendor
        {
            VendorCode = "VEN-001",
            CompanyName = name,
            UserId = "vendor-user-" + Guid.NewGuid(),
            Status = VendorStatus.Approved
        };
        _context.Vendors.Add(vendor);
        _context.SaveChanges();
        return vendor;
    }

    private PurchaseOrder CreatePO(int vendorId, int contractId)
    {
        var po = new PurchaseOrder
        {
            PONumber = "PO-TEST-001",
            VendorId = vendorId,
            ContractId = contractId,
            Status = POStatus.Delivered,
            CreatedByUserId = "user-po",
            ExpectedDeliveryDate = DateTime.UtcNow.AddDays(-5)
        };
        _context.PurchaseOrders.Add(po);
        _context.SaveChanges();
        return po;
    }

    // ─────────────────────────────────────────────────────────

    [Fact]
    public async Task ProcurementSpending_ReturnsOnlyApprovedInvoices()
    {
        // Arrange
        var vendor = CreateVendor("Acme Corp");
        var contract = new Contract
        {
            ContractNumber = "CON-001",
            VendorId = vendor.Id,
            Status = ContractStatus.Active,
            CreatedByUserId = "user-po",
            StartDate = DateTime.UtcNow.AddMonths(-6),
            EndDate = DateTime.UtcNow.AddMonths(6)
        };
        _context.Contracts.Add(contract);
        _context.SaveChanges();

        var po = CreatePO(vendor.Id, contract.Id);

        var gr = new GoodsReceipt
        {
            GRNumber = "GR-001",
            PurchaseOrderId = po.Id,
            Status = GoodsReceiptStatus.Accepted,
            ReceivedByUserId = "user-wm"
        };
        _context.GoodsReceipts.Add(gr);
        _context.SaveChanges();

        // Approved invoice — should appear in results
        var approvedInvoice = new Invoice
        {
            InvoiceNumber = "INV-001",
            VendorId = vendor.Id,
            PurchaseOrderId = po.Id,
            GoodsReceiptId = gr.Id,
            TotalAmount = 5000m,
            Status = InvoiceStatus.Approved,
            SubmittedAt = DateTime.UtcNow
        };

        // Submitted invoice — should NOT appear
        var submittedInvoice = new Invoice
        {
            InvoiceNumber = "INV-002",
            VendorId = vendor.Id,
            PurchaseOrderId = po.Id,
            GoodsReceiptId = gr.Id,
            TotalAmount = 1000m,
            Status = InvoiceStatus.Submitted,
            SubmittedAt = DateTime.UtcNow
        };

        _context.Invoices.AddRange(approvedInvoice, submittedInvoice);
        _context.SaveChanges();

        // Act
        var result = await _controller.ProcurementSpending();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);

        // Use reflection to inspect anonymous type
        var value = ok.Value!;
        var total = (decimal)value.GetType().GetProperty("total")!.GetValue(value)!;
        var records = (System.Collections.IList)value
            .GetType().GetProperty("records")!.GetValue(value)!;

        Assert.Equal(5000m, total);   // only approved
        Assert.Single(records);
    }

    [Fact]
    public async Task ProcurementSpending_WhenNoApprovedInvoices_ReturnsTotalZero()
    {
        // Arrange — no invoices seeded

        // Act
        var result = await _controller.ProcurementSpending();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var value = ok.Value!;
        var total = (decimal)value.GetType().GetProperty("total")!.GetValue(value)!;

        Assert.Equal(0m, total);
    }

    public void Dispose() => _context.Dispose();
}