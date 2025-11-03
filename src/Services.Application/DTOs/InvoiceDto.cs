namespace Services.Application.DTOs;

public class InvoiceDto
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string CompanyID { get; set; } = string.Empty;
    public string CompnyID { get; set; } = string.Empty;
    public string DisplayNumber { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string CreatedDate { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string InvoiceDate { get; set; } = string.Empty;
    public decimal AmountCollect { get; set; }
    public string TaxType { get; set; } = string.Empty;
    public string AppointmentId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int QboId { get; set; }
    public string DiscountRate { get; set; } = string.Empty;
    public string DiscountOption { get; set; } = string.Empty;
    public int? QboEstimateId { get; set; }
    public string ExpirationDate { get; set; } = string.Empty;
    public string SyncToken { get; set; } = string.Empty;
    public string QboPaymentID { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public string LoanStatus { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public string ConvertedInvocieID { get; set; } = string.Empty;
    public string ConvertedInvocieNumber { get; set; } = string.Empty;
    public string RequestedDepositAmount { get; set; } = string.Empty;
    public string RequestedDepositPercentage { get; set; } = string.Empty;
    public int RequestedAmtType { get; set; }
    public List<InvoiceItemDto>? Items { get; set; }
}

public class InvoiceItemDto
{
    public string ItemId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Quantity { get; set; } = string.Empty;
    public string UnitPrice { get; set; } = string.Empty;
    public string TotalPrice { get; set; } = string.Empty;
    public string IsTaxable { get; set; } = string.Empty;
    public string ItemTyId { get; set; } = string.Empty;
}

public class InvoiceEditDto
{
    public string InvoiceID { get; set; } = string.Empty;
    public string CompnyID { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string DisplayNumber { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public string InvoiceType { get; set; } = string.Empty;
    public string ModifiedDate { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string InvoiceDate { get; set; } = string.Empty;
    public decimal AmountCollect { get; set; }
    public string Type { get; set; } = string.Empty;
    public string DiscountRate { get; set; } = string.Empty;
    public string DiscountOption { get; set; } = string.Empty;
    public string ExpirationDate { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public string LoanStatus { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public string ConvertedInvocieID { get; set; } = string.Empty;
    public string ConvertedInvocieNumber { get; set; } = string.Empty;
    public string TaxType { get; set; } = string.Empty;
    public string RequestedDepositAmount { get; set; } = string.Empty;
    public string RequestedDepositPercentage { get; set; } = string.Empty;
    public int RequestedAmtType { get; set; }
    public List<InvoiceItemDto>? Items { get; set; }
}

