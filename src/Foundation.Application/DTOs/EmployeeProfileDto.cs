namespace Foundation.Application.DTOs;

public sealed class EmployeeProfileDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string JobTitle { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string ProfilePictureUrl { get; init; } = string.Empty;
    public string Bio { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
}
