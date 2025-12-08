using Honeyspur;
using Honeyspur.Items;

public class FlowerPlant : TileInstance
{
    private int growthStage = 0;
    private int daysInCurrentStage = 0;
    private readonly string flowerItemName;
    private readonly GrowthConfig growthConfig;

    public FlowerPlant(string flowerItemName, GrowthConfig growthConfig)
    {
        this.flowerItemName = flowerItemName;
        this.growthConfig = growthConfig;
        IsInteractable = true;
    }

    public override void DayUpdate(GameContext context)
    {
        // Only advance if not fully matured (can become dead even when not watering)
        if (growthStage >= growthConfig.MaxStage - 1)
            return;
        

        // Check if the grass base tile beneath this plant was watered today
        // Use the map name to get the correct base tile from the right map
        var baseTile = !string.IsNullOrEmpty(MapName) 
            ? context.MapManager.GetBaseTileAt(Position, MapName)
            : context.MapManager.GetBaseTileAt(Position);
        
        bool wasWateredToday = false;
        if (baseTile is Grass grassTile)
            wasWateredToday = grassTile.IsWatered();

        if (wasWateredToday)
        {
            daysInCurrentStage++;
            if (growthStage < growthConfig.DaysPerStage.Length && daysInCurrentStage >= growthConfig.DaysPerStage[growthStage])
            {
                growthStage++;
                daysInCurrentStage = 0;
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        if (!tileTextures.TryGetValue(def.ImagePath, out var tex))
            return;

        Vector2 position = new(Position.X * 16, Position.Y * 16);

        int frameWidth = 16;
        int frameIndex = growthStage;
        Rectangle sourceRect = new(
            def.ImageX + frameIndex * frameWidth,
            def.ImageY,
            frameWidth,
            def.ImageHeight
        );

        int defaultImageOffsetY = def.ImageHeight - 16;

        spriteBatch.Draw(
            tex,
            new Vector2(position.X, position.Y - defaultImageOffsetY),
            sourceRect,
            Color.White
        );
    }

     public override bool OnShear(GameContext context)
    {
        if (growthStage >= 3)
        {
            ItemInstance itemToDrop = null;

            if (growthStage == 5)
                itemToDrop = ItemRegistry.Get("Dead Flower");
            else if (growthStage == 4)
                itemToDrop = ItemRegistry.Get(flowerItemName);

            if (itemToDrop != null)
            {
                Vector2 dropPosition = Position.ToVector2() * 16f + new Vector2(8, 8);
                // Only drop 1 flower instead of 3
                var drop = new DroppedItem(itemToDrop.Clone(), dropPosition, 16f);
                context.DroppedItems.Add(drop);
            }
            // Reset plant to regrow
            growthStage = 2;
            daysInCurrentStage = 0;

        }
        return true;
    }
}
