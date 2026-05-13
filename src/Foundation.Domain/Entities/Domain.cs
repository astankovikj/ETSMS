namespace Foundation.Domain.Entities;

public sealed class Domain
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DomainType DomainType { get; set; }
}
