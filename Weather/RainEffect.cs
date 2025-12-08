namespace Honeyspur;

public class RainEffect
{
    private Texture2D rainTexture;
    private readonly Random random = new();

    private class RainDrop
    {
        public Vector2 Position;
        public Vector2 Direction = new(-2, 4);
        public float Speed = 100f;
        public float AnimationTime = 0f;
        public int Frame = 0;
    }

    private class RainSplash
    {
        public Vector2 Position;
        public float AnimationTime = 0f;
        public int Frame = 0;
        public float Lifetime = 0.5f;
    }

    private readonly List<RainDrop> rainDrops = new List<RainDrop>();
    private readonly List<RainSplash> rainSplashes = new List<RainSplash>();

    private readonly int rainDropFrameCount = 3;
    private readonly int rainSplashFrameCount = 3;
    private readonly float rainDropFrameRate = 8f;
    private readonly float rainSplashFrameRate = 5f;

    private int screenWidth, screenHeight;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, int width, int height)
    {
        // Use the combined weather texture; rain frames are at y=0
        rainTexture = TextureManager.GetTexture("graphics/weather");
        screenWidth = width;
        screenHeight = height;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        for (int i = 0; i < 3; i++)
        {
            float x = random.Next(0, screenWidth + 150);
            float y = random.Next(-50, -10);
            rainDrops.Add(new RainDrop { Position = new Vector2(x, y) });

            float splashX = x + random.Next(-2, 3);
            float splashY = random.Next(0, screenHeight - 10);
            rainSplashes.Add(new RainSplash { Position = new Vector2(splashX, splashY) });
        }

        for (int i = rainDrops.Count - 1; i >= 0; i--)
        {
            var drop = rainDrops[i];
            drop.Position += drop.Direction * drop.Speed * delta;
            drop.AnimationTime += delta;
            drop.Frame = (int)(drop.AnimationTime * rainDropFrameRate) % rainDropFrameCount;

            if (drop.Position.Y > screenHeight + 20)
                rainDrops.RemoveAt(i);
        }

        for (int i = rainSplashes.Count - 1; i >= 0; i--)
        {
            var splash = rainSplashes[i];
            splash.AnimationTime += delta;
            splash.Frame = (int)(splash.AnimationTime * rainSplashFrameRate) % rainSplashFrameCount;
            splash.Lifetime -= delta;
            if (splash.Lifetime <= 0)
                rainSplashes.RemoveAt(i);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var drop in rainDrops)
        {
            var source = new Rectangle(drop.Frame * 16, 0, 16, 16);
            spriteBatch.Draw(rainTexture, drop.Position, source, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.95f);
        }

        foreach (var splash in rainSplashes)
        {
            var source = new Rectangle(splash.Frame * 16, 16, 16, 16);
            spriteBatch.Draw(rainTexture, splash.Position, source, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.96f);
        }
    }
}
