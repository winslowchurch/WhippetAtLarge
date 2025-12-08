using Microsoft.Xna.Framework.Content;

namespace Honeyspur.UI
{
    public class LifeBar
    {
        private Texture2D uiTexture;
        private UIAtlas uiAtlas;
        private static Texture2D pinkTexture;
        private SparkleEffect sparkleEffect;
        private int maxHealth = 100;
        private int currentHealth = 0;
        private int previousHealth = 0;
        
        // Damage animation fields
        private bool isDamageAnimating = false;
        private float damageAnimationTimer = 0f;
        private const float damageAnimationDuration = 1.0f; // One second animation for longer flashes

        public void LoadContent(ContentManager content, Texture2D sharedUiTexture, UIAtlas sharedUiAtlas)
        {
            uiTexture = sharedUiTexture;
            uiAtlas = sharedUiAtlas;
            sparkleEffect = new SparkleEffect();
            sparkleEffect.LoadContent(content);
            
            if (pinkTexture == null)
            {
                pinkTexture = new Texture2D(Game1.Instance.GraphicsDevice, 1, 1);
                pinkTexture.SetData([Game1.Colors.LightPink]);
            }
        }

        public void TickUpdate(GameTime gameTime, int current, int max)
        {
            if (current > previousHealth) sparkleEffect.TriggerSparkle(); // Trigger sparkles on health increase
            
            // Detect damage and trigger damage animation
            if (current < currentHealth && !isDamageAnimating)
            {
                isDamageAnimating = true;
                damageAnimationTimer = 0f;
            }
            
            previousHealth = currentHealth;
            currentHealth = current;
            maxHealth = max;

            // Update damage animation timer
            if (isDamageAnimating)
            {
                damageAnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (damageAnimationTimer >= damageAnimationDuration)
                {
                    isDamageAnimating = false;
                    damageAnimationTimer = 0f;
                }
            }

            sparkleEffect.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            // Get life bar rectangle from atlas
            Rectangle lifeBarSrc = uiAtlas.GetRect("lifeBar");
            Vector2 position = new(10, viewport.Height - lifeBarSrc.Height * 4 - 10);
            float scale = 4f;
            spriteBatch.Draw(uiTexture, position, lifeBarSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Calculate health bar height
            int barWidth = (int)(lifeBarSrc.Width * scale * (9f / 15f));
            int barHeight = (int)(lifeBarSrc.Height * scale * (67f / 86f));
            int filledHeight = (int)(barHeight * (currentHealth / (float)maxHealth));
            int barX = (int)(position.X + (lifeBarSrc.Width * scale - barWidth) / 2f);
            int y = (int)(position.Y + (15*scale)+(barHeight-filledHeight));

            // Calculate bar color with red flash effect during damage animation
            Color barColor = Color.White;
            if (isDamageAnimating)
            {
                float normalizedTime = damageAnimationTimer / damageAnimationDuration;
                
                // Create a slower pulsing red effect - 3 pulses over 1 second
                float flashIntensity = (float)Math.Sin(normalizedTime * Math.PI * 3) * (1f - normalizedTime * 0.5f);
                flashIntensity = Math.Max(0, flashIntensity);
                
                // Interpolate between original pink and bright red
                Color redFlash = new(255, 80, 80, 255);
                barColor = Color.Lerp(Color.White, redFlash, flashIntensity);
            }

            spriteBatch.Draw(pinkTexture, new Rectangle(barX, y, barWidth, filledHeight), barColor);
            
            // Draw sparkle animations
            const float sparkleScale = 4f;
            Vector2 basePos = new(position.X, position.Y);
            Vector2 leftSparklePos = basePos + new Vector2(-32, -50);
            Vector2 rightSparklePos = basePos + new Vector2(lifeBarSrc.Width * scale - 16, -16);
            
            sparkleEffect.DrawSparkles(spriteBatch, leftSparklePos, rightSparklePos, sparkleScale);
            
            MouseState mouseState = Mouse.GetState();
            Point mousePos = new(mouseState.X, mouseState.Y);
            // Draw health numbers if hovering
            if (IsHovering(mousePos, viewport))
            {
                DrawNumber(spriteBatch, viewport);
            }
        }

        public bool IsHovering(Point mousePos, Viewport viewport)
        {

            Rectangle lifeBarSrc = uiAtlas.GetRect("lifeBar");
            Vector2 position = new(10, viewport.Height - lifeBarSrc.Height * 4 - 10);
            float scale = 4f;
            
            Rectangle lifeBarBounds = new(
                (int)position.X,
                (int)position.Y,
                (int)(lifeBarSrc.Width * scale),
                (int)(lifeBarSrc.Height * scale)
            );
            
            return lifeBarBounds.Contains(mousePos);
        }

        private void DrawNumber(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (Game1.PixelFont == null) return;

            Rectangle lifeBarSrc = uiAtlas.GetRect("lifeBar");
            Vector2 position = new(10, viewport.Height - lifeBarSrc.Height * 4 - 10);
            float scale = 4f;
            
            // Calculate center of life bar
            float centerX = position.X + (lifeBarSrc.Width * scale / 2f);
            float centerY = position.Y + (lifeBarSrc.Height * scale / 2f);
            
            // Draw health text
            string currentHealthText = currentHealth.ToString();
            string maxHealthText = maxHealth.ToString();
            
            float textScale = 0.4f;
            Vector2 currentHealthSize = Game1.PixelFont.MeasureString(currentHealthText) * textScale;
            Vector2 maxHealthSize = Game1.PixelFont.MeasureString(maxHealthText) * textScale;
            
            // Position texts vertically centered in the bar
            Vector2 currentHealthPos = new(
                centerX - currentHealthSize.X / 2f,
                centerY - currentHealthSize.Y - 1
            );
            
            Vector2 maxHealthPos = new(
                centerX - maxHealthSize.X / 2f,
                centerY + 1
            );
            
            // Draw current health (top)
            spriteBatch.DrawString(
                Game1.PixelFont,
                currentHealthText,
                currentHealthPos,
                Game1.Colors.DarkPurple,
                0,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f
            );
            
            // Draw separator line
            float lineWidth = (int)(lifeBarSrc.Width * scale * (9f / 15f)) - 5;
            Rectangle lineRect = new(
                (int)(centerX - lineWidth / 2f),
                (int)(centerY - 2),
                (int)lineWidth,
                2
            );
            spriteBatch.Draw(pinkTexture, lineRect, Game1.Colors.DarkPurple);
            
            // Draw max health (bottom)
            spriteBatch.DrawString(
                Game1.PixelFont,
                maxHealthText,
                maxHealthPos,
                Game1.Colors.DarkPurple,
                0,
                Vector2.Zero,
                textScale,
                SpriteEffects.None,
                0f
            );
        }
    }
}