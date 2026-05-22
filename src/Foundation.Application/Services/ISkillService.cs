using Foundation.Application.DTOs;

namespace Foundation.Application.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> SearchSkillsAsync(string query, CancellationToken cancellationToken = default);
    Task<AddEmployeeSkillResult> AddEmployeeSkillAsync(Guid employeeId, AddEmployeeSkillDto dto, CancellationToken cancellationToken = default);
}

public enum AddEmployeeSkillResultStatus
{
    Created,
    Duplicate,
    SkillNotFound,
    ValidationError
}

public sealed class AddEmployeeSkillResult
{
    public AddEmployeeSkillResultStatus Status { get; init; }
    public EmployeeSkillDto? EmployeeSkill { get; init; }
    public string? ErrorMessage { get; init; }

    public static AddEmployeeSkillResult Success(EmployeeSkillDto employeeSkill) =>
        new() { Status = AddEmployeeSkillResultStatus.Created, EmployeeSkill = employeeSkill };

    public static AddEmployeeSkillResult Conflict(string errorMessage) =>
        new() { Status = AddEmployeeSkillResultStatus.Duplicate, ErrorMessage = errorMessage };

    public static AddEmployeeSkillResult NotFound(string errorMessage) =>
        new() { Status = AddEmployeeSkillResultStatus.SkillNotFound, ErrorMessage = errorMessage };

    public static AddEmployeeSkillResult Invalid(string errorMessage) =>
        new() { Status = AddEmployeeSkillResultStatus.ValidationError, ErrorMessage = errorMessage };
}
