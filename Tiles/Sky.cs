using Honeyspur;

// the sky and such
public class Sky : TileInstance
{
    // Hardcoded lists of background tile IDs for day and night.
    private static readonly int[] DayTiles = [132, 134, 136, 138];
    private static readonly int[] NightTiles = [133, 135, 137, 139];

    private static readonly Random rng = new();
    private int currentDayIndex = 0;
    private int currentNightIndex = 0;

    // Twinkling star animation
    private const float STAR_FRAME_DURATION = 0.17f;
    private const int STAR_TOTAL_FRAMES = 7;
    private const float STAR_INTERVAL = 10f;
    private const float STAR_ANIMATION_DURATION = STAR_FRAME_DURATION * STAR_TOTAL_FRAMES;
    private readonly Texture2D effectsTexture;

    // Random offset for this tile instance (so not all sky tiles twinkle at the same time)
    private readonly float timeOffset;

    public Sky()
    {
        effectsTexture = TextureManager.GetTexture("graphics/effects");
        // Random time offset so each sky tile twinkles at different times
        timeOffset = (float)(rng.NextDouble() * STAR_INTERVAL);
    }

    public override void DayUpdate(GameContext context)
    {
        // Randomly select new sky tile indices for the new day/night
        currentDayIndex = rng.Next(0, DayTiles.Length);
        currentNightIndex = rng.Next(0, NightTiles.Length);
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        Vector2 position = new(Position.X * 16, Position.Y * 16);

        // Use DayManager's darkness transition to blend between day and night sky tiles.
        float darkness = context.DayManager.GetDarknessTransition();
        float nightAlpha = Math.Clamp(darkness / 0.8f, 0f, 1f);
        float dayAlpha = 1f - nightAlpha;

        // Choose paired indices (safeguard with modulo in case lengths differ)
        int dayIndex = currentDayIndex % DayTiles.Length;
        int nightIndex = currentNightIndex % NightTiles.Length;

        var dayDef = TileDefinitions.All[DayTiles[dayIndex]];
        var nightDef = TileDefinitions.All[NightTiles[nightIndex]];

        // Draw day tile with its alpha, then night tile on top with night alpha to blend.
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, dayDef, Color.White * dayAlpha);
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, nightDef, Color.White * nightAlpha);

        // Draw twinkling star animation at night
        if (darkness > 0.5f)
        {
            float elapsed = context.DayManager.TotalElapsedTime + timeOffset;
            float cycleTime = elapsed % STAR_INTERVAL;

            // Show star only during the animation duration at the start of each cycle
            if (cycleTime < STAR_ANIMATION_DURATION)
            {
                int frame = (int)(cycleTime / STAR_FRAME_DURATION) % STAR_TOTAL_FRAMES;
                Rectangle sourceRect = new(frame * 16, 96, 16, 16);

                // Generate deterministic random position based on the current cycle
                // Position stays the same for the entire animation, but changes each cycle
                int cycle = (int)(elapsed / STAR_INTERVAL);
                Random starRng = new(cycle * 1000 + Position.X * 100 + Position.Y);

                Vector2 starOffset = new(
                    (float)(starRng.NextDouble() * dayDef.ImageWidth),     // 0 to ImageWidth
                    (float)(starRng.NextDouble() * -dayDef.ImageHeight)    // -ImageHeight to 0
                );

                Vector2 starPosition = position + starOffset;
                spriteBatch.Draw(effectsTexture, starPosition, sourceRect, Color.White * nightAlpha);
            }
        }

        // Draw mountains on top of the background (full opacity)
        var mountainDef = TileDefinitions.All[BaseTileId];
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, mountainDef);
    }
}