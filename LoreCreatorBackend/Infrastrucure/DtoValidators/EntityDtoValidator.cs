using System.Linq;
using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Infrastrucure.LlmCommunication;
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Infrastrucure.DtoValidators
{
  static class EntityDtoValidator
  {
    public static async Task<Entity> GetNewValidEntity(LoreDbContext db, EntityDto entityDto, int worldId)
    {
      var existing = await db.Entities
      .SingleOrDefaultAsync(e => e.Name == entityDto.Name && e.WorldId == worldId);

      if (existing != null && !string.IsNullOrWhiteSpace(existing.Description))
        throw new EntityDtoInvalidException(
            $"Entity with name '{entityDto.Name}' already in database");

      var type = await db.EntityTypes
              .SingleOrDefaultAsync(t => t.Name == entityDto.EntityType)
              ?? throw new EntityDtoInvalidException(
                  $"Entity type '{entityDto.EntityType}' not in database");

      var relatedNames = entityDto.RelatedEntities.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

      var related = await db.Entities
      .Where(e => relatedNames.Contains(e.Name) && e.WorldId == worldId)
      .ToListAsync();

      foreach (var relDto in entityDto.RelatedEntities)
      {
        if (related.All(r => r.Name != relDto.Name))
        {
          var relType = await db.EntityTypes
              .SingleOrDefaultAsync(t => t.Name == relDto.EntityType)
              ?? throw new EntityDtoInvalidException(
                  $"Entity type '{entityDto.EntityType}' not in database");

          var placeholder = new Entity
          {
            WorldId = worldId,
            Name = relDto.Name,
            Description = null,
            EntityType = relType
          };
          db.Entities.Add(placeholder);
          related.Add(placeholder);
        }
      }

      Entity entity = new Entity
      {
        WorldId = worldId,
        Name = entityDto.Name,
        Description = entityDto.Description,
        EntityType = type,
        RelatedEntities = related
      };

      return entity;
    }

    public static async Task<Entity> GetUpdatedValidEntity(LoreDbContext db, EntityDto entityDto, int worldId)
    {
      var existing = await db.Entities
      .SingleOrDefaultAsync(e => e.Name == entityDto.Name && e.WorldId == worldId)
      ?? throw new EntityDtoInvalidException(
            $"Entity with name '{entityDto.Name}' not in database");

      var type = await db.EntityTypes
              .SingleOrDefaultAsync(t => t.Name == entityDto.EntityType)
              ?? throw new EntityDtoInvalidException(
                  $"Entity type '{entityDto.EntityType}' not in database");

      var relatedNames = entityDto.RelatedEntities.Select(r => r.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

      var related = await db.Entities
      .Where(e => relatedNames.Contains(e.Name) && e.WorldId == worldId)
      .ToListAsync();

      foreach (var relDto in entityDto.RelatedEntities)
      {
        if (related.All(r => r.Name != relDto.Name))
        {
          var relType = await db.EntityTypes
              .SingleOrDefaultAsync(t => t.Name == relDto.EntityType)
              ?? throw new EntityDtoInvalidException(
                  $"Entity type '{entityDto.EntityType}' not in database");

          var placeholder = new Entity
          {
            WorldId = worldId,
            Name = relDto.Name,
            Description = null,
            EntityType = relType
          };
          db.Entities.Add(placeholder);
          related.Add(placeholder);
        }
      }

      Entity entity = new Entity
      {
        WorldId = worldId,
        Name = entityDto.Name,
        Description = entityDto.Description,
        EntityType = type,
        RelatedEntities = related
      };

      return entity;
    }
  }
}