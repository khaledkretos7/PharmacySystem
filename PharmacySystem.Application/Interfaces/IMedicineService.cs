using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Medicine;

namespace PharmacySystem.Application.Interfaces;

/// <summary>Medicine (drug) management service.</summary>
public interface IMedicineService
{
    Task<ApiResponse<PagedResult<MedicineListDto>>>  GetPagedAsync(MedicineFilterDto filter, CancellationToken ct = default);
    Task<ApiResponse<MedicineResponseDto>>           GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<MedicineResponseDto>>           GetByBarcodeAsync(string barcode, CancellationToken ct = default);
    Task<ApiResponse<MedicineResponseDto>>           CreateAsync(CreateMedicineDto dto, CancellationToken ct = default);
    Task<ApiResponse<MedicineResponseDto>>           UpdateAsync(UpdateMedicineDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                          DeleteAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<List<MedicineListDto>>>         GetLowStockAsync(CancellationToken ct = default);
    Task<ApiResponse<List<MedicineListDto>>>         GetNearExpiryAsync(int daysThreshold = 30, CancellationToken ct = default);
    Task<ApiResponse<List<MedicineListDto>>>         GetExpiredAsync(CancellationToken ct = default);
    Task<ApiResponse<List<MedicineListDto>>>         GetByCategoryAsync(int categoryId, CancellationToken ct = default);
    Task<ApiResponse<List<MedicineListDto>>>         SearchAsync(string searchTerm, CancellationToken ct = default);
}
