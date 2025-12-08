using Microsoft.Xna.Framework.Content;

namespace Honeyspur;

public class Game1 : Game
{
    // Static properties for game-wide access
    public static Game1 Instance { get; private set; }
    public static GraphicsDeviceManager Graphics { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }
    public static GameWindow GameWindow { get; private set; }
    public static ContentManager ContentManager { get; private set; }
    public static SpriteFont PixelFont { get; private set; }
    public static SpriteFont BorderFont { get; private set; }
    public static int ScreenWidth { get; private set; }
    public static int ScreenHeight { get; private set; }
    public static Texture2D CursorTexture { get; private set; }
    public static Rectangle CursorArrowRect { get; private set; }
    public static Rectangle CursorHandRect { get; private set; }
    public static bool UseHandCursor { get; set; } = false;

    // Color palette for consistent theming
    public static class Colors
    {
        public static readonly Color DarkRed = new(69, 15, 0);
        public static readonly Color LightPink = new(222, 133, 220, 255);
        public static readonly Color DarkPurple = new(84, 46, 223);
        public static readonly Color LightOrange = new(246, 170, 62);
    }

    // Game state management
    public enum GameState { Title, Main, Paused }
    private GameState currentState = GameState.Title;

    private readonly ScreenFader screenFader = new();
    private TitleScreen titleScreen;
    private MainGame mainGame;

    public Game1()
    {
        Instance = this;
        Graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        ContentManager = Content;
        GameWindow = Window;

        // Configure display
        Graphics.IsFullScreen = true;
        var adapter = GraphicsAdapter.DefaultAdapter;
        var mode = adapter.CurrentDisplayMode;
        ScreenWidth = mode.Width;
        ScreenHeight = mode.Height;
        Graphics.PreferredBackBufferWidth = ScreenWidth;
        Graphics.PreferredBackBufferHeight = ScreenHeight;
        Graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        TextureManager.Initialize(Content);
        SpriteBatch = new SpriteBatch(GraphicsDevice);
        titleScreen = new TitleScreen();
        mainGame = new MainGame();

        // Load core game content
        titleScreen.LoadContent(Content);
        mainGame.LoadContent(Content, GraphicsDevice);
        PixelFont = Content.Load<SpriteFont>("fonts/PixelPurl");
        BorderFont = Content.Load<SpriteFont>("fonts/Border");
        CursorTexture = TextureManager.GetTexture("graphics/ui");
        CursorArrowRect = new Rectangle(240, 0, 32, 32);
        CursorHandRect = new Rectangle(272, 0, 32, 32);
    }

    protected override void UnloadContent()
    {
        // Clear static resources
        TextureManager.UnloadAll();
        SpriteBatch?.Dispose();
        SpriteBatch = null;
        PixelFont = null;
        BorderFont = null;
        ContentManager = null;
        Instance = null;

        // Unload screens
        titleScreen = null;
        mainGame = null;

        // Unload all content
        Content.Unload();
        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        screenFader.Update(gameTime);
        if (screenFader.IsFading)
        {
            base.Update(gameTime);
            return;
        }

        if (currentState == GameState.Title)
        {
            titleScreen.Update(gameTime, GraphicsDevice);
            if (titleScreen.StartPressed)
            {
                SoundPlayer.Play("ding");
                screenFader.StartFadeOut(1.5f, () =>
                {
                    currentState = GameState.Main;
                    mainGame.Start();
                });
            }
            else if (titleScreen.ExitPressed)
            {
                Exit();
            }
        }
        else if (currentState == GameState.Main)
        {
            mainGame.Update(gameTime);
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        if (currentState == GameState.Title)
            titleScreen.Draw(SpriteBatch, GraphicsDevice);
        else if (currentState == GameState.Main)
            mainGame.Draw(SpriteBatch, GraphicsDevice);

        screenFader.Draw(SpriteBatch, GraphicsDevice);
        base.Draw(gameTime);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        MouseState mouse = Mouse.GetState();
        Vector2 cursorPos = new(mouse.X, mouse.Y);

        Rectangle sourceRect = UseHandCursor ? CursorHandRect : CursorArrowRect;

        Vector2 cursorOffset = new(21, 15);
        SpriteBatch.Draw(CursorTexture, cursorPos - cursorOffset, sourceRect, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        SpriteBatch.End();
    }
}
