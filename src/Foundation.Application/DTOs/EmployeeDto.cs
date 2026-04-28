namespace Foundation.Application.DTOs;

public sealed class EmployeeDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public IReadOnlyList<DomainDto> Domains { get; init; } = [];
}
