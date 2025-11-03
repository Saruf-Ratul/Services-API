namespace Services.Application.DTOs;

public class ResponseDto
{
    public bool IsValid { get; set; } = false;
    public string Message { get; set; } = string.Empty;
    public string CompanyID { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyTag { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class StringResultDto
{
    public string Status { get; set; } = string.Empty;
    public string Response { get; set; } = string.Empty;
}

public class LoginRequestDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DeviceID { get; set; }
    public string? CompanyID { get; set; }
    public int AppType { get; set; }
}

