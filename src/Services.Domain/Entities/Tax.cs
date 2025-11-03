namespace Services.Domain.Entities;

public class Tax
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Rate { get; set; }
}

