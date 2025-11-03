namespace Services.Application.DTOs;

public class PaymentDto
{
    public string CompanyID { get; set; } = string.Empty;
    public string InvocieId { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string CheckName { get; set; } = string.Empty;
    public string CheckNumber { get; set; } = string.Empty;
}

