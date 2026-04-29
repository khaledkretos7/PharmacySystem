using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Inventory;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(IUnitOfWork uow, IMapper mapper, ILogger<InventoryService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<InventoryAuditResponseDto>>> GetAuditsAsync(PagedRequest request, CancellationToken ct = default)
    {
        var (items, total) = await _uow.InventoryAudits.GetPagedAsync(request.PageNumber, request.PageSize, ct: ct);
        return ApiResponse<PagedResult<InventoryAuditResponseDto>>.Ok(
            PagedResult<InventoryAuditResponseDto>.Create(_mapper.Map<List<InventoryAuditResponseDto>>(items), total, request.PageNumber, request.PageSize));
    }

    public async Task<ApiResponse<List<InventoryAuditResponseDto>>> GetByMedicineAsync(int medicineId, CancellationToken ct = default)
    {
        var list = await _uow.InventoryAudits.GetByMedicineAsync(medicineId, ct);
        return ApiResponse<List<InventoryAuditResponseDto>>.Ok(_mapper.Map<List<InventoryAuditResponseDto>>(list));
    }

    public async Task<ApiResponse<bool>> AdjustStockAsync(StockAdjustmentDto dto, string userId, CancellationToken ct = default)
    {
        var medicine = await _uow.Medicines.GetByIdAsync(dto.MedicineId, ct)
                       ?? throw new NotFoundException(nameof(Medicine), dto.MedicineId);

        int newQty = medicine.QuantityInStock + dto.QuantityAdjusted;
        if (newQty < 0)
            throw new BusinessRuleException($"Adjustment would result in negative stock ({newQty}).");

        int before = medicine.QuantityInStock;
        medicine.QuantityInStock = newQty;
        medicine.UpdatedAt = DateTime.UtcNow;
        if (newQty == 0) medicine.Status = MedicineStatus.OutOfStock;
        else if (medicine.Status == MedicineStatus.OutOfStock) medicine.Status = MedicineStatus.Active;
        _uow.Medicines.Update(medicine);

        await _uow.InventoryAudits.AddAsync(new InventoryAudit
        {
            MedicineId        = dto.MedicineId,
            ActionType        = AuditActionType.Adjustment,
            QuantityChanged   = dto.QuantityAdjusted,
            QuantityBefore    = before,
            QuantityAfter     = newQty,
            BatchNumber       = dto.BatchNumber,
            ExpiryDate        = dto.ExpiryDate,
            ReferenceDocument = dto.ReferenceDocument,
            Notes             = dto.Notes,
            PerformedByUserId = userId
        }, ct);

        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Stock adjusted for Medicine {Id}: {Before} -> {After}", dto.MedicineId, before, newQty);
        return ApiResponse<bool>.Ok(true, $"Stock adjusted. New quantity: {newQty}");
    }

    public async Task<ApiResponse<PhysicalCountResultDto>> RunPhysicalCountAsync(PhysicalCountDto dto, string userId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var variances = new List<StockVarianceDto>();
            decimal totalVarianceValue = 0;

            foreach (var item in dto.Items)
            {
                var medicine = await _uow.Medicines.GetByIdAsync(item.MedicineId, ct)
                               ?? throw new NotFoundException(nameof(Medicine), item.MedicineId);

                int variance      = item.ActualQuantity - medicine.QuantityInStock;
                decimal varValue  = variance * medicine.CostPrice;
                totalVarianceValue += Math.Abs(varValue);

                variances.Add(new StockVarianceDto
                {
                    MedicineId     = medicine.Id,
                    MedicineName   = medicine.Name,
                    SystemQuantity = medicine.QuantityInStock,
                    ActualQuantity = item.ActualQuantity,
                    Variance       = variance,
                    VarianceValue  = varValue
                });

                if (variance != 0)
                {
                    int before = medicine.QuantityInStock;
                    medicine.QuantityInStock = item.ActualQuantity;
                    medicine.UpdatedAt = DateTime.UtcNow;
                    _uow.Medicines.Update(medicine);

                    await _uow.InventoryAudits.AddAsync(new InventoryAudit
                    {
                        MedicineId        = medicine.Id,
                        ActionType        = AuditActionType.Adjustment,
                        QuantityChanged   = variance,
                        QuantityBefore    = before,
                        QuantityAfter     = item.ActualQuantity,
                        Notes             = $"Physical count: {dto.Notes}",
                        ReferenceDocument = "PHYSICAL-COUNT",
                        PerformedByUserId = userId
                    }, ct);
                }
            }

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            return ApiResponse<PhysicalCountResultDto>.Ok(new PhysicalCountResultDto
            {
                AuditDate           = DateTime.UtcNow,
                TotalItemsChecked   = dto.Items.Count,
                DiscrepanciesFound  = variances.Count(v => v.HasDiscrepancy),
                TotalVarianceValue  = totalVarianceValue,
                Variances           = variances
            });
        }
        catch { await _uow.RollbackTransactionAsync(ct); throw; }
    }
}
