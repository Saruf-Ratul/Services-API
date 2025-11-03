namespace Services.Domain.Entities;

public class Invoice
{
    public string InvoiceID { get; set; } = string.Empty;
    public Guid CustomerGuid { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string QBOCustomerId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public string QBOId { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string InvoiceDate { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public bool IsConverted { get; set; }
    public string ConvertedInvoiceID { get; set; } = string.Empty;
    public decimal Due { get; set; }
    public decimal AmountCollect { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Surcharge { get; set; }
    public string Note { get; set; } = string.Empty;
}

public class InvoiceItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int ItemTypeId { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public string IsTaxable { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public long? QboId { get; set; }
}

public class InvoiceDetails
{
    public string InvoiceID { get; set; } = string.Empty;
    public Guid CustomerGuid { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string InvoiceDate { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public Invoice? Invoice { get; set; }
    public List<InvoiceItem>? Items { get; set; }
    public List<InvoiceDetailsItemWise>? InvoiceDetailsList { get; set; }
}

public class InvoiceDetailsItemWise
{
    public Guid ID { get; set; } = Guid.NewGuid();
    public int SortId { get; set; }
    public string RefId { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string InvoiceId { get; set; } = string.Empty;
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string LineNum { get; set; } = "0";
    public decimal? Quantity { get; set; }
    public decimal? UPrice { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedDate { get; set; }
    public string ModifiedBy { get; set; } = string.Empty;
    public decimal? TotalPrice { get; set; }
    public string CreatedDate { get; set; } = string.Empty;
    public string ItemTyId { get; set; } = string.Empty;
    public string IsTaxable { get; set; } = string.Empty;
    public string ServiceDate { get; set; } = string.Empty;
}

public class AppointmentInvoice
{
    public string InvoiceID { get; set; } = string.Empty;
    public string CustomerGuid { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string QBOCustomerId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal DepositAmount { get; set; }
    public string City { get; set; } = string.Empty;
    public string QBOId { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string InvoiceDate { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal AmountCollect { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public decimal Tax { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string Due { get; set; } = string.Empty;
    public bool IsConverted { get; set; }
    public string ConvertedInvoiceID { get; set; } = string.Empty;
    public decimal Surcharge { get; set; }
    public string DiscountOption { get; set; } = string.Empty;
    public string TaxType { get; set; } = string.Empty;
    public string RequestedDepositAmount { get; set; } = string.Empty;
    public string RequestedDepositPercentage { get; set; } = string.Empty;
    public int RequestedAmountType { get; set; }
    public List<InvoiceItem>? Items { get; set; }
    public List<Payment>? PaymentList { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public string CompanyId { get; set; } = string.Empty;
    public string InvocieId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CheckName { get; set; } = string.Empty;
    public string CheckNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsDeposit { get; set; }
    public string Source { get; set; } = string.Empty;
    public string CreatedDate { get; set; } = string.Empty;
    public int? QboId { get; set; }
    public string PaymentRefNum { get; set; } = string.Empty;
    public int RMPaymentId { get; set; }
}

public class CSLTag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

public class AppointmnetForm
{
    public int AppointmentId { get; set; }
    public int CustomerId { get; set; }
    public string CompanyId { get; set; } = string.Empty;
    public List<int> FormIds { get; set; } = new();
    public string UserId { get; set; } = string.Empty;
}

public class AppoinmentListWithForms : Appointment
{
    public List<int> FormIds { get; set; } = new();
}

public class Items
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int ItemTypeId { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = string.Empty;
    public bool IsTaxable { get; set; } = false;
    public string CompanyId { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public int? QboId { get; set; }
}

public class EmailContent
{
    public byte[]? FileContent { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}

