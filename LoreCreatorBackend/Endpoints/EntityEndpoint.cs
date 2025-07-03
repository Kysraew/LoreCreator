
using LoreCreatorBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Endpoints
{
  public static class EntitiesEndpoints
  {
    public static void MapEititiesEndpoints(this IEndpointRouteBuilder app)
    {
      RouteGroupBuilder entitiesApi = app.MapGroup("/entities");

      entitiesApi.MapGet("/", GetAllEntities);
      entitiesApi.MapGet("/{id:int}", GetEntity);
      entitiesApi.MapGet("by-name/{name: string}", GetEntityByName);
    }

    public static async Task<IResult> GetAllEntities(LoreDbContext db)
    {
      ICollection<Entity> entities = await db.Entities.ToListAsync();
      return TypedResults.Ok(entities);
    }

    public static async Task<IResult> GetEntity(int id, LoreDbContext db)
    {
      Entity? entity = await db.Entities.FindAsync(id);
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

    public static async Task<IResult> GetEntityByName(string name, LoreDbContext db)
    {
      Entity? entity = await db.Entities.Where(e => e.Name == name).FirstAsync();
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }


    public static async Task<IResult> GenerateNewEntity(string name, LoreDbContext db)
    {
      Entity? entity = await db.Entities.Where(e => e.Name == name).FirstAsync();
      return entity is null ? Results.NotFound() : TypedResults.Ok(entity);
    }

  }
}