using Microsoft.Xna.Framework.Content;

namespace Honeyspur.Items;

public static class ItemRegistry
{
    public static Dictionary<string, ItemInstance> Items = [];

    public static void LoadContent(ContentManager content)
    {
        var objectsTexture = TextureManager.GetTexture("graphics/objects");

        var itemDataList = JsonLoader.LoadItems("Items/items.json");
        foreach (var data in itemDataList)
        {
            var category = Enum.Parse<ItemCategory>(data.Category);
            var toolType = !string.IsNullOrEmpty(data.ToolType) 
                ? Enum.Parse<ToolType>(data.ToolType) 
                : ToolType.None;
            var seedType = !string.IsNullOrEmpty(data.SeedType)
                ? Enum.Parse<SeedType>(data.SeedType)
                : SeedType.None;
            
            Items[data.Name] = new ItemInstance
            {
                Name = data.Name,
                Category = category,
                ToolType = toolType,
                SeedType = seedType,
                Icon = objectsTexture,
                IconX = data.IconX,
                IconY = data.IconY,
                HealthValue = data.HealthValue,
                PlacedTileName = data.PlacedTileName,
                Description = data.Description
            };
        }

        // Load furniture items from furnitureItems.json
        var furnitureDataList = JsonLoader.LoadItems("Items/furnitureItems.json");
        foreach (var data in furnitureDataList)
        {
            var category = Enum.Parse<ItemCategory>(data.Category);
            
            // For furniture, we'll set the icon to null initially 
            // and render it dynamically based on the tile definition
            Items[data.Name] = new ItemInstance
            {
                Name = data.Name,
                Category = category,
                Icon = null, // Will be set dynamically
                IconX = 0,
                IconY = 0,
                PlacedTileName = data.Name, // Furniture items match tile names
                Description = data.Description
            };
        }
    }

    public static ItemInstance Get(string key)
    {
        return Items.TryGetValue(key, out ItemInstance value)
            ? new ItemInstance
            {
                Name = value.Name,
                Category = value.Category,
                ToolType = value.ToolType,
                SeedType = value.SeedType,
                Icon = value.Icon,
                IconX = value.IconX,
                IconY = value.IconY,
                HealthValue = value.HealthValue,
                PlacedTileName = value.PlacedTileName,
                Description = value.Description
            }
            : null;
    }

    public static ItemInstance GetFurnitureItem(string name, Texture2D icon)
    {
        // First check if this furniture item exists in the registry
        if (Items.TryGetValue(name, out var registeredItem) && registeredItem.Category == ItemCategory.Furniture)
        {
            var item = registeredItem.Clone();
            item.Icon = icon; // Set the dynamically rendered icon
            item.Quantity = 1;
            return item;
        }

        // Fallback: create a basic furniture item if not found in registry
        return new ItemInstance
        {
            Name = name,
            Category = ItemCategory.Furniture,
            Icon = icon,
            IconX = 0,
            IconY = 0,
            PlacedTileName = name,
            Quantity = 1
        };
    }
}
