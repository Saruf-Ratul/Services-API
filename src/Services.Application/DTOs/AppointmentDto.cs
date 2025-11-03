namespace Services.Application.DTOs;

public class AppointmentDto
{
    public string CompanyID { get; set; } = string.Empty;
    public int ApptID { get; set; }
    public string AppoinmentUId { get; set; } = string.Empty;
    public int CustomerID { get; set; }
    public string ServiceTypeId { get; set; } = string.Empty;
    public int? ResourceID { get; set; }
    public int? TimeSlotId { get; set; }
    public DateTime ApptDateTime { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string TimeSlot { get; set; } = string.Empty;
    public string Note { get; set; } = string.Empty;
    public string StatusId { get; set; } = string.Empty;
    public string TicketStatusId { get; set; } = string.Empty;
    public string PromoCode { get; set; } = string.Empty;
    public string UserID { get; set; } = string.Empty;
}

