namespace Services.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string CompanyID { get; set; } = string.Empty;
    public bool IsCECAppUser { get; set; }
    public bool IsCECProAppUser { get; set; }
    public bool IsMfaEnabled { get; set; }
    public string? MfaSecretKey { get; set; }
    public List<string>? RecoveryCodes { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Company
{
    public int CompanyIdInt { get; set; }
    public string CompanyID { get; set; } = string.Empty;
    public string CompanyGUID { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyTag { get; set; } = string.Empty;
    public string? TimeZone { get; set; }
}

