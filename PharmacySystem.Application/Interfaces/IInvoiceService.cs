using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Invoice;

namespace PharmacySystem.Application.Interfaces;

/// <summary>Sales invoice and returns management.</summary>
public interface IInvoiceService
{
    Task<ApiResponse<PagedResult<InvoiceListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<ApiResponse<InvoiceResponseDto>>          GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<InvoiceResponseDto>>          GetByNumberAsync(string invoiceNumber, CancellationToken ct = default);

    /// <summary>
    /// Creates a confirmed invoice:
    /// • Validates stock availability per line
    /// • Blocks expired medicines
    /// • Deducts stock and writes InventoryAudit rows
    /// • Generates sequential invoice number
    /// All in one DB transaction.
    /// </summary>
    Task<ApiResponse<InvoiceResponseDto>>          CreateAsync(CreateInvoiceDto dto, string userId, CancellationToken ct = default);

    Task<ApiResponse<bool>>                        CancelAsync(int id, string userId, CancellationToken ct = default);

    /// <summary>
    /// Processes a return:
    /// • Validates quantity against original invoice line
    /// • Restores stock and writes InventoryAudit row
    /// • Issues refund flag
    /// </summary>
    Task<ApiResponse<InvoiceReturnResponseDto>>    CreateReturnAsync(CreateInvoiceReturnDto dto, string userId, CancellationToken ct = default);

    Task<ApiResponse<List<InvoiceListDto>>>        GetByCustomerAsync(int customerId, CancellationToken ct = default);
    Task<ApiResponse<List<InvoiceListDto>>>        GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
