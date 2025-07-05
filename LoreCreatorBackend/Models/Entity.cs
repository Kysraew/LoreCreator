namespace LoreCreatorBackend.Models
{
  public class Entity
  {
    public int Id { get; set; }
    public int WorldId { get; set; }
    public World World { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<Entity> RelatedEntities { get; set; } = null!;
    public int EntityTypeId { get; set; }
    public EntityType EntityType { get; set; } = null!;
  }
}