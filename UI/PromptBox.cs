using Microsoft.Xna.Framework.Content;

namespace Honeyspur.UI
{
    public class PromptBox
    {
        private Texture2D uiTexture;
        private UIAtlas uiAtlas;
        private Texture2D whiteTexture;
        private string question;
        private bool isVisible = false;
        private Rectangle yesButton, noButton;
        private Action onYes;
        private bool wasHoveringYes = false;
        private bool wasHoveringNo = false;

        public void LoadContent(GraphicsDevice graphicsDevice, Texture2D sharedUiTexture, UIAtlas sharedUiAtlas)
        {
            uiTexture = sharedUiTexture;
            uiAtlas = sharedUiAtlas;
            whiteTexture = new Texture2D(graphicsDevice, 1, 1);
            whiteTexture.SetData([Color.White]);
        }

        public bool IsVisible() => isVisible;

        public void Show(string question, Action onYes, GameContext context)
        {
            this.question = question;
            this.onYes = onYes;
            isVisible = true;
            wasHoveringYes = false;
            wasHoveringNo = false;
            context.FreezeGame();
        }

        public void Hide(GameContext context)
        {
            isVisible = false;
            context.UnfreezeGame();
        }

        public bool IsHovering(Point mouseScreenPos, GameContext context)
        {
            if (!isVisible) return false;

            Rectangle promptBoxSrc = uiAtlas.GetRect("promptBox");
            float boxScale = 5f;
            Vector2 boxPos = new(
                context.GraphicsDevice.Viewport.Width / 2 - (promptBoxSrc.Width * boxScale) / 2,
                context.GraphicsDevice.Viewport.Height / 2 - (promptBoxSrc.Height * boxScale) / 2
            );

            Vector2 yesPos = boxPos + new Vector2(45, 75);
            Vector2 noPos = boxPos + new Vector2(45, 115);

            int buttonWidth = (int)((promptBoxSrc.Width * boxScale) - 90);
            int buttonHeight = 40;

            Rectangle yesRect = new(yesPos.ToPoint(), new Point(buttonWidth, buttonHeight));
            Rectangle noRect = new(noPos.ToPoint(), new Point(buttonWidth, buttonHeight));

            return yesRect.Contains(mouseScreenPos) || noRect.Contains(mouseScreenPos);
        }

        public bool HandleClick(Point mouseScreenPos, GameContext context)
        {
            if (!isVisible) return false;

            // Get prompt box rectangle from atlas
            Rectangle promptBoxSrc = uiAtlas.GetRect("promptBox");
            float boxScale = 5f;
            Vector2 boxPos = new(
                context.GraphicsDevice.Viewport.Width / 2 - (promptBoxSrc.Width * boxScale) / 2,
                context.GraphicsDevice.Viewport.Height / 2 - (promptBoxSrc.Height * boxScale) / 2
            );

            Vector2 yesPos = boxPos + new Vector2(45, 75);
            Vector2 noPos = boxPos + new Vector2(45, 115);

            int buttonWidth = (int)((promptBoxSrc.Width * boxScale) - 90);
            int buttonHeight = 40;

            Rectangle yesRect = new(yesPos.ToPoint(), new Point(buttonWidth, buttonHeight));
            Rectangle noRect = new(noPos.ToPoint(), new Point(buttonWidth, buttonHeight));

            if (yesRect.Contains(mouseScreenPos))
            {
                SoundPlayer.Play("click");
                Hide(context);
                onYes?.Invoke();
                return true;
            }

            if (noRect.Contains(mouseScreenPos))
            {
                SoundPlayer.Play("click");
                Hide(context);
                return true;
            }

            return false;
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!isVisible) return;

            // Get prompt box rectangle from atlas
            Rectangle promptBoxSrc = uiAtlas.GetRect("promptBox");
            float boxScale = 5f;
            Vector2 boxPos = new(
                viewport.Width / 2 - (promptBoxSrc.Width * boxScale) / 2,
                viewport.Height / 2 - (promptBoxSrc.Height * boxScale) / 2
            );

            spriteBatch.Draw(uiTexture, boxPos, promptBoxSrc, Color.White, 0f, Vector2.Zero, boxScale, SpriteEffects.None, 0f);

            // Question position and scale
            float textScale = 0.75f;
            Vector2 questionPos = boxPos + new Vector2(45, 30);
            spriteBatch.DrawString(Game1.PixelFont, question, questionPos, Game1.Colors.DarkRed, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            // Button positions
            Vector2 yesPos = boxPos + new Vector2(45, 75);
            Vector2 noPos = boxPos + new Vector2(45, 115);

            // Button sizes (full box width minus 20px for padding)
            int buttonWidth = (int)((promptBoxSrc.Width * boxScale) - 90);
            int buttonHeight = 40;

            yesButton = new Rectangle(yesPos.ToPoint(), new Point(buttonWidth, buttonHeight));
            noButton = new Rectangle(noPos.ToPoint(), new Point(buttonWidth, buttonHeight));

            // Get mouse position
            MouseState mouseState = Mouse.GetState();
            Point mousePos = mouseState.Position;

            // Change color on hover
            bool yesHover = yesButton.Contains(mousePos);
            bool noHover = noButton.Contains(mousePos);

            // Play hover sound when mouse enters button (not when leaving or staying)
            if (yesHover && !wasHoveringYes)
                SoundPlayer.Play("hover");

            if (noHover && !wasHoveringNo)
                SoundPlayer.Play("hover");

            // Update previous hover states for next frame
            wasHoveringYes = yesHover;
            wasHoveringNo = noHover;

            // Draw button backgrounds if hovered
            if (yesHover)
                spriteBatch.Draw(whiteTexture, yesButton, Color.White * 0.25f);
            if (noHover)
                spriteBatch.Draw(whiteTexture, noButton, Color.White * 0.25f);

            // Draw button text
            spriteBatch.DrawString(Game1.PixelFont, "Yes", yesPos, Game1.Colors.DarkRed, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
            spriteBatch.DrawString(Game1.PixelFont, "No", noPos, Game1.Colors.DarkRed, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);
        }
    }
}