namespace PharmacySystem.Domain.Enums;

// ──────────────────────────────────────────────
//  Identity / Access
// ──────────────────────────────────────────────

public enum UserRole
{
    Admin   = 1,
    Cashier = 2,
    Viewer  = 3
}

// ──────────────────────────────────────────────
//  Invoice
// ──────────────────────────────────────────────

public enum InvoiceStatus
{
    Draft     = 1,
    Confirmed = 2,
    Returned  = 3,
    Cancelled = 4
}

public enum PaymentMethod
{
    Cash        = 1,
    CreditCard  = 2,
    DebitCard   = 3,
    Insurance   = 4,
    BankTransfer= 5
}

// ──────────────────────────────────────────────
//  Purchase
// ──────────────────────────────────────────────

public enum PurchaseStatus
{
    Ordered           = 1,
    PartiallyReceived = 2,
    Received          = 3,
    Cancelled         = 4
}

// ──────────────────────────────────────────────
//  Medicine / Inventory
// ──────────────────────────────────────────────

public enum MedicineStatus
{
    Active       = 1,
    Discontinued = 2,
    OutOfStock   = 3
}

public enum DosageForm
{
    Tablet    = 1,
    Capsule   = 2,
    Syrup     = 3,
    Injection = 4,
    Cream     = 5,
    Drops     = 6,
    Inhaler   = 7,
    Patch     = 8,
    Other     = 99
}

// ──────────────────────────────────────────────
//  Inventory Audit
// ──────────────────────────────────────────────

public enum AuditActionType
{
    StockIn        = 1,   // Purchase received
    StockOut       = 2,   // Sale issued
    ReturnIn       = 3,   // Customer return
    ReturnOut      = 4,   // Return to supplier
    Adjustment     = 5,   // Manual correction
    ExpiredDisposal= 6    // Expired stock written off
}

// ──────────────────────────────────────────────
//  Invoice Return
// ──────────────────────────────────────────────

public enum ReturnReason
{
    WrongMedicine  = 1,
    Expired        = 2,
    DamagedPackage = 3,
    CustomerRefusal= 4,
    Other          = 99
}
