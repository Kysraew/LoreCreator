
namespace LoreCreatorBackend.Infrastrucure.LlmCommunication
{
  public class EntityDto
  {
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public List<RelatedEntityDto> RelatedEntities { get; set; } = new();
  }
}
