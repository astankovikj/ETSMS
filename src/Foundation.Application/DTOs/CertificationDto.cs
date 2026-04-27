namespace Foundation.Application.DTOs;

public sealed class CertificationDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}
