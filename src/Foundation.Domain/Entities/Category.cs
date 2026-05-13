namespace Foundation.Domain.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<Technology> Technologies { get; set; } = new List<Technology>();
}
