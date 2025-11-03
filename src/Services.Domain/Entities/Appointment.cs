namespace Services.Domain.Entities;

public class Appointment
{
    public string CompanyID { get; set; } = string.Empty;
    public int ApptID { get; set; }
    public string AppoinmentUId { get; set; } = string.Empty;
    public int CustomerID { get; set; }
    public string ServiceTypeId { get; set; } = string.Empty;
    public int? ResourceID { get; set; }
    public int? TimeSlotId { get; set; }
    public string ApptDateTime { get; set; } = string.Empty;
    public string StartDateTime { get; set; } = string.Empty;
    public string EndDateTime { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string StatusId { get; set; } = string.Empty;
    public string TicketStatusId { get; set; } = string.Empty;
    public string CreatedDateTime { get; set; } = string.Empty;
    public bool? MarkDownloaded { get; set; } = false;
    public string PromoCode { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;
    
    // Navigation properties
    public Resource? Resource { get; set; }
    public Customer? Customer { get; set; }
    public Status? Status { get; set; }
    public TicketStatus? TicketStatus { get; set; }
    public ServiceType? ServiceType { get; set; }
    public List<AppointmentInvoice>? Invoices { get; set; }
}

