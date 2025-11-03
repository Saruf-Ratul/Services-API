namespace Services.Domain.Entities;

public class ServiceType
{
    public string CompanyID { get; set; } = string.Empty;
    public int ServiceTypeID { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public string CreatedDateTime { get; set; } = string.Empty;
    public int? Hour { get; set; }
    public int? Minute { get; set; }
    public string CalenderColor { get; set; } = string.Empty;
    public int? ReminderID { get; set; }
    public bool IsInternalUse { get; set; } = false;
}

