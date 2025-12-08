using Honeyspur;

public class ThreeWickCandle : TileInstance
{
    private const float FRAME_DURATION = 0.3f;
    private const int TOTAL_FRAMES = 4;
    private readonly Texture2D effectsTexture;

    public ThreeWickCandle()
    {
        IsInteractable = true;
        effectsTexture = TextureManager.GetTexture("graphics/effects");
    }

    public override LightType LightType => LightType.Flame;
    public override Vector2 LightOffset => new(3f, 0f);

    public override bool OnRightClick(GameContext context)
    {
        IsOn = !IsOn;
        if (IsOn) SoundPlayer.Play("candle");
        context.MapManager.ToggleLightTileList(this);
        return true;
    }

    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        Vector2 position = new(Position.X * 16, Position.Y * 16);

        // Draw the base candle tile
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, def);

        // Draw the flame animation if the candle is on
        if (IsOn)
        {
            float elapsed = context.DayManager.TotalElapsedTime;
            int frame = (int)(elapsed / FRAME_DURATION) % TOTAL_FRAMES;

            Rectangle sourceRect = new(frame * 16, 64, 16, 16);
            Vector2 effectPosition = position + new Vector2(0, -15);
            spriteBatch.Draw(effectsTexture, effectPosition, sourceRect, Color.White * 0.9f);
        }
    }
}

public class TwoWickCandle : TileInstance
{
    private const float FRAME_DURATION = 0.3f;
    private const int TOTAL_FRAMES = 3;
    private readonly Texture2D effectsTexture;

    public TwoWickCandle()
    {
        IsInteractable = true;
        effectsTexture = TextureManager.GetTexture("graphics/effects");
    }

    public override LightType LightType => LightType.Flame;
    public override Vector2 LightOffset => new(3f, 0f);

    public override bool OnRightClick(GameContext context)
    {
        IsOn = !IsOn;
        if (IsOn) SoundPlayer.Play("candle");
        context.MapManager.ToggleLightTileList(this);
        return true;
    }

    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        Vector2 position = new(Position.X * 16, Position.Y * 16);

        // Draw the base candle tile
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, def);

        // Draw the flame animation if the candle is on
        if (IsOn)
        {
            float elapsed = context.DayManager.TotalElapsedTime;
            int frame = (int)(elapsed / FRAME_DURATION) % TOTAL_FRAMES;

            Rectangle sourceRect = new((frame * 16) + 64, 64, 16, 16);
            Vector2 effectPosition = position + new Vector2(0, -7);
            spriteBatch.Draw(effectsTexture, effectPosition, sourceRect, Color.White * 0.6f);
        }
    }
}