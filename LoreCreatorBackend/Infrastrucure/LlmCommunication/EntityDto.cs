
namespace LoreCreatorBackend.Infrastrucure.LlmCommunication
{
  public class EntityDto
  {
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<string> RelatedEntities { get; set; } = new();
  }
}
