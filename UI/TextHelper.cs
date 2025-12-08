namespace Honeyspur.UI;

public static class TextHelper
{
    public static void DrawBorderedText(
        SpriteBatch spriteBatch,
        SpriteFont font,
        string text,
        Vector2 position,
        Color textColor,
        Color borderColor,
        float scale)
    {
        float borderOffset = scale * 4;

        // Draw border in all 8 surrounding positions
        Vector2[] offsets = [
            new Vector2(-borderOffset, -borderOffset), // top-left
            new Vector2(0, -borderOffset),             // top
            new Vector2(borderOffset, -borderOffset),  // top-right
            new Vector2(-borderOffset, 0),             // left
            new Vector2(borderOffset, 0),              // right
            new Vector2(-borderOffset, borderOffset),  // bottom-left
            new Vector2(0, borderOffset),              // bottom
            new Vector2(borderOffset, borderOffset)    // bottom-right
        ];

        foreach (var offset in offsets)
        {
            spriteBatch.DrawString(
                font,
                text,
                position + offset,
                borderColor,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );
        }

        // Draw main text on top
        spriteBatch.DrawString(
            font,
            text,
            position,
            textColor,
            0,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}

