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
      RouteGroupBuilder worldsApi = app.MapGroup("/worlds");

      worldsApi.MapGet("", GetAllWorld);
      worldsApi.MapGet("/{worldId}", GetWorld);
      worldsApi.MapGet("by-name/{name}", GetWorldByName);
      worldsApi.MapPost("", AddNewWorld);
      worldsApi.MapPut("/{worldId}", EditWorld);
      worldsApi.MapDelete("/{worldId}", DeleteWorld);

      RouteGroupBuilder worldEntitiesApi = worldsApi.MapGroup("/{worldId:int}/entities");

      worldEntitiesApi.MapGet("", GetAllWorldEntities);
      worldEntitiesApi.MapGet("by-name/{name}", GetEntityByName);
      worldEntitiesApi.MapPost("add", AddNewEntity);
      worldEntitiesApi.MapPost("generate", GenerateNewEntity);
      worldEntitiesApi.MapPut("update/{entityId}", EditEntity);
      worldEntitiesApi.MapPut("regenerate/{entityId}", GenerateUpdatedEntity);

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
      World? entity = await db.Worlds.Where(e => e.Name == name).FirstOrDefaultAsync();
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
    public static async Task<IResult> GetAllWorldEntities(int worldId, LoreDbContext db)
    {
      ICollection<Entity> entities = await db.Entities.Where(e => e.WorldId == worldId).ToListAsync();
      return TypedResults.Ok(entities);
    }

    public static async Task<IResult> GetEntityByName(int worldId, string name, LoreDbContext db)
    {
      Entity? entity = await db.Entities.Where(e => e.Name == name && e.WorldId == worldId).FirstOrDefaultAsync();
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
      Entity? entity = await db.Entities.Where(e => e.WorldId == worldId && e.Id == entityId).FirstOrDefaultAsync();
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
      Entity? entity = await db.Entities.Where(e => e.WorldId == worldId && e.Id == entityId).FirstOrDefaultAsync();
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


  }
}