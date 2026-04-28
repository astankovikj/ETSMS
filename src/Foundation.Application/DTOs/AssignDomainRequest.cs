using System.ComponentModel.DataAnnotations;

namespace Foundation.Application.DTOs;

public sealed class AssignDomainRequest
{
    [Required]
    public Guid DomainId { get; init; }
}
