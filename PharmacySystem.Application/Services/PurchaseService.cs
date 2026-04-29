using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Purchase;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<PurchaseService> _logger;

    public PurchaseService(IUnitOfWork uow, IMapper mapper, ILogger<PurchaseService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<PurchaseListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Purchases.GetPagedAsync(request.PageNumber, request.PageSize, p => !p.IsDeleted, ct);
        return ApiResponse<PagedResult<PurchaseListDto>>.Ok(
            PagedResult<PurchaseListDto>.Create(_mapper.Map<List<PurchaseListDto>>(items), total, request.PageNumber, request.PageSize));
    }

    public async Task<ApiResponse<PurchaseResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Purchases.GetWithItemsAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Purchase), id);
        return ApiResponse<PurchaseResponseDto>.Ok(_mapper.Map<PurchaseResponseDto>(entity));
    }

    public async Task<ApiResponse<PurchaseResponseDto>> CreateAsync(CreatePurchaseDto dto, string userId, CancellationToken ct = default)
    {
        var supplier = await _uow.Suppliers.GetByIdAsync(dto.SupplierId, ct)
                       ?? throw new NotFoundException(nameof(Supplier), dto.SupplierId);

        var items    = new List<PurchaseItem>();
        decimal sub  = 0;
        decimal tax  = 0;
        decimal disc = 0;

        foreach (var i in dto.Items)
        {
            var med = await _uow.Medicines.GetByIdAsync(i.MedicineId, ct)
                      ?? throw new NotFoundException(nameof(Medicine), i.MedicineId);

            decimal discAmt = i.UnitCost * i.Quantity * i.DiscountRate;
            decimal taxAmt  = (i.UnitCost * i.Quantity - discAmt) * i.TaxRate;
            decimal total   = i.UnitCost * i.Quantity - discAmt + taxAmt;

            items.Add(new PurchaseItem
            {
                MedicineId       = i.MedicineId,
                QuantityOrdered  = i.Quantity,
                QuantityReceived = 0,
                UnitCost         = i.UnitCost,
                DiscountRate     = i.DiscountRate,
                DiscountAmount   = discAmt,
                TaxRate          = i.TaxRate,
                TaxAmount        = taxAmt,
                TotalCost        = total,
                BatchNumber      = i.BatchNumber,
                ExpiryDate       = i.ExpiryDate
            });
            sub  += i.UnitCost * i.Quantity;
            disc += discAmt;
            tax  += taxAmt;
        }

        var poNumber = await _uow.Purchases.GeneratePurchaseNumberAsync(ct);
        decimal grandTotal = sub - disc + tax - dto.DiscountAmount;

        var purchase = new Purchase
        {
            PurchaseNumber       = poNumber,
            SupplierId           = dto.SupplierId,
            SupplierInvoiceNumber= dto.SupplierInvoiceNumber,
            PurchaseDate         = dto.PurchaseDate,
            ExpectedDate         = dto.ExpectedDate,
            SubTotal             = sub,
            DiscountAmount       = disc + dto.DiscountAmount,
            TaxAmount            = tax,
            TotalAmount          = grandTotal,
            RemainingBalance     = grandTotal,
            Status               = PurchaseStatus.Ordered,
            Notes                = dto.Notes,
            CreatedByUserId      = userId,
            Items                = items
        };

        await _uow.Purchases.AddAsync(purchase, ct);
        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Purchase {Number} created. Total={Total}", poNumber, grandTotal);

        var result = await _uow.Purchases.GetWithItemsAsync(purchase.Id, ct);
        return ApiResponse<PurchaseResponseDto>.Created(_mapper.Map<PurchaseResponseDto>(result));
    }

    public async Task<ApiResponse<PurchaseResponseDto>> ReceiveAsync(ReceivePurchaseDto dto, string userId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var purchase = await _uow.Purchases.GetWithItemsAsync(dto.PurchaseId, ct)
                           ?? throw new NotFoundException(nameof(Purchase), dto.PurchaseId);

            if (purchase.Status == PurchaseStatus.Cancelled)
                throw new InvalidOperationDomainException("Cannot receive a cancelled purchase.");

            foreach (var receiveItem in dto.Items)
            {
                var poItem = purchase.Items.FirstOrDefault(i => i.Id == receiveItem.PurchaseItemId)
                             ?? throw new BusinessRuleException($"PurchaseItem {receiveItem.PurchaseItemId} not found.");

                int remaining = poItem.QuantityOrdered - poItem.QuantityReceived;
                if (receiveItem.QuantityReceived > remaining)
                    throw new BusinessRuleException($"Cannot receive more than ordered. Remaining: {remaining}.");

                poItem.QuantityReceived += receiveItem.QuantityReceived;

                var med = await _uow.Medicines.GetByIdAsync(poItem.MedicineId, ct)
                          ?? throw new NotFoundException(nameof(Medicine), poItem.MedicineId);

                int before = med.QuantityInStock;
                med.QuantityInStock += receiveItem.QuantityReceived;
                if (med.Status == MedicineStatus.OutOfStock) med.Status = MedicineStatus.Active;
                if (poItem.ExpiryDate.HasValue) med.ExpiryDate = poItem.ExpiryDate;
                med.UpdatedAt = DateTime.UtcNow;
                _uow.Medicines.Update(med);

                await _uow.InventoryAudits.AddAsync(new InventoryAudit
                {
                    MedicineId        = med.Id,
                    ActionType        = AuditActionType.StockIn,
                    QuantityChanged   = receiveItem.QuantityReceived,
                    QuantityBefore    = before,
                    QuantityAfter     = med.QuantityInStock,
                    BatchNumber       = poItem.BatchNumber,
                    ExpiryDate        = poItem.ExpiryDate,
                    ReferenceDocument = purchase.PurchaseNumber,
                    PerformedByUserId = userId
                }, ct);
            }

            bool allReceived = purchase.Items.All(i => i.QuantityReceived >= i.QuantityOrdered);
            purchase.Status       = allReceived ? PurchaseStatus.Received : PurchaseStatus.PartiallyReceived;
            purchase.ReceivedDate = DateTime.UtcNow;
            purchase.UpdatedAt    = DateTime.UtcNow;
            _uow.Purchases.Update(purchase);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            var result = await _uow.Purchases.GetWithItemsAsync(purchase.Id, ct);
            return ApiResponse<PurchaseResponseDto>.Ok(_mapper.Map<PurchaseResponseDto>(result));
        }
        catch { await _uow.RollbackTransactionAsync(ct); throw; }
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, CancellationToken ct = default)
    {
        var purchase = await _uow.Purchases.GetByIdAsync(id, ct)
                       ?? throw new NotFoundException(nameof(Purchase), id);
        if (purchase.Status == PurchaseStatus.Received)
            throw new InvalidOperationDomainException("Cannot cancel an already received purchase.");
        purchase.Status    = PurchaseStatus.Cancelled;
        purchase.UpdatedAt = DateTime.UtcNow;
        _uow.Purchases.Update(purchase);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true);
    }

    public async Task<ApiResponse<List<PurchaseListDto>>> GetBySupplierAsync(int supplierId, CancellationToken ct = default)
    {
        var list = await _uow.Purchases.GetBySupplierAsync(supplierId, ct);
        return ApiResponse<List<PurchaseListDto>>.Ok(_mapper.Map<List<PurchaseListDto>>(list));
    }
}
