using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Category;

namespace PharmacySystem.Application.Interfaces;

public interface ICategoryService
{
    Task<ApiResponse<List<CategoryResponseDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ApiResponse<CategoryResponseDto>>       GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<CategoryResponseDto>>       CreateAsync(CreateCategoryDto dto, CancellationToken ct = default);
    Task<ApiResponse<CategoryResponseDto>>       UpdateAsync(UpdateCategoryDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                      DeleteAsync(int id, CancellationToken ct = default);
}
