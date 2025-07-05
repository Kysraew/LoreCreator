using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Infrastrucure.DtoValidators;
using LoreCreatorBackend.Infrastrucure.LlmCommunication;
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Endpoints
{
  public static class WorldEndpoints
  {
    public static void MapWorldEndpoints(this IEndpointRouteBuilder app)
    {
      RouteGroupBuilder worldSettingApi = app.MapGroup("/worlds");

      worldSettingApi.MapGet("/", GetAllWorld);
      worldSettingApi.MapGet("/{worldId}", GetWorld);
      worldSettingApi.MapGet("by-name/{name}", GetWorldByName);
      worldSettingApi.MapPost("", AddNewWorld);
      worldSettingApi.MapPut("/{worldId}", EditWorld);
      worldSettingApi.MapDelete("/{worldId}", DeleteWorld);

      RouteGroupBuilder entitiesApi = app.MapGroup("/{worldId}/entities");
      entitiesApi.MapGet("/", GetAllEntities);
      entitiesApi.MapGet("/{entityId}", GetEntity);
      entitiesApi.MapGet("by-name/{name}", GetEntityByName);
      entitiesApi.MapPost("add", AddNewEntity);
      entitiesApi.MapPost("generate", GenerateNewEntity);
      entitiesApi.MapPut("update/{entityId}", EditEntity);
      entitiesApi.MapPut("regenerate/{entityId}", GenerateUpdatedEntity);
      entitiesApi.MapDelete("/{entityId}", DeleteEntity);
    }


    //world
    public static async Task<IResult> GetAllWorld(LoreDbContext db)
    {
      ICollection<World> entities = await db.Worlds.ToListAsync();
      return TypedResults.Ok(entities);
    }

    public static async Task<IResult> GetWorld(int worldId, LoreDbContext db)
    {
      World? entity = await db.Worlds.FindAsync(worldId);
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

    public static async Task<IResult> GetWorldByName(string name, LoreDbContext db)
    {
      World? entity = await db.Worlds.Where(e => e.Name == name).FirstAsync();
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

    public static async Task<IResult> AddNewWorld(World world, LoreDbContext db)
    {
      db.Worlds.Add(world);
      await db.SaveChangesAsync();

      return TypedResults.Created($"/worlds/{world.Id}", world);
    }
    public static async Task<IResult> EditWorld(int worldId, World changedWorld, LoreDbContext db)
    {
      World? world = await db.Worlds.FindAsync(worldId);

      if (world is null) return Results.NotFound();

      world.Description = changedWorld.Description;
      world.Name = changedWorld.Name;

      await db.SaveChangesAsync();

      return Results.NoContent();
    }

    public static async Task<IResult> DeleteWorld(int worldId, LoreDbContext db)
    {
      if (await db.Worlds.FindAsync(worldId) is World world)
      {
        db.Worlds.Remove(world);
        await db.SaveChangesAsync();
        return Results.NoContent();
      }

      return Results.NotFound();
    }


    //entities
    public static async Task<IResult> GetAllEntities(LoreDbContext db)
    {
      ICollection<Entity> entities = await db.Entities.ToListAsync();
      return TypedResults.Ok(entities);
    }

    public static async Task<IResult> GetEntity(int entityId, LoreDbContext db)
    {
      Entity? entity = await db.Entities.FindAsync(entityId);
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

    public static async Task<IResult> GetEntityByName(string name, LoreDbContext db)
    {
      Entity? entity = await db.Entities.Where(e => e.Name == name).FirstAsync();
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

    public static async Task<IResult> GenerateNewEntity(EntityDto entityDto, LoreDbContext db, ILlmProvider llmProvider, int worldId)
    {
      World? world = await db.Worlds.FindAsync(worldId);
      if (world is null) return Results.ValidationProblem(new Dictionary<string, string[]>
      {
        ["worldId"] = new[] { "Can't find world with this id" }
      });

      ICollection<EntityType> entityTypes = await db.EntityTypes.ToListAsync();

      try
      {
        EntityDto llmEntityDto = await llmProvider.GetGeneratedEntityAsync(entityDto, world, entityTypes, []);
        Entity entity = await EntityDtoValidator.GetNewValidEntity(db, entityDto, worldId);

        db.Entities.Add(entity);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/worlds/{worldId}/entities/{entity.Id}", entity);
      }
      catch (EntityDtoInvalidException e)
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          ["Problem"] = new[] { e.Message }
        });
      }
    }

    public static async Task<IResult> GenerateUpdatedEntity(EntityDto entityDto, LoreDbContext db, ILlmProvider llmProvider, int worldId, int entityId)
    {
      Entity? entity = await db.Entities.Where(e => e.WorldId == worldId && e.Id == entityId).FirstAsync();
      if (entity is null) return Results.NotFound();

      World? world = await db.Worlds.FindAsync(worldId);
      if (world is null) return Results.ValidationProblem(new Dictionary<string, string[]>
      {
        ["worldId"] = new[] { "Can't find world with this id" }
      });

      ICollection<EntityType> entityTypes = await db.EntityTypes.ToListAsync();

      try
      {
        EntityDto llmEntityDto = await llmProvider.GetUpdatedEntityAsync(entityDto, world, entityTypes, []);
        Entity updatedEntity = await EntityDtoValidator.GetNewValidEntity(db, entityDto, worldId);

        db.Entry(entity).CurrentValues.SetValues(entity);
        entity.Id = entityId;

        await db.SaveChangesAsync();

        return Results.NoContent();
      }
      catch (EntityDtoInvalidException e)
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          ["Problem"] = new[] { e.Message }
        });
      }
    }

    public static async Task<IResult> AddNewEntity(EntityDto entityDto, int worldId, LoreDbContext db)
    {
      World? world = await db.Worlds.FindAsync(worldId);
      if (world is null) return Results.ValidationProblem(new Dictionary<string, string[]>
      {
        ["worldId"] = new[] { "Can't find world with this id" }
      });

      try
      {
        Entity entity = await EntityDtoValidator.GetNewValidEntity(db, entityDto, worldId);

        db.Entities.Add(entity);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/worlds/{worldId}/entities/{entity.Id}", entity);
      }
      catch (EntityDtoInvalidException e)
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          ["Problem"] = new[] { e.Message }
        });
      }
    }

    public static async Task<IResult> EditEntity(EntityDto entityDto, int worldId, int entityId, LoreDbContext db)
    {
      Entity? entity = await db.Entities.Where(e => e.WorldId == worldId && e.Id == entityId).FirstAsync();
      if (entity is null) return Results.NotFound();

      try
      {
        Entity? newEntity = await EntityDtoValidator.GetUpdatedValidEntity(db, entityDto, worldId);

        db.Entry(entity).CurrentValues.SetValues(newEntity);
        entity.Id = entityId;

        await db.SaveChangesAsync();

        return Results.NoContent();
      }
      catch (EntityDtoInvalidException e)
      {
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
          ["Problem"] = new[] { e.Message }
        });
      }
    }

    public static async Task<IResult> DeleteEntity(int worldId, int entityId, LoreDbContext db)
    {
      if (await db.Entities.FindAsync(worldId) is Entity world)
      {
        db.Entities.Remove(world);
        await db.SaveChangesAsync();
        return Results.NoContent();
      }

      return Results.NotFound();
    }
  }
}