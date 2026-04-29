using PharmacySystem.Application.DTOs.Invoice;
using PharmacySystem.Application.DTOs.Medicine;

namespace PharmacySystem.Application.DTOs.Reports;

// ── Dashboard ────────────────────────────────────────────────────────────────

public class DashboardDto
{
    // Today
    public decimal TodayTotalSales       { get; set; }
    public int     TodayInvoiceCount     { get; set; }
    public decimal TodayTotalReturns     { get; set; }

    // Month
    public decimal MonthTotalSales       { get; set; }
    public decimal MonthTotalPurchases   { get; set; }
    public decimal MonthTotalExpenses    { get; set; }
    public decimal MonthGrossProfit      { get; set; }
    public decimal MonthNetProfit        { get; set; }

    // Stock alerts
    public int LowStockCount            { get; set; }
    public int ExpiringIn30DaysCount    { get; set; }
    public int ExpiredCount             { get; set; }
    public int OutOfStockCount          { get; set; }

    // Alert lists
    public List<MedicineListDto> LowStockAlerts     { get; set; } = [];
    public List<MedicineListDto> ExpiringAlerts      { get; set; } = [];

    // Recent activity
    public List<InvoiceListDto>  RecentInvoices      { get; set; } = [];
}

// ── Sales Report ────────────────────────────────────────────────────────────

public class DailySalesReportDto
{
    public DateTime Date             { get; set; }
    public int      TotalInvoices    { get; set; }
    public decimal  TotalRevenue     { get; set; }
    public decimal  TotalDiscount    { get; set; }
    public decimal  TotalTax         { get; set; }
    public decimal  NetRevenue       { get; set; }
    public decimal  TotalReturns     { get; set; }
    public decimal  NetSales         { get; set; }
    public List<InvoiceListDto> Invoices { get; set; } = [];
}

public class SalesSummaryDto
{
    public DateTime From           { get; set; }
    public DateTime To             { get; set; }
    public int      TotalInvoices  { get; set; }
    public decimal  TotalRevenue   { get; set; }
    public decimal  TotalDiscount  { get; set; }
    public decimal  TotalTax       { get; set; }
    public decimal  NetRevenue     { get; set; }
    public decimal  TotalReturns   { get; set; }

    // Breakdown by payment method
    public Dictionary<string, decimal> ByPaymentMethod { get; set; } = [];
}

// ── Purchases Report ────────────────────────────────────────────────────────

public class PurchasesReportDto
{
    public DateTime From               { get; set; }
    public DateTime To                 { get; set; }
    public int      TotalOrders        { get; set; }
    public decimal  TotalAmount        { get; set; }
    public decimal  TotalPaid          { get; set; }
    public decimal  TotalUnpaid        { get; set; }
    public List<PurchaseItemReportDto> TopItems { get; set; } = [];
}

public class PurchaseItemReportDto
{
    public string  MedicineName    { get; set; } = string.Empty;
    public int     TotalQuantity   { get; set; }
    public decimal TotalCost       { get; set; }
}

// ── Expenses Report ────────────────────────────────────────────────────────

public class ExpensesReportDto
{
    public DateTime From                     { get; set; }
    public DateTime To                       { get; set; }
    public decimal  TotalAmount              { get; set; }
    public Dictionary<string, decimal> ByCategory { get; set; } = [];
}

// ── Profit Report ────────────────────────────────────────────────────────────

public class ProfitSummaryDto
{
    public DateTime From                 { get; set; }
    public DateTime To                   { get; set; }
    public decimal  TotalRevenue         { get; set; }
    public decimal  TotalCOGS            { get; set; }  // Cost of Goods Sold
    public decimal  GrossProfit          { get; set; }
    public decimal  GrossProfitMargin    { get; set; }  // %
    public decimal  TotalOperatingExpenses{ get; set; }
    public decimal  NetProfit            { get; set; }
    public decimal  NetProfitMargin      { get; set; }  // %
}

// ── Top Medicines ─────────────────────────────────────────────────────────────

public class TopMedicineDto
{
    public int     MedicineId    { get; set; }
    public string  MedicineName  { get; set; } = string.Empty;
    public int     TotalQuantity { get; set; }
    public decimal TotalRevenue  { get; set; }
    public decimal TotalProfit   { get; set; }
}
