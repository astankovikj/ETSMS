namespace Foundation.Domain.Entities;

/// <summary>
/// Represents a parent → child (primary → secondary) relationship between two technologies.
/// </summary>
public sealed class TechnologyRelationship
{
    public int PrimaryTechnologyId { get; set; }
    public int SecondaryTechnologyId { get; set; }

    public Technology PrimaryTechnology { get; set; } = null!;
    public Technology SecondaryTechnology { get; set; } = null!;
}
