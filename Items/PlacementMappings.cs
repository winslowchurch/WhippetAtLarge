using System.Linq;
using Honeyspur.Items;

namespace Honeyspur;

// This is for cases such as when you place a seed down, then the item placed is not a seed but a bush
// otherwise, it checks the whatever dictionary i have where the names match aka fridge = fridge 
public static class PlacementMappings
{
    public static bool TryGetPlacementInfo(string itemName, out string tileName, out Type requiredBase)
    {
        // First check if the item has a custom placement mapping in the item data
        var itemData = ItemRegistry.Get(itemName);
        if (itemData?.PlacedTileName != null)
        {
            tileName = itemData.PlacedTileName;
            // Seeds require grass as the base tile
            requiredBase = itemData.Category == ItemCategory.Seed ? typeof(Grass) : null;
            
            // Furniture needs special handling based on tile type
            if (itemData.Category == ItemCategory.Furniture)
            {
                // Check the tile definition to determine required base
                // Copy to local variable since out parameters can't be used in lambdas
                string localTileName = tileName;
                var tileDef = TileDefinitions.All.Values.FirstOrDefault(def => def.Name == localTileName);
                if (tileDef != null)
                {
                    // Beds require floor tiles as the base
                    requiredBase = tileDef.InstanceType == typeof(Bed) ? typeof(Floor) : null;
                }
            }
            
            return true;
        }

        // Fallback: check if the item name directly matches a tile definition
        var directTileDef = TileDefinitions.All.Values.FirstOrDefault(def => def.Name == itemName);
        if (directTileDef != null)
        {
            tileName = directTileDef.Name;
            requiredBase = directTileDef.InstanceType == typeof(Bed) ? typeof(Floor) : null;
            return true;
        }

        tileName = null;
        requiredBase = null;
        return false;
    }
}