using AutoMapper;
using Microsoft.Extensions.Logging;
using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Medicine;
using PharmacySystem.Application.DTOs.Reports;
using PharmacySystem.Application.Interfaces;
using PharmacySystem.Domain.Interfaces;

namespace PharmacySystem.Application.Services;

public class ReportService : IReportService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper     _mapper;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IUnitOfWork uow, IMapper mapper, ILogger<ReportService> logger)
    { _uow = uow; _mapper = mapper; _logger = logger; }

    public async Task<ApiResponse<DashboardDto>> GetDashboardAsync(CancellationToken ct = default)
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd   = todayStart.AddDays(1);
        var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var todayInvoices  = await _uow.Invoices.GetByDateRangeAsync(todayStart, todayEnd, ct);
        var monthInvoices  = await _uow.Invoices.GetByDateRangeAsync(monthStart, todayEnd, ct);
        var monthPurchases = await _uow.Purchases.GetPagedAsync(1, 9999, p => !p.IsDeleted && p.PurchaseDate >= monthStart, ct);
        var monthExpenses  = await _uow.Expenses.GetByDateRangeAsync(monthStart, todayEnd, ct);
        var lowStock       = await _uow.Medicines.GetLowStockAsync(ct);
        var nearExpiry     = await _uow.Medicines.GetNearExpiryAsync(30, ct);
        var expired        = await _uow.Medicines.FindAsync(m => !m.IsDeleted && m.ExpiryDate.HasValue && m.ExpiryDate.Value < DateTime.UtcNow, ct);
        var outOfStock     = await _uow.Medicines.FindAsync(m => !m.IsDeleted && m.QuantityInStock == 0, ct);
        var recentInvoices = await _uow.Invoices.GetByDateRangeAsync(todayStart.AddDays(-7), todayEnd, ct);

        decimal monthRevenue  = monthInvoices.Sum(i => i.TotalAmount);
        decimal monthCOGS     = 0; // computed from invoice items in a real implementation
        decimal monthPurchAmt = monthPurchases.Items.Sum(p => p.TotalAmount);
        decimal monthExpAmt   = monthExpenses.Sum(e => e.Amount);

        return ApiResponse<DashboardDto>.Ok(new DashboardDto
        {
            TodayTotalSales        = todayInvoices.Sum(i => i.TotalAmount),
            TodayInvoiceCount      = todayInvoices.Count,
            TodayTotalReturns      = 0,
            MonthTotalSales        = monthRevenue,
            MonthTotalPurchases    = monthPurchAmt,
            MonthTotalExpenses     = monthExpAmt,
            MonthGrossProfit       = monthRevenue - monthCOGS,
            MonthNetProfit         = monthRevenue - monthCOGS - monthExpAmt,
            LowStockCount          = lowStock.Count,
            ExpiringIn30DaysCount  = nearExpiry.Count,
            ExpiredCount           = expired.Count,
            OutOfStockCount        = outOfStock.Count,
            LowStockAlerts         = _mapper.Map<List<MedicineListDto>>(lowStock.Take(10)),
            ExpiringAlerts         = _mapper.Map<List<MedicineListDto>>(nearExpiry.Take(10)),
            RecentInvoices         = recentInvoices.Take(10).Select(i => new DTOs.Invoice.InvoiceListDto
            {
                Id            = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate   = i.InvoiceDate,
                CustomerName  = i.Customer?.Name,
                TotalAmount   = i.TotalAmount,
                Status        = i.Status.ToString(),
                ItemCount     = i.Items.Count
            }).ToList()
        });
    }

    public async Task<ApiResponse<DailySalesReportDto>> GetDailySalesAsync(DateTime date, CancellationToken ct = default)
    {
        var from    = date.Date.ToUniversalTime();
        var to      = from.AddDays(1);
        var invoices= await _uow.Invoices.GetByDateRangeAsync(from, to, ct);

        return ApiResponse<DailySalesReportDto>.Ok(new DailySalesReportDto
        {
            Date           = date.Date,
            TotalInvoices  = invoices.Count,
            TotalRevenue   = invoices.Sum(i => i.TotalAmount),
            TotalDiscount  = invoices.Sum(i => i.DiscountAmount),
            TotalTax       = invoices.Sum(i => i.TaxAmount),
            NetRevenue     = invoices.Sum(i => i.TotalAmount - i.DiscountAmount),
            TotalReturns   = 0,
            NetSales       = invoices.Sum(i => i.TotalAmount),
            Invoices       = invoices.Select(i => new DTOs.Invoice.InvoiceListDto
            {
                Id            = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceDate   = i.InvoiceDate,
                CustomerName  = i.Customer?.Name,
                TotalAmount   = i.TotalAmount,
                Status        = i.Status.ToString(),
                ItemCount     = i.Items.Count
            }).ToList()
        });
    }

    public async Task<ApiResponse<SalesSummaryDto>> GetSalesSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var invoices = await _uow.Invoices.GetByDateRangeAsync(from.ToUniversalTime(), to.ToUniversalTime(), ct);

        var byPayment = invoices
            .GroupBy(i => i.PaymentMethod.ToString())
            .ToDictionary(g => g.Key, g => g.Sum(i => i.TotalAmount));

        return ApiResponse<SalesSummaryDto>.Ok(new SalesSummaryDto
        {
            From           = from,
            To             = to,
            TotalInvoices  = invoices.Count,
            TotalRevenue   = invoices.Sum(i => i.TotalAmount),
            TotalDiscount  = invoices.Sum(i => i.DiscountAmount),
            TotalTax       = invoices.Sum(i => i.TaxAmount),
            NetRevenue     = invoices.Sum(i => i.TotalAmount),
            TotalReturns   = 0,
            ByPaymentMethod= byPayment
        });
    }

    public async Task<ApiResponse<PurchasesReportDto>> GetPurchasesReportAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var (purchases, _) = await _uow.Purchases.GetPagedAsync(1, 9999,
            p => !p.IsDeleted && p.PurchaseDate >= from.ToUniversalTime() && p.PurchaseDate <= to.ToUniversalTime(), ct);

        return ApiResponse<PurchasesReportDto>.Ok(new PurchasesReportDto
        {
            From         = from,
            To           = to,
            TotalOrders  = purchases.Count,
            TotalAmount  = purchases.Sum(p => p.TotalAmount),
            TotalPaid    = purchases.Sum(p => p.AmountPaid),
            TotalUnpaid  = purchases.Sum(p => p.RemainingBalance),
            TopItems     = []
        });
    }

    public async Task<ApiResponse<ExpensesReportDto>> GetExpensesReportAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var expenses = await _uow.Expenses.GetByDateRangeAsync(from.ToUniversalTime(), to.ToUniversalTime(), ct);

        var byCategory = expenses
            .GroupBy(e => e.ExpenseCategory?.Name ?? "Uncategorized")
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Amount));

        return ApiResponse<ExpensesReportDto>.Ok(new ExpensesReportDto
        {
            From        = from,
            To          = to,
            TotalAmount = expenses.Sum(e => e.Amount),
            ByCategory  = byCategory
        });
    }

    public async Task<ApiResponse<ProfitSummaryDto>> GetProfitSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default)
    {
        var invoices = await _uow.Invoices.GetByDateRangeAsync(from.ToUniversalTime(), to.ToUniversalTime(), ct);
        var expenses = await _uow.Expenses.GetByDateRangeAsync(from.ToUniversalTime(), to.ToUniversalTime(), ct);

        decimal revenue  = invoices.Sum(i => i.TotalAmount);
        decimal expTotal = expenses.Sum(e => e.Amount);
        decimal gross    = revenue; // COGS not tracked separately here — extend in Step 4 with EF projection
        decimal net      = gross - expTotal;

        return ApiResponse<ProfitSummaryDto>.Ok(new ProfitSummaryDto
        {
            From                  = from,
            To                    = to,
            TotalRevenue          = revenue,
            TotalCOGS             = 0,
            GrossProfit           = gross,
            GrossProfitMargin     = revenue == 0 ? 0 : Math.Round(gross / revenue * 100, 2),
            TotalOperatingExpenses= expTotal,
            NetProfit             = net,
            NetProfitMargin       = revenue == 0 ? 0 : Math.Round(net / revenue * 100, 2)
        });
    }

    public async Task<ApiResponse<List<TopMedicineDto>>> GetTopMedicinesAsync(DateTime from, DateTime to, int count = 10, CancellationToken ct = default)
    {
        var invoices = await _uow.Invoices.GetByDateRangeAsync(from.ToUniversalTime(), to.ToUniversalTime(), ct);

        var topMedicines = invoices
            .SelectMany(i => i.Items)
            .GroupBy(item => new { item.MedicineId, item.Medicine?.Name })
            .Select(g => new TopMedicineDto
            {
                MedicineId    = g.Key.MedicineId,
                MedicineName  = g.Key.Name ?? string.Empty,
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalRevenue  = g.Sum(i => i.TotalAmount),
                TotalProfit   = 0
            })
            .OrderByDescending(t => t.TotalRevenue)
            .Take(count)
            .ToList();

        return ApiResponse<List<TopMedicineDto>>.Ok(topMedicines);
    }

    // ── Export stubs (implemented fully in Step 8 with ClosedXML + QuestPDF) ─

    public Task<byte[]> ExportSalesToExcelAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());

    public Task<byte[]> ExportSalesToPdfAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());

    public Task<byte[]> ExportInventoryToExcelAsync(CancellationToken ct = default)
        => Task.FromResult(Array.Empty<byte>());

    public Task<string> ExportFullBackupAsync(CancellationToken ct = default)
        => Task.FromResult(string.Empty);
}
