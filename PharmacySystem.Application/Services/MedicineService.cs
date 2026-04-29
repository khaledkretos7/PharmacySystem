using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Medicine;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<MedicineService> _logger;

    public MedicineService(IUnitOfWork uow, IMapper mapper, ILogger<MedicineService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<MedicineListDto>>> GetPagedAsync(MedicineFilterDto filter, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Medicines.GetPagedAsync(
            filter.PageNumber, filter.PageSize,
            m => !m.IsDeleted
                 && (filter.CategoryId == null || m.CategoryId == filter.CategoryId)
                 && (filter.SupplierId == null || m.SupplierId == filter.SupplierId)
                 && (filter.Status == null || m.Status == filter.Status)
                 && (filter.LowStock != true || m.QuantityInStock <= m.ReorderLevel)
                 && (filter.NearExpiry != true || (m.ExpiryDate.HasValue && m.ExpiryDate.Value <= DateTime.UtcNow.AddDays(30)))
                 && (filter.SearchTerm == null ||
                     m.Name.Contains(filter.SearchTerm) ||
                     m.GenericName.Contains(filter.SearchTerm) ||
                     (m.Barcode != null && m.Barcode.Contains(filter.SearchTerm))),
            ct);

        return ApiResponse<PagedResult<MedicineListDto>>.Ok(
            PagedResult<MedicineListDto>.Create(_mapper.Map<List<MedicineListDto>>(items), total, filter.PageNumber, filter.PageSize));
    }

    public async Task<ApiResponse<MedicineResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Medicines.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Medicine), id);
        return ApiResponse<MedicineResponseDto>.Ok(_mapper.Map<MedicineResponseDto>(entity));
    }

    public async Task<ApiResponse<MedicineResponseDto>> GetByBarcodeAsync(string barcode, CancellationToken ct = default)
    {
        var entity = await _uow.Medicines.GetByBarcodeAsync(barcode, ct)
                     ?? throw new NotFoundException(nameof(Medicine), barcode);
        return ApiResponse<MedicineResponseDto>.Ok(_mapper.Map<MedicineResponseDto>(entity));
    }

    public async Task<ApiResponse<MedicineResponseDto>> CreateAsync(CreateMedicineDto dto, CancellationToken ct = default)
    {
        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var exists = await _uow.Medicines.AnyAsync(m => m.Barcode == dto.Barcode && !m.IsDeleted, ct);
            if (exists) throw new DuplicateEntryException("Barcode", dto.Barcode);
        }
        var categoryExists = await _uow.Categories.AnyAsync(c => c.Id == dto.CategoryId && !c.IsDeleted, ct);
        if (!categoryExists) throw new NotFoundException(nameof(Category), dto.CategoryId);

        var entity = _mapper.Map<Medicine>(dto);
        await _uow.Medicines.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Medicine '{Name}' created Id={Id}", entity.Name, entity.Id);
        // reload with nav props
        var created = await _uow.Medicines.GetByIdAsync(entity.Id, ct);
        return ApiResponse<MedicineResponseDto>.Created(_mapper.Map<MedicineResponseDto>(created));
    }

    public async Task<ApiResponse<MedicineResponseDto>> UpdateAsync(UpdateMedicineDto dto, CancellationToken ct = default)
    {
        var entity = await _uow.Medicines.GetByIdAsync(dto.Id, ct)
                     ?? throw new NotFoundException(nameof(Medicine), dto.Id);

        if (!string.IsNullOrWhiteSpace(dto.Barcode))
        {
            var barcodeConflict = await _uow.Medicines.AnyAsync(m => m.Barcode == dto.Barcode && m.Id != dto.Id && !m.IsDeleted, ct);
            if (barcodeConflict) throw new DuplicateEntryException("Barcode", dto.Barcode);
        }
        _mapper.Map(dto, entity);
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Medicines.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<MedicineResponseDto>.Ok(_mapper.Map<MedicineResponseDto>(entity));
    }

    public async Task<ApiResponse<bool>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Medicines.GetByIdAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Medicine), id);
        entity.IsDeleted = true;
        entity.Status    = MedicineStatus.Discontinued;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Medicines.Update(entity);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<List<MedicineListDto>>> GetLowStockAsync(CancellationToken ct = default)
    {
        var list = await _uow.Medicines.GetLowStockAsync(ct);
        return ApiResponse<List<MedicineListDto>>.Ok(_mapper.Map<List<MedicineListDto>>(list));
    }

    public async Task<ApiResponse<List<MedicineListDto>>> GetNearExpiryAsync(int daysThreshold = 30, CancellationToken ct = default)
    {
        var list = await _uow.Medicines.GetNearExpiryAsync(daysThreshold, ct);
        return ApiResponse<List<MedicineListDto>>.Ok(_mapper.Map<List<MedicineListDto>>(list));
    }

    public async Task<ApiResponse<List<MedicineListDto>>> GetExpiredAsync(CancellationToken ct = default)
    {
        var list = await _uow.Medicines.FindAsync(
            m => !m.IsDeleted && m.ExpiryDate.HasValue && m.ExpiryDate.Value < DateTime.UtcNow, ct);
        return ApiResponse<List<MedicineListDto>>.Ok(_mapper.Map<List<MedicineListDto>>(list));
    }

    public async Task<ApiResponse<List<MedicineListDto>>> GetByCategoryAsync(int categoryId, CancellationToken ct = default)
    {
        var list = await _uow.Medicines.GetByCategoryAsync(categoryId, ct);
        return ApiResponse<List<MedicineListDto>>.Ok(_mapper.Map<List<MedicineListDto>>(list));
    }

    public async Task<ApiResponse<List<MedicineListDto>>> SearchAsync(string searchTerm, CancellationToken ct = default)
    {
        var list = await _uow.Medicines.FindAsync(
            m => !m.IsDeleted && (
                m.Name.Contains(searchTerm) ||
                m.GenericName.Contains(searchTerm) ||
                (m.Barcode != null && m.Barcode.Contains(searchTerm))), ct);
        return ApiResponse<List<MedicineListDto>>.Ok(_mapper.Map<List<MedicineListDto>>(list));
    }
}
