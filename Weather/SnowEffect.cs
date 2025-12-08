namespace Honeyspur;

public class SnowEffect
{
    private Texture2D snowTexture;
    private readonly Random random = new();

    private class SnowFlake
    {
        public Vector2 Position; // base position (updated by velocity)
        public Vector2 Direction = new(-0.5f, 1f); // mostly down, slight left
        public float Speed = 30f;
        public float AnimationTime = 0f;
        public int Frame = 0;

        // oscillation parameters (for the gliding path)
        public float Amplitude;
        public float Frequency;
        public float Phase;
    }

    private readonly List<SnowFlake> snowFlakes = new List<SnowFlake>();

    private readonly int frameCount = 3;

    private int screenWidth, screenHeight;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, int width, int height)
    {
        // Use the combined weather texture; snow frames are at y=64
        snowTexture = TextureManager.GetTexture("graphics/weather");
        screenWidth = width;
        screenHeight = height;
        // Spawn an initial cloud of snowflakes so the screen appears filled immediately
        InitializeSnow(200);
    }

    // Populate the scene with an initial set of flakes distributed across the whole screen
    private void InitializeSnow(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float x = (float)random.NextDouble() * (screenWidth + 150);
            // allow flakes to start anywhere vertically so the screen is already covered
            float y = (float)random.NextDouble() * (screenHeight + 60) - 30f; // a little above/below range

            var flake = new SnowFlake
            {
                Position = new Vector2(x, y),
                Speed = 20f + (float)random.NextDouble() * 30f,
                Amplitude = 8f + (float)random.NextDouble() * 20f,
                Frequency = 0.5f + (float)random.NextDouble() * 1.2f,
                Phase = (float)random.NextDouble() * MathF.PI * 2f,
                AnimationTime = (float)random.NextDouble() * 10f,
            };

            // small variation in horizontal drift per flake
            float horiz = -0.6f + (float)random.NextDouble() * 1.2f; // between -0.6 and +0.6
            flake.Direction = new Vector2(horiz, 1f);

            flake.Frame = random.Next(frameCount);

            snowFlakes.Add(flake);
        }
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // spawn a few flakes per frame (tweak as desired)
        for (int i = 0; i < 2; i++)
        {
            // spawn across the whole screen (and slightly off the right edge) so flakes appear visibly
            float x = random.Next(0, screenWidth + 150);
            float y = random.Next(-60, -10);

            var flake = new SnowFlake
            {
                Position = new Vector2(x, y),
                Speed = 20f + (float)random.NextDouble() * 30f,
                Amplitude = 8f + (float)random.NextDouble() * 20f,
                Frequency = 0.5f + (float)random.NextDouble() * 1.2f,
                Phase = (float)random.NextDouble() * MathF.PI * 2f,
                // pick a random static frame for this flake so it doesn't animate between different snow sprites
                Frame = random.Next(frameCount)
            };

            snowFlakes.Add(flake);
        }

        for (int i = snowFlakes.Count - 1; i >= 0; i--)
        {
            var flake = snowFlakes[i];

            // move base position by the velocity vector
            flake.Position += flake.Direction * flake.Speed * delta;
            flake.AnimationTime += delta;

            // remove when off-screen (a bit of leeway)
            if (flake.Position.Y > screenHeight + 40 || flake.Position.X < -60)
                snowFlakes.RemoveAt(i);
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var flake in snowFlakes)
        {
            // compute gliding offset using a sine wave
            float xOffset = MathF.Sin(flake.AnimationTime * flake.Frequency + flake.Phase) * flake.Amplitude;

            var drawPos = new Vector2(flake.Position.X + xOffset, flake.Position.Y);

            var source = new Rectangle(flake.Frame * 16, 64, 16, 16);
            spriteBatch.Draw(snowTexture, drawPos, source, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.95f);
        }
    }
}
