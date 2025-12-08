using Honeyspur;
using Honeyspur.Items;

public class FlowerBush : TileInstance
{
    private int growthStage = 0;
    private int daysInCurrentStage = 0;
    private readonly string flowerItemName;
    private readonly GrowthConfig growthConfig;

    public FlowerBush(string flowerItemName, GrowthConfig growthConfig)
    {
        this.flowerItemName = flowerItemName;
        this.growthConfig = growthConfig;
        IsInteractable = true;
    }

    public override void DayUpdate(GameContext context)
    {
        if (growthStage >= growthConfig.MaxStage) return;

        daysInCurrentStage++;
        // Only advance if not at dead stage
        if (growthStage < growthConfig.DaysPerStage.Length && daysInCurrentStage >= growthConfig.DaysPerStage[growthStage])
        {
            growthStage++;
            daysInCurrentStage = 0;
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        if (!tileTextures.TryGetValue(def.ImagePath, out var tex))
            return;

        Vector2 position = new(Position.X * 16, Position.Y * 16);

        int frameWidth = 32;
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
                for (int i = 0; i < 3; i++)
                {
                    var drop = new DroppedItem(itemToDrop.Clone(), dropPosition, 16f);
                    context.DroppedItems.Add(drop);
                }
            }
            // Reset bush to regrow
            growthStage = 2;
            daysInCurrentStage = 0;
        }
        return true;
    }
}
