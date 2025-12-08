using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Honeyspur.UI
{
    public class CoinBar
    {
        private Texture2D uiTexture;
        private UIAtlas atlas;
        private int currentCoins = 100;
        private int targetCoins = 0;
        private float coinTimer = 0f;
        private string clockTime = "6:00 AM";
        private readonly float coinInterval = 0.008f;

        public void LoadContent(Texture2D sharedUiTexture, UIAtlas sharedAtlas)
        {
            // Use shared UI texture and atlas from UIManager
            uiTexture = sharedUiTexture;
            atlas = sharedAtlas;
        }

    public void AddCoins(int coins)
    {
        targetCoins += coins;
        SoundPlayer.Play("coinJingle");
    }

        public void TickUpdate(GameTime gameTime, GameContext context)
        {
            if (currentCoins < targetCoins)
            {
                coinTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (coinTimer >= coinInterval)
                {
                    coinTimer = 0f;
                    currentCoins++;
                }
            }

            clockTime = context.DayManager.GetClockTime();
        }

        private string GetSeasonIconName(DayManager.Season season)
        {
            return season switch
            {
                DayManager.Season.Spring => "springIcon",
                DayManager.Season.Summer => "summerIcon",
                DayManager.Season.Autumn => "autumnIcon",
                DayManager.Season.Winter => "winterIcon",
                _ => "summerIcon" // Default fallback
            };
        }

        public void Draw(SpriteBatch spriteBatch, GameContext context)
        {
            Vector2 iconPosition = new(10, 10);
            float scale = 4f;
            float fontScale = 0.8f;

            // Determine text color based on frozen state
            Color textColor = context.IsFrozen ? Color.AntiqueWhite : Color.Black;

            // Draw coin bar and coins
            // Use atlas source rectangle for coinBar
            Rectangle coinSrc = atlas.GetRect("coinBar");
            spriteBatch.Draw(uiTexture, iconPosition, coinSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            string coinText = currentCoins.ToString();
            int digitCount = coinText.Length;
            float digitSlotWidth = 7f * scale + 2;
            float totalTextWidth = digitSlotWidth * digitCount * fontScale;
            float coinBarWidth = coinSrc.Width * scale;
            float rightEdge = iconPosition.X + coinBarWidth - 11;

            Vector2 textStartPos = new(rightEdge - totalTextWidth - 3, iconPosition.Y + 8);

            for (int i = 0; i < digitCount; i++)
            {
                string digit = coinText[i].ToString();
                Vector2 digitPos = new(textStartPos.X + i * digitSlotWidth * fontScale, textStartPos.Y);
                spriteBatch.DrawString(Game1.PixelFont, digit, digitPos, Color.Black, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
            }

            // Split time into digits and AM/PM
            string timeDigits = clockTime[..^3]; // Gets "5:15 " or "12:00 "
            string amPm = clockTime[^2..]; // Gets "AM" or "PM"

            // Use seasonal icon based on current season
            string seasonIconName = GetSeasonIconName(context.DayManager.currentSeason);
            Rectangle seasonIconSrc = atlas.GetRect(seasonIconName);
            Vector2 smallIconPos = new(iconPosition.X + (3 * scale), iconPosition.Y + (20 * scale));
            spriteBatch.Draw(uiTexture, smallIconPos, seasonIconSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Draw time digits
            Vector2 timePos = new(smallIconPos.X + (22 * scale), smallIconPos.Y + (3 * scale));
            spriteBatch.DrawString(Game1.PixelFont, timeDigits, timePos, textColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);

            // Draw AM/PM at fixed position
            // Calculate position based on widest possible time (12:00 )
            float maxTimeWidth = Game1.PixelFont.MeasureString("12:00 ").X * fontScale;
            Vector2 amPmPos = new(timePos.X + maxTimeWidth - 10f, timePos.Y);
            spriteBatch.DrawString(Game1.PixelFont, amPm, amPmPos, textColor, 0f, Vector2.Zero, fontScale, SpriteEffects.None, 0f);
        }
    }
}
