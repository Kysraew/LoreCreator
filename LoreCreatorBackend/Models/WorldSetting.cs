namespace LoreCreatorBackend.Models
{
  public class WorldSetting
  {
    public int Id { get; set; }

    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
  }
}