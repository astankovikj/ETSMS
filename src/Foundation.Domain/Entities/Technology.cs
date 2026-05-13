namespace Foundation.Domain.Entities;

public sealed class Technology
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CategoryId { get; set; }

    public Category Category { get; set; } = null!;

    /// <summary>Relationships where this technology is the primary (parent).</summary>
    public ICollection<TechnologyRelationship> PrimaryRelationships { get; set; } = new List<TechnologyRelationship>();

    /// <summary>Relationships where this technology is the secondary (child/specialisation).</summary>
    public ICollection<TechnologyRelationship> SecondaryRelationships { get; set; } = new List<TechnologyRelationship>();
}
