using System.Linq.Expressions;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Bookkeeping;
using DirectoryPlatform.Core.Entities.Bookkeeping;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class BookkeepingServiceTests
{
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IPaymentRepository> _paymentRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BookkeepingService _service;

    public BookkeepingServiceTests()
    {
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _paymentRepoMock = new Mock<IPaymentRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _service = new BookkeepingService(_invoiceRepoMock.Object, _paymentRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateInvoiceAsync_CalculatesSwissVAT()
    {
        var request = new CreateInvoiceRequest
        {
            UserId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Standard Subscription", Quantity = 1, UnitPrice = 100m }
            }
        };

        _invoiceRepoMock.Setup(r => r.GetNextInvoiceNumberAsync()).ReturnsAsync("INV-2026-0001");
        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
            .ReturnsAsync((Invoice i) => i);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Setup for the GetInvoiceAsync call within CreateInvoiceAsync
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) =>
            {
                return new Invoice
                {
                    Id = id,
                    InvoiceNumber = "INV-2026-0001",
                    UserId = request.UserId,
                    Subtotal = 100m,
                    TaxAmount = Math.Round(100m * 0.081m, 2), // 8.10
                    TotalAmount = Math.Round(100m + 100m * 0.081m, 2), // 108.10
                    DueDate = request.DueDate,
                    Status = InvoiceStatus.Draft,
                    LineItems = new List<InvoiceLineItem>
                    {
                        new() { Id = Guid.NewGuid(), Description = "Standard Subscription", Quantity = 1, UnitPrice = 100m, TotalPrice = 100m }
                    },
                    Payments = new List<Payment>()
                };
            });

        var result = await _service.CreateInvoiceAsync(request);

        result.Should().NotBeNull();
        result.InvoiceNumber.Should().Be("INV-2026-0001");
        result.Subtotal.Should().Be(100m);
        result.TaxAmount.Should().Be(8.10m); // Swiss VAT 8.1%
        result.TotalAmount.Should().Be(108.10m);
        result.Currency.Should().Be("CHF");
    }

    [Fact]
    public async Task CreateInvoiceAsync_MultipleLineItems_CalculatesCorrectly()
    {
        var request = new CreateInvoiceRequest
        {
            UserId = Guid.NewGuid(),
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<CreateInvoiceLineItemRequest>
            {
                new() { Description = "Item 1", Quantity = 2, UnitPrice = 50m },
                new() { Description = "Item 2", Quantity = 1, UnitPrice = 100m }
            }
        };

        Invoice? captured = null;
        _invoiceRepoMock.Setup(r => r.GetNextInvoiceNumberAsync()).ReturnsAsync("INV-2026-0002");
        _invoiceRepoMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
            .Callback<Invoice>(i => captured = i)
            .ReturnsAsync((Invoice i) => i);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Guid id) => new Invoice
            {
                Id = id, InvoiceNumber = "INV-2026-0002", UserId = request.UserId,
                Subtotal = 200m, TaxAmount = Math.Round(200m * 0.081m, 2), TotalAmount = Math.Round(200m * 1.081m, 2),
                LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
            });

        await _service.CreateInvoiceAsync(request);

        captured.Should().NotBeNull();
        captured!.Subtotal.Should().Be(200m); // 2*50 + 1*100
        captured.TaxAmount.Should().Be(Math.Round(200m * 0.081m, 2)); // 16.20
        captured.TotalAmount.Should().Be(Math.Round(200m * 1.081m, 2)); // 216.20
    }

    [Fact]
    public async Task GetInvoiceAsync_ExistingInvoice_ReturnsDto()
    {
        var id = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = id, InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
            Status = InvoiceStatus.Issued, DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
        };
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(id)).ReturnsAsync(invoice);

        var result = await _service.GetInvoiceAsync(id);

        result.Should().NotBeNull();
        result!.InvoiceNumber.Should().Be("INV-2026-0001");
        result.Status.Should().Be("Issued");
    }

    [Fact]
    public async Task GetInvoiceAsync_NonExisting_ReturnsNull()
    {
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Invoice?)null);

        var result = await _service.GetInvoiceAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_ToPaid_SetsPaidDate()
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(), InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Status = InvoiceStatus.Issued, Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
        };
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(invoice.Id)).ReturnsAsync(invoice);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.UpdateInvoiceStatusAsync(invoice.Id, new UpdateInvoiceStatusRequest { Status = "Paid" });

        result.Status.Should().Be("Paid");
        invoice.PaidDate.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_NonExisting_ThrowsKeyNotFound()
    {
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Invoice?)null);

        var act = () => _service.UpdateInvoiceStatusAsync(Guid.NewGuid(), new UpdateInvoiceStatusRequest { Status = "Paid" });

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task UpdateInvoiceStatusAsync_WithNotes_AppendsNotes()
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(), InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Status = InvoiceStatus.Issued, Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
            DueDate = DateTime.UtcNow.AddDays(30), Notes = "Original notes",
            LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
        };
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(invoice.Id)).ReturnsAsync(invoice);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.UpdateInvoiceStatusAsync(invoice.Id, new UpdateInvoiceStatusRequest
        {
            Status = "Cancelled",
            Notes = "Cancelled by admin"
        });

        invoice.Notes.Should().Contain("Original notes");
        invoice.Notes.Should().Contain("Cancelled by admin");
    }

    [Fact]
    public async Task RecordPaymentAsync_ValidPayment_ReturnsPaymentDto()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId, InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Status = InvoiceStatus.Issued, Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(invoiceId)).ReturnsAsync(invoice);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => p);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new CreatePaymentRequest
        {
            InvoiceId = invoiceId,
            Amount = 108.10m,
            PaymentMethod = "Twint",
            TransactionReference = "TX-123"
        };

        var result = await _service.RecordPaymentAsync(request, Guid.NewGuid());

        result.Should().NotBeNull();
        result.Amount.Should().Be(108.10m);
        result.PaymentMethod.Should().Be("Twint");
        result.Status.Should().Be("Completed");
        // Full payment should mark invoice as paid
        invoice.Status.Should().Be(InvoiceStatus.Paid);
        invoice.PaidDate.Should().NotBeNull();
    }

    [Fact]
    public async Task RecordPaymentAsync_PartialPayment_DoesNotMarkAsPaid()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId, InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Status = InvoiceStatus.Issued, Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
            DueDate = DateTime.UtcNow.AddDays(30),
            LineItems = new List<InvoiceLineItem>(),
            Payments = new List<Payment>()
        };

        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(invoiceId)).ReturnsAsync(invoice);
        _paymentRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
            .ReturnsAsync((Payment p) => p);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var request = new CreatePaymentRequest
        {
            InvoiceId = invoiceId,
            Amount = 50m,
            PaymentMethod = "BankTransfer"
        };

        await _service.RecordPaymentAsync(request, Guid.NewGuid());

        invoice.Status.Should().Be(InvoiceStatus.Issued); // Not fully paid
    }

    [Fact]
    public async Task RecordPaymentAsync_InvalidPaymentMethod_ThrowsArgumentException()
    {
        var invoiceId = Guid.NewGuid();
        var invoice = new Invoice
        {
            Id = invoiceId, InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
            Status = InvoiceStatus.Issued, TotalAmount = 100m,
            LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
        };
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(invoiceId)).ReturnsAsync(invoice);

        var request = new CreatePaymentRequest
        {
            InvoiceId = invoiceId,
            Amount = 100m,
            PaymentMethod = "Bitcoin"
        };

        var act = () => _service.RecordPaymentAsync(request, Guid.NewGuid());

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid payment method: Bitcoin");
    }

    [Fact]
    public async Task RecordPaymentAsync_NonExistingInvoice_ThrowsKeyNotFound()
    {
        _invoiceRepoMock.Setup(r => r.GetWithDetailsAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Invoice?)null);

        var request = new CreatePaymentRequest { InvoiceId = Guid.NewGuid(), Amount = 100m, PaymentMethod = "Twint" };
        var act = () => _service.RecordPaymentAsync(request, Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetReportAsync_ReturnsReport()
    {
        _invoiceRepoMock.Setup(r => r.GetTotalRevenueAsync(null, null)).ReturnsAsync(10000m);
        _invoiceRepoMock.Setup(r => r.GetCountByStatusAsync("Paid")).ReturnsAsync(50);
        _invoiceRepoMock.Setup(r => r.GetCountByStatusAsync("Overdue")).ReturnsAsync(5);
        _invoiceRepoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(60);
        _invoiceRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Invoice, bool>>>()))
            .ReturnsAsync(new List<Invoice>
            {
                new() { TotalAmount = 200m },
                new() { TotalAmount = 300m }
            });

        var result = await _service.GetReportAsync();

        result.TotalRevenue.Should().Be(10000m);
        result.PaidInvoices.Should().Be(50);
        result.OverdueInvoices.Should().Be(5);
        result.TotalInvoices.Should().Be(60);
        result.OutstandingAmount.Should().Be(500m);
    }

    [Fact]
    public async Task GetUserInvoicesAsync_ReturnsUserInvoices()
    {
        var userId = Guid.NewGuid();
        var invoices = new List<Invoice>
        {
            new()
            {
                Id = Guid.NewGuid(), InvoiceNumber = "INV-2026-0001", UserId = userId,
                Subtotal = 100m, TaxAmount = 8.10m, TotalAmount = 108.10m,
                Status = InvoiceStatus.Paid, DueDate = DateTime.UtcNow.AddDays(30),
                LineItems = new List<InvoiceLineItem>(), Payments = new List<Payment>()
            }
        };
        _invoiceRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(invoices);

        var result = (await _service.GetUserInvoicesAsync(userId)).ToList();

        result.Should().HaveCount(1);
        result[0].InvoiceNumber.Should().Be("INV-2026-0001");
    }

    [Fact]
    public async Task SearchInvoicesAsync_ReturnsPaged()
    {
        var invoices = new List<Invoice>
        {
            new()
            {
                Id = Guid.NewGuid(), InvoiceNumber = "INV-2026-0001", UserId = Guid.NewGuid(),
                TotalAmount = 100m, Status = InvoiceStatus.Issued, DueDate = DateTime.UtcNow.AddDays(30),
                User = new Core.Entities.User { Username = "testuser" }
            }
        };
        _invoiceRepoMock.Setup(r => r.SearchAsync(null, null, null, null, 1, 20))
            .ReturnsAsync((invoices, 1));

        var request = new InvoiceSearchRequest { PageNumber = 1, PageSize = 20 };
        var result = await _service.SearchInvoicesAsync(request);

        result.TotalCount.Should().Be(1);
        result.PageNumber.Should().Be(1);
    }
}
