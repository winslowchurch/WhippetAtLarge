namespace Honeyspur;

public record GrowthConfig(int MaxStage, int[] DaysPerStage);

public static class TileFactory
{
    public static readonly GrowthConfig TreeGrowthConfig = new(3, [2, 1, 1]);
    public static readonly GrowthConfig FlowerTreeGrowthConfig = new(5, [2, 1, 1, 1, 1]);
    public static readonly GrowthConfig FlowerBushGrowthConfig = new(5, [2, 1, 1, 1, 1]);
    public static readonly GrowthConfig FlowerPlantGrowthConfig = new(5, [1, 1, 1, 1, 1]);

    public static TileInstance Create(TileDefinition def, Point pos, int id)
    {
        TileInstance tile;
        
        // Check if the definition specifies an instance type
        if (def.InstanceType != null)
            tile = CreateInstanceOfType(def.InstanceType, def.InstanceParam, pos, id);
        else
            tile = new StaticTile { Position = pos, BaseTileId = id };

        return tile;
    }

    private static TileInstance CreateInstanceOfType(Type instanceType, object param, Point pos, int id)
    {
        TileInstance tile;

        // Handle special cases that need parameters
        if (instanceType == typeof(FlowerBush))
        {
            string flowerName = (string)(param ?? "Unknown Flower");
            tile = new FlowerBush(flowerName, FlowerBushGrowthConfig) { Position = pos, BaseTileId = id };
        }
        else if (instanceType == typeof(FlowerPlant))
        {
            string flowerName = (string)(param ?? "Unknown Flower");
            tile = new FlowerPlant(flowerName, FlowerPlantGrowthConfig) { Position = pos, BaseTileId = id };
        }
        else if (instanceType == typeof(FlowerTree))
        {
            string treeName = (string)(param ?? "Unknown Tree");
            tile = new FlowerTree(treeName, FlowerTreeGrowthConfig) { Position = pos, BaseTileId = id };
        }
        else if (instanceType == typeof(Bed))
        {
            tile = new Bed { Position = pos, BaseTileId = id, IsOn = true };
        }
        else
        {
            // Use reflection to create instance with parameterless constructor
            tile = (TileInstance)Activator.CreateInstance(instanceType)!;
            tile.Position = pos;
            tile.BaseTileId = id;
        }

        return tile;
    }
}
