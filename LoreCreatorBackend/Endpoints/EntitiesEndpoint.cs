using LoreCreatorBackend.Infrastrucure.Database;
using LoreCreatorBackend.Models;

namespace LoreCreatorBackend.Endpoints
{
  public static class EntitiesEndpoints
  {
    public static void MapEntitiesEndpoints(this IEndpointRouteBuilder app)
    {
      RouteGroupBuilder entitiesApi = app.MapGroup("/entities");

      entitiesApi.MapGet("{entityId}", GetEntity);
      entitiesApi.MapDelete("{entityId}", DeleteEntity);
    }

    public static async Task<IResult> GetEntity(int entityId, LoreDbContext db)
    {
      Entity? entity = await db.Entities.FindAsync(entityId);
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
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