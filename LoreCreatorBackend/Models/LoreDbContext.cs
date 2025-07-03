
using Microsoft.EntityFrameworkCore;

namespace LoreCreatorBackend.Models
{
  public class LoreDbContext : DbContext
  {
    public LoreDbContext(DbContextOptions<LoreDbContext> options) : base(options) { }

    public DbSet<Entity> Entities => Set<Entity>();
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<WorldSetting> WorldSettings => Set<WorldSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<EntityType>().HasData(
          new EntityType { Id = 1, Name = "Person" },
          new EntityType { Id = 2, Name = "Place" },
          new EntityType { Id = 3, Name = "Object" }
      );
    }

  }
}