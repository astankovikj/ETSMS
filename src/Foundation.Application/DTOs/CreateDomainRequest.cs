using System.ComponentModel.DataAnnotations;

namespace Foundation.Application.DTOs;

public sealed class CreateDomainRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MinLength(1)]
    [MaxLength(50)]
    public string Type { get; init; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; init; }
}
