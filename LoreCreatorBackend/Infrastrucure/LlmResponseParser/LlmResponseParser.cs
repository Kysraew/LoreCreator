using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Infrastrucure.LlmCommunication;
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Infrastrucure.LlmResponseParser
{
  class LLResponseParser
  {
    public async Task<Entity> GetValidEntityDto(LoreDbContext db, EntityDto entityDto)
    {
      var entitySameName = await db.Entities
        .SingleOrDefaultAsync(t => t.Name == entityDto.Name)
        ?? throw new LlmResponseNotAlignedWithDatabaseExtension(
            $"Entity with name '{entityDto.Name}' already in database");

      var type = await db.EntityTypes
              .SingleOrDefaultAsync(t => t.Name == entityDto.EntityType)
              ?? throw new LlmResponseNotAlignedWithDatabaseExtension(
                  $"Entity type '{entityDto.EntityType}' not in database");

      var related = await db.Entities
          .Where(e => entityDto.RelatedEntities.Contains(e.Name))
          .ToListAsync();

      var missing = entityDto.RelatedEntities.Except(related.Select(r => r.Name)).ToList();
      if (missing.Any())
        throw new LlmResponseNotAlignedWithDatabaseExtension(
            $"Entities not in database: {string.Join(", ", missing)}");

      var entity = new Entity
      {
        Name = entityDto.Name,
        Description = entityDto.Description,
        EntityType = type,
        RelatedEntities = related
      };

      return entity;
    }
  }
}