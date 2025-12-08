using Microsoft.Xna.Framework.Content;

namespace Honeyspur.UI
{
    public class AnnouncementBox
    {
        private Texture2D uiTexture;
        private UIAtlas uiAtlas;
        private Texture2D whiteTexture;
        private string message;
        private bool isVisible;
        private Action onClose;

        // Typewriter animation
        private int visibleCharacters;
        private float typewriterTimer;
        private readonly float typewriterSpeed = 0.05f;
        private bool isTypewriterComplete;

        private const float BoxScale = 5f;
        private const float TextScale = 0.6f;

        // Cached values for layout to avoid duplication
        private Rectangle cachedBoxRect;
        private bool rectNeedsUpdate = true;

        public void LoadContent(GraphicsDevice graphicsDevice, Texture2D sharedUiTexture, UIAtlas sharedUiAtlas)
        {
            uiTexture = sharedUiTexture;
            uiAtlas = sharedUiAtlas;
            whiteTexture = new Texture2D(graphicsDevice, 1, 1);
            whiteTexture.SetData([Color.White]);
        }

        public bool IsVisible() => isVisible;

        public void Show(string message, Action onClose = null, GameContext context = null)
        {
            this.message = message;
            this.onClose = onClose;
            isVisible = true;

            // Reset animation
            visibleCharacters = 0;
            typewriterTimer = 0f;
            isTypewriterComplete = false;
            rectNeedsUpdate = true;

            context?.FreezeGame();
        }

        public void Hide(GameContext context = null)
        {
            isVisible = false;
            onClose?.Invoke();
            context?.UnfreezeGame();
        }

        public void TickUpdate(GameTime gameTime)
        {
            if (!isVisible || isTypewriterComplete || string.IsNullOrEmpty(message))
                return;

            typewriterTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (typewriterTimer >= typewriterSpeed)
            {
                typewriterTimer = 0f;
                visibleCharacters++;

                if (visibleCharacters <= message.Length)
                    SoundPlayer.Play("text");

                if (visibleCharacters >= message.Length)
                    isTypewriterComplete = true;
            }
        }

        private Rectangle GetBoxRect(Viewport viewport)
        {
            // Only recalc when needed
            if (!rectNeedsUpdate)
                return cachedBoxRect;

            Rectangle src = uiAtlas.GetRect("announcementBox");

            Vector2 pos = new(
                viewport.Width / 2f - (src.Width * BoxScale) / 2f,
                viewport.Height / 2f - (src.Height * BoxScale) / 2f
            );

            cachedBoxRect = new Rectangle(
                pos.ToPoint(),
                new Point((int)(src.Width * BoxScale), (int)(src.Height * BoxScale))
            );

            rectNeedsUpdate = false;
            return cachedBoxRect;
        }

        public bool IsHovering(Point mouseScreenPos, GameContext context)
        {
            if (!isVisible)
                return false;

            return GetBoxRect(context.GraphicsDevice.Viewport).Contains(mouseScreenPos);
        }

        public bool HandleClick(Point mouseScreenPos, GameContext context)
        {
            if (!isVisible)
                return false;

            if (GetBoxRect(context.GraphicsDevice.Viewport).Contains(mouseScreenPos))
            {
                SoundPlayer.Play("click");
                Hide(context);
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!isVisible)
                return;

            Rectangle src = uiAtlas.GetRect("announcementBox");
            Rectangle rect = GetBoxRect(viewport);

            // Draw announcement box
            spriteBatch.Draw(uiTexture, rect.Location.ToVector2(), src, Color.White, 0f, Vector2.Zero, BoxScale, SpriteEffects.None, 0f);

            // Draw message
            Vector2 messagePos = rect.Location.ToVector2() + new Vector2(24, rect.Height / 2f - 14);
            string visibleMessage = visibleCharacters >= message.Length
                ? message
                : message[..Math.Max(0, visibleCharacters)];

            spriteBatch.DrawString(Game1.PixelFont, visibleMessage, messagePos, Game1.Colors.DarkRed, 0f, Vector2.Zero, TextScale, SpriteEffects.None, 0f);
        }
    }
}