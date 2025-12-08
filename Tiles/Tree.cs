using Honeyspur.Items;

namespace Honeyspur;

public class FlowerTree : TileInstance
{
    private int growthStage = 0;
    private int daysInCurrentStage = 0;
    private readonly string grownItemName;
    private readonly GrowthConfig growthConfig;

    public FlowerTree(string flowerItemName, GrowthConfig growthConfig)
    {
        this.grownItemName = flowerItemName;
        this.growthConfig = growthConfig;
        IsInteractable = true;
    }

    public override bool OnShear(GameContext context)
    {
        if (growthStage > 3)
        {
            ItemInstance itemToDrop = null;

            // Flower
            if (growthStage == 4) itemToDrop = ItemRegistry.Get(grownItemName);
            else if (growthStage == 5) // Rotten stage
            {
                var originalItem = ItemRegistry.Get(grownItemName);
                if (originalItem != null)
                {
                    if (originalItem.Category == ItemCategory.Food) itemToDrop = ItemRegistry.Get("Rotten Fruit");
                    else if (originalItem.Category == ItemCategory.Flower) itemToDrop = ItemRegistry.Get("Dead Flower");
                }
            }

            if (itemToDrop != null)
            {
                Vector2 dropPosition = Position.ToVector2() * 16f + new Vector2(8, 8);
                for (int i = 0; i < 3; i++)
                {
                    var drop = new DroppedItem(itemToDrop.Clone(), dropPosition, 16f);
                    context.DroppedItems.Add(drop);
                }
            }
            // Return to fully grown but barren stage
            growthStage = 3;
            daysInCurrentStage = 0;
        }
        
        return true;
    }

    public override void DayUpdate(GameContext context)
    {
        if (growthStage >= growthConfig.MaxStage) return;

        daysInCurrentStage++;
        if (daysInCurrentStage >= growthConfig.DaysPerStage[growthStage])
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

        int frameWidth = 48;
        int frameIndex = growthStage;
        Rectangle sourceRect = new(frameIndex * frameWidth, def.ImageY, frameWidth, def.ImageHeight);

        int defaultImageOffsetY = def.ImageHeight - 16;
        int addedXOffset = def.ImageOffset?.X ?? 0;

        spriteBatch.Draw(
            tex,
            new Vector2(position.X + addedXOffset, position.Y - defaultImageOffsetY),
            sourceRect,
            Color.White
        );
    }
}

public class TreeTile : TileInstance
{
    private int growthStage = 0;
    private int daysInCurrentStage = 0;
    private readonly GrowthConfig growthConfig;

    public TreeTile()
    {
        this.growthConfig = TileFactory.TreeGrowthConfig;
    }

    public override void DayUpdate(GameContext context)
    {
        if (growthStage >= growthConfig.MaxStage) return;

        daysInCurrentStage++;
        if (daysInCurrentStage >= growthConfig.DaysPerStage[growthStage])
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

        int frameWidth = 48;
        int frameIndex = growthStage;
        Rectangle sourceRect = new(frameIndex * frameWidth, def.ImageY, frameWidth, def.ImageHeight);

        int defaultImageOffsetY = def.ImageHeight - 16;
        int addedXOffset = def.ImageOffset?.X ?? 0;

        spriteBatch.Draw(
            tex,
            new Vector2(position.X + addedXOffset, position.Y - defaultImageOffsetY),
            sourceRect,
            Color.White
        );
    }
};
