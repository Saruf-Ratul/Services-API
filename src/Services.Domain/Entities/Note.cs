namespace Services.Domain.Entities;

public class Note
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public int? CSLId { get; set; }
    public int? CustomerId { get; set; }
    public int? AppointmentId { get; set; }
    public string CompanyId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public int? TagId { get; set; }
    public string UserName { get; set; } = string.Empty;
}

