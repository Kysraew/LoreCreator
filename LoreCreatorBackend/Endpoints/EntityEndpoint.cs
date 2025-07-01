
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
    }

    public static async Task<IResult> GetAllEntities(LoreDbContext db)
    {
      var entities = await db.Entities.ToListAsync();
      return TypedResults.Ok(entities);
    }
  }
}