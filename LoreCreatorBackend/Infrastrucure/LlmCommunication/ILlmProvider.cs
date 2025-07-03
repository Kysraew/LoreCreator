using LoreCreatorBackend.Models;

namespace LoreCreatorBackend.Infrastrucure.LlmCommunication
{
  interface ILlmProvider
  {
    public Task<EntityDto> GetGeneratedEntityAsync(WorldSetting worldSetting, ICollection<EntityType> entityTypes, ICollection<Entity> RelavantEntities);
  }
}