namespace Honeyspur.Items;

public enum ItemCategory
{
    Food,
    Drink,
    Furniture,
    Seed,
    Tool,
    Flower,
}

public enum ToolType
{
    None,
    Hoe,
    Shears,
    WateringCan,
}

public enum SeedType
{
    None,
    FlowerPlant,  // Requires tilled grass
    FlowerBush,   // Works on regular grass
    Tree,         // Works on regular grass
}

public class ItemInstance
{
    public Texture2D Icon { get; set; }
    public string Name { get; set; }
    public ItemCategory Category { get; set; }
    public ToolType ToolType { get; set; } = ToolType.None;
    public SeedType SeedType { get; set; } = SeedType.None;
    public int Quantity { get; set; } = 1;
    public int IconX { get; set; } = 0;
    public int IconY { get; set; } = 0;
    public int HealthValue { get; set; } = 0;
    public string PlacedTileName { get; set; }
    public string Description { get; set; }

    public ItemInstance Clone()
    {
        return (ItemInstance)this.MemberwiseClone();
    }
}
