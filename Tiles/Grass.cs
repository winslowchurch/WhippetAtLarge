using Honeyspur;

public class Grass : TileInstance
{
    private bool isWatered = false;
    private bool isTilled = false;
    private static readonly Random random = new();

    public Grass()
    {
        IsInteractable = true;
    }

    public override void DayUpdate(GameContext context)
    {
        // Water the tile if weather is raining or snowing
        if (isTilled && context.WeatherManager.IsWetWeather())
            isWatered = true;
        else
            isWatered = false;

        // Check if there's a flower plant at this tile position
        if (isTilled)
        {
            var objectTile = !string.IsNullOrEmpty(MapName)
                ? context.MapManager.GetObjectTileAt(Position, MapName)
                : context.MapManager.GetObjectTileAt(Position);

            // If no flower plant exists, 50% chance to untill
            if (objectTile is not FlowerPlant)
            {
                if (random.NextDouble() < 0.5)
                {
                    isTilled = false;
                    isWatered = false;
                }
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        var waterShadowDef = TileDefinitions.All[33];
        var tilledSoilDef = TileDefinitions.All[38];

        TileRenderer.DrawTile( // Draw normal grass tile
            context,
            spriteBatch,
            tileTextures,
            new Vector2(Position.X * 16, Position.Y * 16),
            def
        );

        if (isTilled)
        {
            TileRenderer.DrawTile( // Draw tilled soil overlay
                context,
                spriteBatch,
                tileTextures,
                new Vector2(Position.X * 16, Position.Y * 16),
                tilledSoilDef
            );


            if (isWatered)
            {
                TileRenderer.DrawTile( // Water shadow
                    context,
                    spriteBatch,
                    tileTextures,
                    new Vector2(Position.X * 16, Position.Y * 16),
                    waterShadowDef
                );
            }
        }
    }

    public override bool OnSpray(GameContext context)
    {
        // Can only water if tilled
        if (!isTilled)
            return false;

        isWatered = true;
        return true;
    }

    public override bool OnHoe(GameContext context)
    {
        // Toggle tilled state
        isTilled = true;
        if (context.WeatherManager.IsWetWeather())
            isWatered = true;
        return true;
    }

    // Method for plants to check if grass is watered
    public bool IsWatered()
    {
        return isWatered;
    }

    // Method to check if grass is tilled (for planting validation)
    public bool IsTilled()
    {
        return isTilled;
    }

    // Method to set watered status (for other systems if needed)
    public void SetWatered(bool watered)
    {
        isWatered = watered;
    }
}
