using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Endpoints
{
  public static class EntitiesEndpoint
  {
    public static void MapEntitiesEndpoints(this IEndpointRouteBuilder app)
    {
      RouteGroupBuilder entityTypesApi = app.MapGroup("/types");

      entityTypesApi.MapGet("", GetAllEntityTypes);
      entityTypesApi.MapGet("/{entityTypeId}", GetEntityType);
      //entityTypesApi.MapGet("by-name/{name}", GetEntityTypeByName);
      entityTypesApi.MapPost("", AddNewEntityType);
      entityTypesApi.MapPut("/{entityTypeId}", EditEntityType);
      entityTypesApi.MapDelete("/{entityTypeId}", DeleteEntityType);
    }

    public static async Task<IResult> GetAllEntityTypes(LoreDbContext db)
    {
      ICollection<EntityType> entities = await db.EntityTypes.ToListAsync();
      return TypedResults.Ok(entities);
    }

    public static async Task<IResult> GetEntityType(int entityTypeId, LoreDbContext db)
    {
      EntityType? entityType = await db.EntityTypes.FindAsync(entityTypeId);
      return entityType is null ? Results.NotFound() : TypedResults.Ok(entityType);
    }

    public static async Task<IResult> GetWorldByName(string name, LoreDbContext db)
    {
      EntityType? entityType = await db.EntityTypes.Where(e => e.Name == name).FirstOrDefaultAsync();
      return entityType is null ? Results.NotFound() : TypedResults.Ok(entityType);
    }

    public static async Task<IResult> AddNewEntityType(EntityType entityType, LoreDbContext db)
    {
      db.EntityTypes.Add(entityType);
      await db.SaveChangesAsync();

      return TypedResults.Created($"/worlds/{entityType.Id}", entityType);
    }
    public static async Task<IResult> EditEntityType(int entityTypeId, EntityType changedEntityType, LoreDbContext db)
    {
      EntityType? entityType = await db.EntityTypes.FindAsync(entityTypeId);

      if (entityType is null) return Results.NotFound();

      entityType.Name = changedEntityType.Name;

      await db.SaveChangesAsync();

      return Results.NoContent();
    }

    public static async Task<IResult> DeleteEntityType(int entityTypeId, LoreDbContext db)
    {
      if (await db.EntityTypes.FindAsync(entityTypeId) is EntityType entityType)
      {
        db.EntityTypes.Remove(entityType);
        await db.SaveChangesAsync();
        return Results.NoContent();
      }

      return Results.NotFound();
    }
  }
}