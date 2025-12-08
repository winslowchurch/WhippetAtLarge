namespace Honeyspur;

public class ButterflyEffect
{
    private class Butterfly
    {
        public Vector2 Position;
        public Vector2 TargetPosition;
        public float Speed = 20f;
        public float AnimationTime;
        public int Frame;
        public int TextureIndex;
        public float MoveTimer;
    }

    private Texture2D butterflyTexture;
    private readonly List<Butterfly> butterflies = [];
    private readonly Random random = new();

    private readonly int frameCountPerButterfly = 3;
    private float frameRate = 8f;

    private int screenWidth, screenHeight;

    public void LoadContent(Texture2D texture, int width, int height)
    {
        butterflyTexture = texture;
        screenWidth = width;
        screenHeight = height;

        butterflies.Clear();
        for (int i = 0; i < 6; i++)
        {
            butterflies.Add(new Butterfly
            {
                Position = new Vector2(random.Next(0, width), random.Next(0, height)),
                TargetPosition = new Vector2(random.Next(0, width), random.Next(0, height)),
                TextureIndex = random.Next(0, 2),
                AnimationTime = 0f,
                MoveTimer = 0f
            });
        }
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        foreach (var butterfly in butterflies)
        {
            // Animate
            butterfly.AnimationTime += delta;
            butterfly.Frame = (int)(butterfly.AnimationTime * frameRate) % frameCountPerButterfly;

            // Move toward target
            Vector2 direction = butterfly.TargetPosition - butterfly.Position;
            if (direction.Length() > 1f)
            {
                direction.Normalize();
                butterfly.Position += direction * butterfly.Speed * delta;
            }

            // Pick a new target every few seconds
            butterfly.MoveTimer -= delta;
            if (butterfly.MoveTimer <= 0f)
            {
                butterfly.TargetPosition = new Vector2(
                    random.Next(Math.Max(0, (int)butterfly.Position.X - 50), Math.Min(screenWidth, (int)butterfly.Position.X + 50)),
                    random.Next(Math.Max(0, (int)butterfly.Position.Y - 50), Math.Min(screenHeight, (int)butterfly.Position.Y + 50))
                );
                butterfly.MoveTimer = random.Next(2, 5);
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var butterfly in butterflies)
        {
            int yOffset = (butterfly.TextureIndex * 16) + 32;
            var sourceRect = new Rectangle(butterfly.Frame * 16, yOffset, 16, 16);

            spriteBatch.Draw(
                butterflyTexture,
                butterfly.Position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                1f,
                SpriteEffects.None,
                0.95f
            );
        }
    }
}
