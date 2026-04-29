using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Category;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IUnitOfWork uow, IMapper mapper, ILogger<CategoryService> logger)
    {
        _uow    = uow;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<List<CategoryResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _uow.Categories.GetAllAsync(ct);
        var dtos = _mapper.Map<List<CategoryResponseDto>>(list);
        return ApiResponse<List<CategoryResponseDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<CategoryResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Category), id);
        return ApiResponse<CategoryResponseDto>.Ok(_mapper.Map<CategoryResponseDto>(entity));
    }

    public async Task<ApiResponse<CategoryResponseDto>> CreateAsync(CreateCategoryDto dto, CancellationToken ct = default)
    {
        var exists = await _uow.Categories.AnyAsync(c => c.Name == dto.Name && !c.IsDeleted, ct);
        if (exists) throw new DuplicateEntryException("Name", dto.Name);

        var entity = _mapper.Map<Category>(dto);
        await _uow.Categories.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Category '{Name}' created with Id {Id}", entity.Name, entity.Id);
        return ApiResponse<CategoryResponseDto>.Created(_mapper.Map<CategoryResponseDto>(entity));
    }

    public async Task<ApiResponse<CategoryResponseDto>> UpdateAsync(UpdateCategoryDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(Category), dto.Id);

        var nameTaken = await _uow.Categories.AnyAsync(c => c.Name == dto.Name && c.Id != dto.Id && !c.IsDeleted, ct);
        if (nameTaken) throw new DuplicateEntryException("Name", dto.Name);

        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Categories.Update(entity);
        await _uow.SaveChangesAsync(ct);

        return ApiResponse<CategoryResponseDto>.Ok(_mapper.Map<CategoryResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Categories.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Category), id);

        var hasMedicines = await _uow.Categories.HasMedicinesAsync(id, ct);
        if (hasMedicines) throw new BusinessRuleException("Cannot delete category with associated medicines.");

        entity.IsDeleted  = true;
        entity.UpdatedAt  = DateTime.UtcNow;
        _uow.Categories.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true, "Category deleted.");
    }
}
