using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacySystem.Application.Interfaces;

namespace PharmacySystem.API.Controllers;

/// <summary>Reporting and analytics — dashboard, sales, purchases, profit, exports.</summary>
[Authorize]
public class ReportsController : BaseApiController
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    /// <summary>
    /// Get the main dashboard: today's sales, monthly KPIs,
    /// low-stock alerts, expiry alerts, recent invoices.
    /// </summary>
    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var result = await _service.GetDashboardAsync(ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get all sales for a specific date.</summary>
    [HttpGet("daily-sales")]
    public async Task<IActionResult> DailySales([FromQuery] DateTime date, CancellationToken ct)
    {
        var result = await _service.GetDailySalesAsync(date, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get sales summary over a date range.</summary>
    [HttpGet("sales-summary")]
    public async Task<IActionResult> SalesSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetSalesSummaryAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get purchase orders report. [Admin only]</summary>
    [HttpGet("purchases")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PurchasesReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetPurchasesReportAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get expenses report. [Admin only]</summary>
    [HttpGet("expenses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExpensesReport(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetExpensesReportAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get revenue vs expenses profit summary. [Admin only]</summary>
    [HttpGet("profit")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ProfitSummary(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var result = await _service.GetProfitSummaryAsync(from, to, ct);
        return StatusCode(result.StatusCode, result);
    }

    /// <summary>Get top-selling medicines by revenue. [Admin only]</summary>
    [HttpGet("top-medicines")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> TopMedicines(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        [FromQuery] int count = 10,
        CancellationToken ct = default)
    {
        var result = await _service.GetTopMedicinesAsync(from, to, count, ct);
        return StatusCode(result.StatusCode, result);
    }

    // ── Export endpoints ──────────────────────────────────────────────────────

    /// <summary>Export sales to Excel file. [Admin only]</summary>
    [HttpGet("export/sales/excel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportSalesExcel(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var bytes = await _service.ExportSalesToExcelAsync(from, to, ct);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Sales_{from:yyyyMMdd}_{to:yyyyMMdd}.xlsx");
    }

    /// <summary>Export sales to PDF file. [Admin only]</summary>
    [HttpGet("export/sales/pdf")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportSalesPdf(
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken ct)
    {
        var bytes = await _service.ExportSalesToPdfAsync(from, to, ct);
        return File(bytes, "application/pdf", $"Sales_{from:yyyyMMdd}_{to:yyyyMMdd}.pdf");
    }

    /// <summary>Export current inventory to Excel. [Admin only]</summary>
    [HttpGet("export/inventory/excel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportInventoryExcel(CancellationToken ct)
    {
        var bytes = await _service.ExportInventoryToExcelAsync(ct);
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Inventory_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

    /// <summary>Export full JSON backup. [Admin only]</summary>
    [HttpGet("export/backup")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportBackup(CancellationToken ct)
    {
        var path = await _service.ExportFullBackupAsync(ct);
        return Ok(new { BackupPath = path, GeneratedAt = DateTime.UtcNow });
    }
}
