namespace LoreCreatorBackend.Models
{
  public class Entity
  {
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ICollection<Entity> RelatedEntities { get; set; } = null!;
    public int EntityTypeId { get; set; }
    public EntityType EntityType { get; set; } = null!;
  }
}