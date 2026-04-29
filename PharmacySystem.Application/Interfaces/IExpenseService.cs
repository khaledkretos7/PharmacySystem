using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Expense;

namespace PharmacySystem.Application.Interfaces;

public interface IExpenseService
{
    Task<ApiResponse<PagedResult<ExpenseListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<ApiResponse<ExpenseResponseDto>>          GetByIdAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<ExpenseResponseDto>>          CreateAsync(CreateExpenseDto dto, string userId, CancellationToken ct = default);
    Task<ApiResponse<ExpenseResponseDto>>          UpdateAsync(UpdateExpenseDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                        DeleteAsync(int id, CancellationToken ct = default);
    Task<ApiResponse<List<ExpenseListDto>>>         GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse<List<ExpenseCategoryResponseDto>>> GetCategoriesAsync(CancellationToken ct = default);
    Task<ApiResponse<ExpenseCategoryResponseDto>>       CreateCategoryAsync(CreateExpenseCategoryDto dto, CancellationToken ct = default);
    Task<ApiResponse<ExpenseCategoryResponseDto>>       UpdateCategoryAsync(UpdateExpenseCategoryDto dto, CancellationToken ct = default);
    Task<ApiResponse<bool>>                             DeleteCategoryAsync(int id, CancellationToken ct = default);
}
