using DirectoryPlatform.Contracts.DTOs.Bookkeeping;
using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/[controller]")]
public class BookkeepingController : BaseController
{
    private readonly IBookkeepingService _bookkeepingService;

    public BookkeepingController(IBookkeepingService bookkeepingService)
    {
        _bookkeepingService = bookkeepingService;
    }

    [HttpPost("invoices")]
    public async Task<ActionResult<InvoiceDto>> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var invoice = await _bookkeepingService.CreateInvoiceAsync(request);
        return Ok(invoice);
    }

    [HttpGet("invoices/{id}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid id)
    {
        var invoice = await _bookkeepingService.GetInvoiceAsync(id);
        return invoice == null ? NotFound() : Ok(invoice);
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<PagedResultDto<InvoiceSummaryDto>>> SearchInvoices([FromQuery] InvoiceSearchRequest request)
    {
        var result = await _bookkeepingService.SearchInvoicesAsync(request);
        return Ok(result);
    }

    [HttpPut("invoices/{id}/status")]
    public async Task<ActionResult<InvoiceDto>> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusRequest request)
    {
        try
        {
            var invoice = await _bookkeepingService.UpdateInvoiceStatusAsync(id, request);
            return Ok(invoice);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPost("payments")]
    public async Task<ActionResult<PaymentDto>> RecordPayment([FromBody] CreatePaymentRequest request)
    {
        try
        {
            var payment = await _bookkeepingService.RecordPaymentAsync(request, GetCurrentUserId());
            return Ok(payment);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("report")]
    public async Task<ActionResult<BookkeepingReportDto>> GetReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var report = await _bookkeepingService.GetReportAsync(from, to);
        return Ok(report);
    }
}
