using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Expense;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class ExpenseService : IExpenseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(IUnitOfWork uow, IMapper mapper, ILogger<ExpenseService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    // ── Expenses ─────────────────────────────────────────────────────────────

    public async Task<ApiResponse<PagedResult<ExpenseListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Expenses.GetPagedAsync(request.PageNumber, request.PageSize, e => !e.IsDeleted, ct);
        return ApiResponse<PagedResult<ExpenseListDto>>.Ok(
            PagedResult<ExpenseListDto>.Create(_mapper.Map<List<ExpenseListDto>>(items), total, request.PageNumber, request.PageSize));
    }

    public async Task<ApiResponse<ExpenseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Expenses.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Expense), id);
        return ApiResponse<ExpenseResponseDto>.Ok(_mapper.Map<ExpenseResponseDto>(entity));
    }

    public async Task<ApiResponse<ExpenseResponseDto>> CreateAsync(CreateExpenseDto dto, string userId, CancellationToken ct = default)
    {
        var catExists = await _uow.ExpenseCategories.AnyAsync(c => c.Id == dto.ExpenseCategoryId && !c.IsDeleted, ct);
        if (!catExists) throw new NotFoundException(nameof(ExpenseCategory), dto.ExpenseCategoryId);

        var entity = _mapper.Map<Expense>(dto);
        entity.RecordedByUserId = userId;
        await _uow.Expenses.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<ExpenseResponseDto>.Created(_mapper.Map<ExpenseResponseDto>(entity));
    }

    public async Task<ApiResponse<ExpenseResponseDto>> UpdateAsync(UpdateExpenseDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Expenses.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(Expense), dto.Id);
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Expenses.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<ExpenseResponseDto>.Ok(_mapper.Map<ExpenseResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Expenses.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Expense), id);
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Expenses.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<List<ExpenseListDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var list = await _uow.Expenses.GetByDateRangeAsync(from, to, ct);
        return ApiResponse<List<ExpenseListDto>>.Ok(_mapper.Map<List<ExpenseListDto>>(list));
    }

    // ── Expense Categories ────────────────────────────────────────────────────

    public async Task<ApiResponse<List<ExpenseCategoryResponseDto>>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var list = await _uow.ExpenseCategories.GetAllAsync(ct);
        return ApiResponse<List<ExpenseCategoryResponseDto>>.Ok(_mapper.Map<List<ExpenseCategoryResponseDto>>(list));
    }

    public async Task<ApiResponse<ExpenseCategoryResponseDto>> CreateCategoryAsync(CreateExpenseCategoryDto dto, CancellationToken ct = default)
    {
        var exists = await _uow.ExpenseCategories.AnyAsync(c => c.Name == dto.Name && !c.IsDeleted, ct);
        if (exists) throw new DuplicateEntryException("Name", dto.Name);

        var entity = _mapper.Map<ExpenseCategory>(dto);
        await _uow.ExpenseCategories.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<ExpenseCategoryResponseDto>.Created(_mapper.Map<ExpenseCategoryResponseDto>(entity));
    }

    public async Task<ApiResponse<ExpenseCategoryResponseDto>> UpdateCategoryAsync(UpdateExpenseCategoryDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.ExpenseCategories.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(ExpenseCategory), dto.Id);
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.ExpenseCategories.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<ExpenseCategoryResponseDto>.Ok(_mapper.Map<ExpenseCategoryResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteCategoryAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.ExpenseCategories.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(ExpenseCategory), id);

        var hasExpenses = await _uow.ExpenseCategories.HasExpensesAsync(id, ct);
        if (hasExpenses) throw new BusinessRuleException("Cannot delete category with recorded expenses.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.ExpenseCategories.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true);
    }
}
