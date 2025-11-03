namespace Services.Application.DTOs;

public class CSLImageDto
{
    public int CustomerId { get; set; }
    public int AppointmentId { get; set; }
    public int CSLId { get; set; }
    public string CompanyId { get; set; } = string.Empty;
    public string TagName { get; set; } = string.Empty;
    public List<ImageDto>? ImageList { get; set; }
}

public class ImageDto
{
    public string ImageName { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

