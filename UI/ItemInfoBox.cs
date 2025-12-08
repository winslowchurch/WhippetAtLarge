using Honeyspur.Items;

namespace Honeyspur.UI
{
    public class ItemInfoBox
    {
        private Texture2D uiTexture;
        private UIAtlas uiAtlas;
        private ItemInstance currentItem;
        private bool isVisible;

        public void LoadContent(Texture2D sharedUiTexture, UIAtlas sharedUiAtlas)
        {
            uiTexture = sharedUiTexture;
            uiAtlas = sharedUiAtlas;
        }

        public void SetItem(ItemInstance item)
        {
            currentItem = item;
            isVisible = item != null;
        }

        public void Draw(SpriteBatch spriteBatch, Viewport viewport)
        {
            if (!isVisible || currentItem == null) return;

            MouseState mouseState = Mouse.GetState();
            Vector2 mousePosition = new(mouseState.X, mouseState.Y);

            float scale = 4f;

            // Get item info box rectangle from atlas
            Rectangle itemInfoBoxSrc = uiAtlas.GetRect("itemInfoBox");

            // Calculate box dimensions
            float boxWidth = itemInfoBoxSrc.Width * scale;
            float boxHeight = itemInfoBoxSrc.Height * scale;

            // Position the box slightly to the left of the mouse cursor
            Vector2 position = new(
                mousePosition.X - boxWidth - 10,
                MathHelper.Clamp(
                    mousePosition.Y - boxHeight / 2,
                    0,
                    viewport.Height - boxHeight - 10
                )
            );

            // Draw background
            spriteBatch.Draw(uiTexture, position, itemInfoBoxSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Calculate available space for text
            float availableWidth = (itemInfoBoxSrc.Width - 10) * scale; // 5 pixels padding on each side
            float availableHeight = 14f * scale; // Approximate space for name

            // Start with default text scale and adjust if needed
            float textScale = 0.15f * scale;
            Vector2 textSize = Game1.PixelFont.MeasureString(currentItem.Name) * textScale;

            // Reduce scale if text is too big
            while (textSize.X > availableWidth || textSize.Y > availableHeight)
            {
                textScale *= 0.9f;
                textSize = Game1.PixelFont.MeasureString(currentItem.Name) * textScale;
            }

            // Draw item name
            Vector2 textPos = position + new Vector2(5 * scale, 5 * scale);
            spriteBatch.DrawString(Game1.PixelFont, currentItem.Name, textPos, Color.Black, 0f, Vector2.Zero, textScale, SpriteEffects.None, 0f);

            // Draw category
            Vector2 categoryPos = position + new Vector2(5 * scale, 19 * scale);
            spriteBatch.DrawString(Game1.PixelFont, currentItem.Category.ToString(), categoryPos, Color.Black, 0f, Vector2.Zero, 0.12f * scale, SpriteEffects.None, 0f);

            // Draw health value with heart icon if applicable
            if (currentItem.HealthValue > 0)
            {
                Vector2 healthPos = position + new Vector2(16 * scale, 27 * scale);

                // Get heart icon rectangle from atlas
                Rectangle heartIconSrc = uiAtlas.GetRect("heartIcon");
                Vector2 iconPos = healthPos - new Vector2(10 * scale, 0);
                spriteBatch.Draw(uiTexture, iconPos, heartIconSrc, Color.White, 0f, Vector2.Zero, scale * 0.75f, SpriteEffects.None, 0f);

                // Draw health value
                spriteBatch.DrawString(Game1.PixelFont, $"+{currentItem.HealthValue}", healthPos, Color.Black, 0f, Vector2.Zero, 0.12f * scale, SpriteEffects.None, 0f);
            }

            // Draw description if available
            if (!string.IsNullOrWhiteSpace(currentItem.Description))
            {
                // Start Y position: below health line if present, else below category line
                float startY = currentItem.HealthValue > 0 ? 39 * scale : 31 * scale; // a bit of spacing
                Vector2 descPos = position + new Vector2(5 * scale, startY);

                // We will render description potentially across multiple lines if it exceeds available width.
                float descMaxWidth = (itemInfoBoxSrc.Width - 10) * scale;
                string description = currentItem.Description;

                // Use same scale as name (textScale) per requirement
                float descScale = 0.1f * scale;

                // Simple word wrap - limit to 2 lines
                List<string> lines = [];
                string[] words = description.Split(' ');
                string currentLine = string.Empty;
                foreach (var word in words)
                {
                    if (lines.Count >= 2) break; // Limit to 2 lines max
                    
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    Vector2 size = Game1.PixelFont.MeasureString(testLine) * descScale;
                    if (size.X > descMaxWidth && !string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }
                if (!string.IsNullOrEmpty(currentLine) && lines.Count < 2) lines.Add(currentLine);

                // Draw each wrapped line
                float lineHeight = Game1.PixelFont.LineSpacing * descScale;
                for (int i = 0; i < Math.Min(lines.Count, 2); i++)
                {
                    spriteBatch.DrawString(Game1.PixelFont, lines[i], descPos + new Vector2(0, i * lineHeight), Color.Black, 0f, Vector2.Zero, descScale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
