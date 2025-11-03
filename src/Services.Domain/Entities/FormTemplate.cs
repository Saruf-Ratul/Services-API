namespace Services.Domain.Entities;

public class FormTemplate
{
    public int Id { get; set; }
    public string CompanyID { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsAutoAssignEnabled { get; set; }
    public string AutoAssignServiceTypes { get; set; } = string.Empty;
    public string FormStructure { get; set; } = string.Empty;
    public bool RequireSignature { get; set; }
    public bool RequireTip { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDateTime { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedDateTime { get; set; }
}

