using Honeyspur;

public class Fireplace : TileInstance
{
    private float animationStartTime = -1f;
    private const float FRAME_DURATION = 0.2f;
    private const int TOTAL_FRAMES = 7;
    private readonly Texture2D effectsTexture;
    private bool isAnimating = false;

    public Fireplace()
    {
        IsInteractable = true;
        effectsTexture = TextureManager.GetTexture("graphics/effects");
    }

    public override LightType LightType => LightType.Flame;
    public override Vector2 LightOffset => new(3f, 0f);

    public override bool OnRightClick(GameContext context)
    {
        IsOn = !IsOn;
        if (IsOn)
        {
            SoundPlayer.Play("flameOn");
            animationStartTime = context.DayManager.TotalElapsedTime;
            isAnimating = true;
        }
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

        // Draw the base furnace tile
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, def);

        // Draw the flame animation if the furnace is animating
        if (isAnimating)
        {
            if (animationStartTime < 0f)
                animationStartTime = context.DayManager.TotalElapsedTime;

            float elapsed = context.DayManager.TotalElapsedTime - animationStartTime;
            int frame = (int)(elapsed / FRAME_DURATION);

            if (frame >= TOTAL_FRAMES)
            {
                // Clamp to final frame and stop animating (one-shot)
                frame = TOTAL_FRAMES - 1;
                isAnimating = false;
            }

            Rectangle sourceRect = new(frame * 16, 48, 16, 16);
            Vector2 effectPosition = position + new Vector2(0, -4);
            spriteBatch.Draw(effectsTexture, effectPosition, sourceRect, Color.White);
        }
    }
}