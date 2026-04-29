using PharmacySystem.Application.Common;
using PharmacySystem.Application.DTOs.Reports;

namespace PharmacySystem.Application.Interfaces;

public interface IReportService
{
    Task<ApiResponse<DashboardDto>>       GetDashboardAsync(CancellationToken ct = default);
    Task<ApiResponse<DailySalesReportDto>> GetDailySalesAsync(DateTime date, CancellationToken ct = default);
    Task<ApiResponse<SalesSummaryDto>>    GetSalesSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse<PurchasesReportDto>> GetPurchasesReportAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse<ExpensesReportDto>>  GetExpensesReportAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse<ProfitSummaryDto>>   GetProfitSummaryAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<ApiResponse<List<TopMedicineDto>>> GetTopMedicinesAsync(DateTime from, DateTime to, int count = 10, CancellationToken ct = default);

    // ── Export ───────────────────────────────────────────────────────────────
    Task<byte[]> ExportSalesToExcelAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<byte[]> ExportSalesToPdfAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<byte[]> ExportInventoryToExcelAsync(CancellationToken ct = default);
    Task<string> ExportFullBackupAsync(CancellationToken ct = default);  // JSON backup path
}
