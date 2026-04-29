using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Supplier;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(IUnitOfWork uow, IMapper mapper, ILogger<SupplierService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<List<SupplierResponseDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _uow.Suppliers.GetAllAsync(ct);
        return ApiResponse<List<SupplierResponseDto>>.Ok(_mapper.Map<List<SupplierResponseDto>>(list));
    }

    public async Task<ApiResponse<SupplierResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Suppliers.GetWithPurchasesAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Supplier), id);
        return ApiResponse<SupplierResponseDto>.Ok(_mapper.Map<SupplierResponseDto>(entity));
    }

    public async Task<ApiResponse<SupplierResponseDto>> CreateAsync(CreateSupplierDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Supplier>(dto);
        await _uow.Suppliers.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Supplier '{Name}' created with Id {Id}", entity.Name, entity.Id);
        return ApiResponse<SupplierResponseDto>.Created(_mapper.Map<SupplierResponseDto>(entity));
    }

    public async Task<ApiResponse<SupplierResponseDto>> UpdateAsync(UpdateSupplierDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Suppliers.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(Supplier), dto.Id);
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Suppliers.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<SupplierResponseDto>.Ok(_mapper.Map<SupplierResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Suppliers.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Supplier), id);

        var hasPurchases = await _uow.Suppliers.AnyAsync(s => s.Id == id && s.Purchases.Any(p => !p.IsDeleted), ct);
        if (hasPurchases) throw new BusinessRuleException("Cannot delete supplier with existing purchases.");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Suppliers.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true, "Supplier deleted.");
    }
}
