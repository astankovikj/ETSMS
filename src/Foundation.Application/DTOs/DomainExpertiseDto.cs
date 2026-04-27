namespace Foundation.Application.DTOs;

public sealed class DomainExpertiseDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime LastUpdatedAt { get; init; }
}
