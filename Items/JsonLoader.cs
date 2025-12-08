using System.IO;
using System.Text.Json;

namespace Honeyspur.Items;

public static class JsonLoader
{
    public static List<ItemData> LoadItems(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<ItemData>>(json);
    }
}

public class ItemData
{
    public string Name { get; set; }
    public string Category { get; set; }
    public string ToolType { get; set; }
    public string SeedType { get; set; }
    public int IconX { get; set; }
    public int IconY { get; set; }
    public int HealthValue { get; set; }
    public string PlacedTileName { get; set; }
    public string Description { get; set; }
}