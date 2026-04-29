using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Inventory;

namespace PharmacySystem.Application.Interfaces;

public interface IInventoryService
{
    Task<ApiResponse<PagedResult<InventoryAuditResponseDto>>> GetAuditsAsync(PagedRequest request, CancellationToken ct = default);
    Task<ApiResponse<List<InventoryAuditResponseDto>>>        GetByMedicineAsync(int medicineId, CancellationToken ct = default);

    /// <summary>Manual stock adjustment — writes InventoryAudit with type = Adjustment.</summary>
    Task<ApiResponse<bool>>                  AdjustStockAsync(StockAdjustmentDto dto, string userId, CancellationToken ct = default);

    /// <summary>Compare system quantities with physical count, return variance per item.</summary>
    Task<ApiResponse<PhysicalCountResultDto>> RunPhysicalCountAsync(PhysicalCountDto dto, string userId, CancellationToken ct = default);
}
