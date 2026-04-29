using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Invoice;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Enums;
using PharmacySystem.Domain.Exceptions;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<InvoiceService> _logger;

    public InvoiceService(IUnitOfWork uow, IMapper mapper, ILogger<InvoiceService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<PagedResult<InvoiceListDto>>> GetPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var (items, total) = await _uow.Invoices.GetPagedAsync(request.PageNumber, request.PageSize, i => !i.IsDeleted, ct);
        return ApiResponse<PagedResult<InvoiceListDto>>.Ok(
            PagedResult<InvoiceListDto>.Create(_mapper.Map<List<InvoiceListDto>>(items), total, request.PageNumber, request.PageSize));
    }

    public async Task<ApiResponse<InvoiceResponseDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Invoices.GetWithItemsAsync(id, ct)
                     ?? throw new NotFoundException(nameof(Invoice), id);
        return ApiResponse<InvoiceResponseDto>.Ok(_mapper.Map<InvoiceResponseDto>(entity));
    }

    public async Task<ApiResponse<InvoiceResponseDto>> GetByNumberAsync(string invoiceNumber, CancellationToken ct = default)
    {
        var entity = await _uow.Invoices.FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber && !i.IsDeleted, ct)
                     ?? throw new NotFoundException(nameof(Invoice), invoiceNumber);
        return ApiResponse<InvoiceResponseDto>.Ok(_mapper.Map<InvoiceResponseDto>(entity));
    }

    public async Task<ApiResponse<InvoiceResponseDto>> CreateAsync(CreateInvoiceDto dto, string userId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var invoiceItems = new List<InvoiceItem>();
            decimal subTotal = 0;

            foreach (var itemDto in dto.Items)
            {
                var medicine = await _uow.Medicines.GetByIdAsync(itemDto.MedicineId, ct)
                               ?? throw new NotFoundException(nameof(Medicine), itemDto.MedicineId);

                if (medicine.ExpiryDate.HasValue && medicine.ExpiryDate.Value < DateTime.UtcNow)
                    throw new BusinessRuleException($"Medicine '{medicine.Name}' is expired and cannot be sold.");

                if (medicine.QuantityInStock < itemDto.Quantity)
                    throw new InsufficientStockException(medicine.Name, itemDto.Quantity, medicine.QuantityInStock);

                decimal discountAmt = medicine.SellingPrice * itemDto.Quantity * itemDto.DiscountRate;
                decimal taxAmt      = (medicine.SellingPrice * itemDto.Quantity - discountAmt) * itemDto.TaxRate;
                decimal lineTotal   = medicine.SellingPrice * itemDto.Quantity - discountAmt + taxAmt;

                var invoiceItem = new InvoiceItem
                {
                    MedicineId     = medicine.Id,
                    Quantity       = itemDto.Quantity,
                    UnitPrice      = medicine.SellingPrice,
                    DiscountRate   = itemDto.DiscountRate,
                    DiscountAmount = discountAmt,
                    TaxRate        = itemDto.TaxRate,
                    TaxAmount      = taxAmt,
                    TotalAmount    = lineTotal,
                    ExpiryDate     = medicine.ExpiryDate
                };
                invoiceItems.Add(invoiceItem);
                subTotal += lineTotal;

                int stockBefore = medicine.QuantityInStock;
                medicine.QuantityInStock -= itemDto.Quantity;
                medicine.UpdatedAt = DateTime.UtcNow;
                if (medicine.QuantityInStock == 0) medicine.Status = MedicineStatus.OutOfStock;
                _uow.Medicines.Update(medicine);

                await _uow.InventoryAudits.AddAsync(new InventoryAudit
                {
                    MedicineId        = medicine.Id,
                    ActionType        = AuditActionType.StockOut,
                    QuantityChanged   = -itemDto.Quantity,
                    QuantityBefore    = stockBefore,
                    QuantityAfter     = medicine.QuantityInStock,
                    PerformedByUserId = userId,
                    ExpiryDate        = medicine.ExpiryDate
                }, ct);
            }

            decimal totalDiscount = dto.GlobalDiscountAmount + invoiceItems.Sum(i => i.DiscountAmount);
            decimal totalTax      = invoiceItems.Sum(i => i.TaxAmount);
            decimal totalAmount   = subTotal - dto.GlobalDiscountAmount;
            decimal change        = dto.AmountPaid - totalAmount;
            var     invoiceNumber = await _uow.Invoices.GenerateInvoiceNumberAsync(ct);

            var invoice = new Invoice
            {
                InvoiceNumber         = invoiceNumber,
                CustomerId            = dto.CustomerId,
                UserId                = userId,
                PaymentMethod         = dto.PaymentMethod,
                Status                = InvoiceStatus.Confirmed,
                SubTotal              = subTotal,
                DiscountAmount        = totalDiscount,
                TaxAmount             = totalTax,
                TotalAmount           = totalAmount,
                AmountPaid            = dto.AmountPaid,
                ChangeAmount          = change < 0 ? 0 : change,
                PrescriptionReference = dto.PrescriptionReference,
                Notes                 = dto.Notes,
                Items                 = invoiceItems
            };

            await _uow.Invoices.AddAsync(invoice, ct);
            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);

            _logger.LogInformation("Invoice {Number} created. Total={Total}", invoiceNumber, totalAmount);
            var result = await _uow.Invoices.GetWithItemsAsync(invoice.Id, ct);
            return ApiResponse<InvoiceResponseDto>.Created(_mapper.Map<InvoiceResponseDto>(result));
        }
        catch { await _uow.RollbackTransactionAsync(ct); throw; }
    }

    public async Task<ApiResponse<bool>> CancelAsync(int id, string userId, CancellationToken ct = default)
    {
        var invoice = await _uow.Invoices.GetByIdAsync(id, ct)
                      ?? throw new NotFoundException(nameof(Invoice), id);
        if (invoice.Status == InvoiceStatus.Cancelled)
            throw new InvalidOperationDomainException("Invoice is already cancelled.");
        invoice.Status    = InvoiceStatus.Cancelled;
        invoice.UpdatedAt = DateTime.UtcNow;
        _uow.Invoices.Update(invoice);
        await _uow.SaveChangesAsync(ct);
        return ApiResponse<bool>.Ok(true, "Invoice cancelled.");
    }

    public async Task<ApiResponse<InvoiceReturnResponseDto>> CreateReturnAsync(CreateInvoiceReturnDto dto, string userId, CancellationToken ct = default)
    {
        await _uow.BeginTransactionAsync(ct);
        try
        {
            var invoice = await _uow.Invoices.GetWithItemsAsync(dto.InvoiceId, ct)
                          ?? throw new NotFoundException(nameof(Invoice), dto.InvoiceId);

            if (invoice.Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationDomainException("Cannot return items of a cancelled invoice.");

            var invoiceItem = invoice.Items.FirstOrDefault(i => i.MedicineId == dto.MedicineId)
                              ?? throw new BusinessRuleException($"Medicine {dto.MedicineId} not found in invoice.");

            var previousReturns = await _uow.InvoiceReturns.GetByInvoiceAsync(dto.InvoiceId, ct);
            int alreadyReturned = previousReturns.Where(r => r.MedicineId == dto.MedicineId).Sum(r => r.Quantity);
            int maxReturnable   = invoiceItem.Quantity - alreadyReturned;

            if (dto.Quantity > maxReturnable)
                throw new BusinessRuleException($"Max returnable quantity is {maxReturnable}.");

            var medicine = await _uow.Medicines.GetByIdAsync(dto.MedicineId, ct)
                           ?? throw new NotFoundException(nameof(Medicine), dto.MedicineId);

            int stockBefore = medicine.QuantityInStock;
            medicine.QuantityInStock += dto.Quantity;
            if (medicine.Status == MedicineStatus.OutOfStock) medicine.Status = MedicineStatus.Active;
            medicine.UpdatedAt = DateTime.UtcNow;
            _uow.Medicines.Update(medicine);

            await _uow.InventoryAudits.AddAsync(new InventoryAudit
            {
                MedicineId        = medicine.Id,
                ActionType        = AuditActionType.ReturnIn,
                QuantityChanged   = dto.Quantity,
                QuantityBefore    = stockBefore,
                QuantityAfter     = medicine.QuantityInStock,
                ReferenceDocument = invoice.InvoiceNumber,
                PerformedByUserId = userId
            }, ct);

            var returnNumber = await _uow.InvoiceReturns.GenerateReturnNumberAsync(ct);
            var refundAmount = invoiceItem.UnitPrice * dto.Quantity * (1 - invoiceItem.DiscountRate) * (1 + invoiceItem.TaxRate);

            var returnEntity = new InvoiceReturn
            {
                ReturnNumber      = returnNumber,
                InvoiceId         = dto.InvoiceId,
                MedicineId        = dto.MedicineId,
                Quantity          = dto.Quantity,
                UnitPrice         = invoiceItem.UnitPrice,
                RefundAmount      = Math.Round(refundAmount, 2),
                Reason            = dto.Reason,
                Notes             = dto.Notes,
                IsRefunded        = true,
                ProcessedByUserId = userId
            };

            await _uow.InvoiceReturns.AddAsync(returnEntity, ct);
            invoice.Status    = InvoiceStatus.Returned;
            invoice.UpdatedAt = DateTime.UtcNow;
            _uow.Invoices.Update(invoice);

            await _uow.SaveChangesAsync(ct);
            await _uow.CommitTransactionAsync(ct);
            return ApiResponse<InvoiceReturnResponseDto>.Created(_mapper.Map<InvoiceReturnResponseDto>(returnEntity));
        }
        catch { await _uow.RollbackTransactionAsync(ct); throw; }
    }

    public async Task<ApiResponse<List<InvoiceListDto>>> GetByCustomerAsync(int customerId, CancellationToken ct = default)
    {
        var list = await _uow.Invoices.FindAsync(i => i.CustomerId == customerId && !i.IsDeleted, ct);
        return ApiResponse<List<InvoiceListDto>>.Ok(_mapper.Map<List<InvoiceListDto>>(list));
    }

    public async Task<ApiResponse<List<InvoiceListDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var list = await _uow.Invoices.GetByDateRangeAsync(from, to, ct);
        return ApiResponse<List<InvoiceListDto>>.Ok(_mapper.Map<List<InvoiceListDto>>(list));
    }
}
