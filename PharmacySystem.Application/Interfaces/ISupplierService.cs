using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Supplier;

namespace PharmacySystem.Application.Interfaces;

public interface ISupplierService
{
    Task<ApiResponse<List<SupplierResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResponse<SupplierResponseDto>>       GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<SupplierResponseDto>>       CreateAsync(CreateSupplierDto dto, CancellationToken ct = default);
    Task<ApiResponse<SupplierResponseDto>>       UpdateAsync(UpdateSupplierDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                      DeleteAsync(int id, CancellationToken ct = default);
}
