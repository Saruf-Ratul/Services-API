namespace Services.Domain.Entities;

public class Status
{
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public string CompanyId { get; set; } = string.Empty;
}

