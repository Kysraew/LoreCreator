
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Models
{
  public class LoreDbContext : DbContext
  {
    public LoreDbContext(DbContextOptions<LoreDbContext> options) : base(options) { }

    public DbSet<Entity> Entities => Set<Entity>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
  }
}