using Microsoft.Xna.Framework.Content;

namespace Honeyspur.UI
{
    public class SparkleEffect
    {
        private Texture2D sparkleTexture;
        private float sparkleTimer = -1f; // -1 means inactive
        private const float SPARKLE_DURATION = 1.4f;
        private const float FRAME_DURATION = 0.2f;
        private const int TOTAL_FRAMES = 7;
        private const float RIGHT_DELAY = 0.15f;

        public bool IsActive => sparkleTimer >= 0f;

        public void LoadContent(ContentManager content)
        {
            sparkleTexture = TextureManager.GetTexture("graphics/effects");
        }

        public void TriggerSparkle()
        {
            sparkleTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (sparkleTimer >= 0f)
            {
                sparkleTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (sparkleTimer >= SPARKLE_DURATION + RIGHT_DELAY)
                {
                    sparkleTimer = -1f; // Deactivate
                }
            }
        }

        public void DrawSparkles(SpriteBatch spriteBatch, Vector2 leftSparklePosition, Vector2 rightSparklePosition, float sparkleScale, Color color = default, int spriteRow = 0)
        {
            if (sparkleTimer < 0f) return;

            if (color == default) color = Color.White;

            // Draw left sparkle
            DrawSparkle(spriteBatch, sparkleTimer, leftSparklePosition, sparkleScale, color, spriteRow);

            // Draw right sparkle (with delay)
            if (sparkleTimer >= RIGHT_DELAY)
                DrawSparkle(spriteBatch, sparkleTimer - RIGHT_DELAY, rightSparklePosition, sparkleScale, color, spriteRow);
        }

        private void DrawSparkle(SpriteBatch spriteBatch, float timer, Vector2 position, float sparkleScale, Color color, int spriteRow)
        {
            int frame = (int)(timer / FRAME_DURATION);
            if (frame >= TOTAL_FRAMES) return;

            Rectangle sourceRect = new Rectangle(frame * 16, spriteRow * 16, 16, 16);
            spriteBatch.Draw(sparkleTexture, position, sourceRect, color, 0f, Vector2.Zero, sparkleScale, SpriteEffects.None, 0f);
        }
    }
}