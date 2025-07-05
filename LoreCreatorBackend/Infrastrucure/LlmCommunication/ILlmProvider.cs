using LoreCreatorBackend.Models;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication
{
  public interface ILlmProvider
  {
    public Task<EntityDto> GetGeneratedEntityAsync(EntityDto entityDto, World worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities);
    public Task<EntityDto> GetUpdatedEntityAsync(EntityDto entityDto, World worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities);
  }
}