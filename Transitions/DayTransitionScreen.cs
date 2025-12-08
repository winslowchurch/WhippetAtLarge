using Microsoft.Xna.Framework.Content;
using System.IO;

namespace Honeyspur;

public class DayTransitionScreen
{
    private bool isActive = false;
    private string currentDateText = "";
    private string nextDateText = "";
    private bool showButton = false;
    private float transitionTimer = 0f;
    private const float DateChangeDelay = 1.5f; // Delay before showing the new date
    private const float ButtonShowDelay = 1f; // Additional delay before showing button

    private Rectangle startButtonRect;
    private Rectangle exitButtonRect;
    private bool startButtonHovered = false;
    private bool exitButtonHovered = false;
    private bool wasStartButtonHovered = false;
    private bool wasExitButtonHovered = false;
    private SpriteFont font;
    private ButtonState previousLeftButtonState;

    // Sound effect flags
    private bool dateFlipSoundPlayed = false;
    private bool buttonWooshSoundPlayed = false;

    private Texture2D uiTexture;
    private UIAtlas uiAtlas;

    public bool IsActive => isActive;

    public void LoadContent(ContentManager Content)
    {
        font = Content.Load<SpriteFont>("fonts/PixelPurl");
        uiTexture = TextureManager.GetTexture("graphics/ui");
        uiAtlas = new UIAtlas();
        uiAtlas.Load(Path.Combine("data", "ui_atlas.json"));
    }

    public void Show(string currentDate, string nextDate)
    {
        isActive = true;
        currentDateText = currentDate;
        nextDateText = nextDate;
        showButton = false;
        transitionTimer = 0f;
        previousLeftButtonState = Microsoft.Xna.Framework.Input.ButtonState.Released;
        
        // Reset sound flags
        dateFlipSoundPlayed = false;
        buttonWooshSoundPlayed = false;
        wasStartButtonHovered = false;
        wasExitButtonHovered = false;
    }

    public void Hide()
    {
        isActive = false;
        showButton = false;
        transitionTimer = 0f;
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        if (!isActive) return;

        transitionTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Play flip sound when date switches
        if (transitionTimer >= DateChangeDelay && !dateFlipSoundPlayed)
        {
            SoundPlayer.Play("flip");
            dateFlipSoundPlayed = true;
        }

        // Show button after delays
        if (transitionTimer >= DateChangeDelay + ButtonShowDelay && !showButton)
        {
            showButton = true;
            
            // Play woosh sound when button appears
            if (!buttonWooshSoundPlayed)
            {
                SoundPlayer.Play("woosh");
                buttonWooshSoundPlayed = true;
            }
        }

        // Update button hover state
        if (showButton)
        {
            var mouseState = Mouse.GetState();
            Point mousePos = new(mouseState.X, mouseState.Y);
            startButtonHovered = startButtonRect.Contains(mousePos);
            exitButtonHovered = exitButtonRect.Contains(mousePos);

            // Play hover sound when mouse enters button (not when leaving or staying)
            if (startButtonHovered && !wasStartButtonHovered)
                SoundPlayer.Play("hover");
            
            if (exitButtonHovered && !wasExitButtonHovered)
                SoundPlayer.Play("hover");

            // Update previous hover states for next frame
            wasStartButtonHovered = startButtonHovered;
            wasExitButtonHovered = exitButtonHovered;

            // Set hand cursor if hovering over any button
            Game1.UseHandCursor = startButtonHovered || exitButtonHovered;

            // Check for button click (detect the moment the button is pressed, not held)
            bool wasClicked = mouseState.LeftButton == ButtonState.Pressed &&
                              previousLeftButtonState == ButtonState.Released;

            if (wasClicked)
            {
                if (startButtonHovered)
                    OnStartButtonClicked(context);
                else if (exitButtonHovered)
                    OnExitButtonClicked();
            }

            // Update previous state for next frame
            previousLeftButtonState = mouseState.LeftButton;
        }
        else
            Game1.UseHandCursor = false;
    }

    private void OnStartButtonClicked(GameContext context)
    {
        SoundPlayer.Play("click");
        // Hide the transition screen
        Hide();

        // Reopen player's eyes
        context.Player?.ReopenEyes();

        // Start fade in back to gameplay
        context.ScreenFader.StartFadeIn(1.0f);

        // Unfreeze the game
        context.UnfreezeGame();
    }

    private static void OnExitButtonClicked()
    {
        // Exit the game
        Game1.Instance.Exit();
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!isActive) return;

        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;

        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw black background
        Texture2D blackTexture = new(graphicsDevice, 1, 1);
        blackTexture.SetData([Color.Black]);
        spriteBatch.Draw(blackTexture,
            new Rectangle(0, 0, screenWidth, screenHeight),
            Color.Black);

        // Determine which date to show
        string dateToShow = transitionTimer < DateChangeDelay ? currentDateText : nextDateText;

        // Draw date text centered
        Vector2 dateSize = font.MeasureString(dateToShow);
        Vector2 datePosition = new(
            (screenWidth - dateSize.X) / 2,
            (screenHeight / 2) - 60
        );
        spriteBatch.DrawString(font, dateToShow, datePosition, Color.White);

        // Draw buttons if ready
        if (showButton)
        {
            float buttonScale = 6f;
            Rectangle orangeButtonSrc = uiAtlas.GetRect("orangeButton");
            Rectangle purpleButtonSrc = uiAtlas.GetRect("purpleButton");

            // Calculate button positions - bottom right corner
            int buttonSpacing = 20;
            int padding = 40; // Padding from screen edges
            int totalButtonsWidth = (int)(orangeButtonSrc.Width * buttonScale * 2) + buttonSpacing;
            int startX = screenWidth - totalButtonsWidth - padding;
            int buttonY = screenHeight - (int)(orangeButtonSrc.Height * buttonScale) - padding;

            // Exit button (left in the pair, but still bottom right area)
            exitButtonRect = new Rectangle(
                startX,
                buttonY,
                (int)(orangeButtonSrc.Width * buttonScale),
                (int)(orangeButtonSrc.Height * buttonScale)
            );

            // Start New Day button (right in the pair)
            startButtonRect = new Rectangle(
                startX + (int)(orangeButtonSrc.Width * buttonScale) + buttonSpacing,
                buttonY,
                (int)(orangeButtonSrc.Width * buttonScale),
                (int)(orangeButtonSrc.Height * buttonScale)
            );

            // Draw Exit button
            Rectangle exitButtonSrc = exitButtonHovered ? purpleButtonSrc : orangeButtonSrc;
            spriteBatch.Draw(uiTexture, exitButtonRect, exitButtonSrc, Color.White);

            // Draw Start New Day button
            Rectangle startButtonSrc = startButtonHovered ? purpleButtonSrc : orangeButtonSrc;
            spriteBatch.Draw(uiTexture, startButtonRect, startButtonSrc, Color.White);

            // Draw button text
            string exitButtonText = "Exit";
            Vector2 exitTextSize = font.MeasureString(exitButtonText);
            float exitTextScale = 0.8f;
            Color exitTextColor = exitButtonHovered ? Game1.Colors.DarkPurple : Game1.Colors.DarkRed;
            Vector2 exitTextPosition = new(
                exitButtonRect.X + (exitButtonRect.Width - exitTextSize.X * exitTextScale) / 2,
                exitButtonRect.Y + (exitButtonRect.Height - exitTextSize.Y * exitTextScale) / 2 + 4
            );
            spriteBatch.DrawString(font, exitButtonText, exitTextPosition, exitTextColor, 0f, Vector2.Zero, exitTextScale, SpriteEffects.None, 0f);

            string startButtonText = "Start Day";
            Vector2 startTextSize = font.MeasureString(startButtonText);
            float startTextScale = 0.7f;
            Color startTextColor = startButtonHovered ? Game1.Colors.DarkPurple : Game1.Colors.DarkRed;
            Vector2 startTextPosition = new(
                startButtonRect.X + (startButtonRect.Width - startTextSize.X * startTextScale) / 2,
                startButtonRect.Y + (startButtonRect.Height - startTextSize.Y * startTextScale) / 2 + 4
            );
            spriteBatch.DrawString(font, startButtonText, startTextPosition, startTextColor, 0f, Vector2.Zero, startTextScale, SpriteEffects.None, 0f);
        }

        spriteBatch.End();

        blackTexture.Dispose();
    }
}

