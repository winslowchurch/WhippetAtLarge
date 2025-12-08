using Honeyspur;

public class TitleScreen
{
    Texture2D title1, title2;
    Texture2D currentTitle;
    Texture2D titleLogo;

    Texture2D buttonTexture;  // Replace buttonFrame and buttonFrameHover with single texture
    readonly Rectangle normalSourceRect = new(0, 0, 38, 19);
    readonly Rectangle hoverSourceRect = new(0, 19, 38, 19);

    double titleSwapTimer = 0;

    public bool StartPressed { get; private set; }
    public bool ExitPressed { get; private set; }

    bool isHoveringStart = false;
    bool isHoveringExit = false;
    bool wasHoveringStart = false;
    bool wasHoveringExit = false;

    Rectangle startRect;
    Rectangle exitRect;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
    {
        title1 = TextureManager.GetTexture("graphics/misc/title1");
        title2 = TextureManager.GetTexture("graphics/misc/title2");
        currentTitle = title1;

        titleLogo = TextureManager.GetTexture("graphics/misc/logo");

        buttonTexture = TextureManager.GetTexture("graphics/ui");
    }

    public void Update(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        titleSwapTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
        if (titleSwapTimer > 800)
        {
            currentTitle = (currentTitle == title1) ? title2 : title1;
            titleSwapTimer = 0;
        }

        MouseState mouse = Mouse.GetState();
        Point mousePoint = new(mouse.X, mouse.Y);

        int buttonWidth = 38 * 7;
        int buttonHeight = 19 * 7;
        int spacing = 150;
        int centerX = graphicsDevice.Viewport.Width / 2;
        int buttonY = graphicsDevice.Viewport.Height - 220;

        startRect = new Rectangle(centerX - buttonWidth - spacing / 2, buttonY, buttonWidth, buttonHeight);
        exitRect = new Rectangle(centerX + spacing / 2, buttonY, buttonWidth, buttonHeight);

        isHoveringStart = startRect.Contains(mousePoint);
        isHoveringExit = exitRect.Contains(mousePoint);

        // Play hover sound when mouse enters button (not when leaving or staying)
        if (isHoveringStart && !wasHoveringStart)
            SoundPlayer.Play("hover");
        
        if (isHoveringExit && !wasHoveringExit)
            SoundPlayer.Play("hover");

        // Update previous hover states for next frame
        wasHoveringStart = isHoveringStart;
        wasHoveringExit = isHoveringExit;

        // --- Change cursor type based on hover state ---
        Game1.UseHandCursor = isHoveringStart || isHoveringExit;

        StartPressed = isHoveringStart && mouse.LeftButton == ButtonState.Pressed;
        ExitPressed = isHoveringExit && mouse.LeftButton == ButtonState.Pressed;
    }


    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        spriteBatch.Draw(currentTitle, graphicsDevice.Viewport.Bounds, Color.White);

        // Title logo
        Vector2 logoPosition = new Vector2(
            (graphicsDevice.Viewport.Width - titleLogo.Width) / 2,
            (graphicsDevice.Viewport.Height - titleLogo.Height) / 2 - 120
        );
        spriteBatch.Draw(titleLogo, logoPosition, Color.White);

        // --- START BUTTON ---
        Rectangle startSourceRect = isHoveringStart ? hoverSourceRect : normalSourceRect;
        spriteBatch.Draw(buttonTexture, startRect, startSourceRect, Color.White);

        Vector2 startTextSize = Game1.PixelFont.MeasureString("START");
        Vector2 startTextPos = new(
            startRect.X + 2 + (startRect.Width - startTextSize.X) / 2,
            startRect.Y + 7 + (startRect.Height - startTextSize.Y) / 2
        );
        spriteBatch.DrawString(Game1.PixelFont, "START", startTextPos, isHoveringStart ? Game1.Colors.DarkPurple : Game1.Colors.DarkRed);

        // --- EXIT BUTTON ---
        Rectangle exitSourceRect = isHoveringExit ? hoverSourceRect : normalSourceRect;
        spriteBatch.Draw(buttonTexture, exitRect, exitSourceRect, Color.White);

        Vector2 exitTextSize = Game1.PixelFont.MeasureString("EXIT");
        Vector2 exitTextPos = new Vector2(
            exitRect.X + 2 + (exitRect.Width - exitTextSize.X) / 2,
            exitRect.Y + 7 + (exitRect.Height - exitTextSize.Y) / 2
        );
        spriteBatch.DrawString(Game1.PixelFont, "EXIT", exitTextPos, isHoveringExit ? Game1.Colors.DarkPurple : Game1.Colors.DarkRed);

        // Credit
        string credit = "By: Winslow";
        Vector2 creditSize = Game1.PixelFont.MeasureString(credit) * 0.5f;
        spriteBatch.DrawString(Game1.PixelFont, credit,
            new Vector2(graphicsDevice.Viewport.Width - 10, graphicsDevice.Viewport.Height - 10) - creditSize,
            Game1.Colors.DarkRed, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);

        spriteBatch.End();
    }
}
