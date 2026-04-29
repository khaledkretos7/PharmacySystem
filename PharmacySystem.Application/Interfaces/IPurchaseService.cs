using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Purchase;

namespace PharmacySystem.Application.Interfaces;

public interface IPurchaseService
{
    Task<ApiResponse<PagedResult<PurchaseListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<ApiResponse<PurchaseResponseDto>>          GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<PurchaseResponseDto>>          CreateAsync(CreatePurchaseDto dto, string userId, CancellationToken ct = default);

    /// <summary>
    /// Marks items as received, increments stock, writes InventoryAudit.
    /// Supports partial deliveries — updates status to PartiallyReceived or Received.
    /// </summary>
    Task<ApiResponse<PurchaseResponseDto>>          ReceiveAsync(ReceivePurchaseDto dto, string userId, CancellationToken ct = default);

    Task<ApiResponse<bool>>                         CancelAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<List<PurchaseListDto>>>         GetBySupplierAsync(int supplierId, CancellationToken ct = default);
}
