using Microsoft.Xna.Framework.Content;
namespace Honeyspur;

// Move wall furniture (in range update too based on tile type)
// work computer player lagging when camera moves 
// figure out bitmap font so its not fuzzy

// tile / wood / carpet footsteps

public class GameContext
{
    public MapManager MapManager { get; set; }
    public Player Player { get; set; }
    public Camera2D Camera { get; set; }
    public WeatherManager WeatherManager { get; set; }
    public DayManager DayManager { get; set; }
    public LightingManager LightingManager { get; set; }
    public UIManager UIManager { get; set; }
    public ScreenFader ScreenFader { get; set; }
    public DayTransitionScreen DayTransitionScreen { get; set; }
    public GraphicsDevice GraphicsDevice { get; set; }
    public List<DroppedItem> DroppedItems = [];
    public bool IsFrozen { get; private set; } = false;

    public void FreezeGame() => IsFrozen = true;
    public void UnfreezeGame() => IsFrozen = false;
}

public class MainGame
{
    private readonly GameContext context = new();
    private InputManager inputManager;
    private bool started = false;

    public void Start()
    {
        started = true;
    }

    public void LoadContent(ContentManager Content, GraphicsDevice graphicsDevice)
    {
        Items.ItemRegistry.LoadContent(Content);
        context.MapManager = new MapManager();
        context.Player = new Player();
        context.Camera = new Camera2D();
        context.WeatherManager = new WeatherManager();
        context.DayManager = new DayManager();
        context.LightingManager = new LightingManager();
        context.UIManager = new UIManager();
        DroppedItem.LoadContent(Content);
        context.ScreenFader = new ScreenFader();
        context.DayTransitionScreen = new DayTransitionScreen();
        context.GraphicsDevice = graphicsDevice;
        inputManager = new InputManager(context);

        SoundPlayer.Initialize(Content);
        context.MapManager.LoadContent(Content);
        context.Player.LoadContent(Content);
        context.WeatherManager.LoadContent(Content, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        context.LightingManager.LoadContent(graphicsDevice);
        context.UIManager.LoadContent(Content, graphicsDevice);
        context.DayTransitionScreen.LoadContent(Content);

        context.MapManager.PrecacheAllMaps(Maps.All, context);
        LoadMap(Maps.DotHouse, new Vector2(116, 96));
    }

    public void LoadMap(MapData map, Vector2 spawnPos)
    {
        context.MapManager.LoadMap(map, context);

        context.Player.Position = spawnPos;

        context.LightingManager.SetWeatherType(context.WeatherManager.CurrentWeather);
        context.Camera.CenterAndClampToPlayer(context, context.GraphicsDevice);
    }

    public void Update(GameTime gameTime)
    {
        context.ScreenFader.Update(gameTime);
        context.DayTransitionScreen.Update(gameTime, context);

        if (!started) return;

        // If transition screen is active, skip normal game updates
        if (context.DayTransitionScreen.IsActive)
            return;

        //ui are handled even when frozen.
        inputManager.Update(context);
        context.UIManager.Update(gameTime, context);

        // Only update player, camera, and world if not frozen
        if (!context.IsFrozen)
        {
            UpdatePlayerAndCamera(gameTime);
            UpdateWorld(gameTime);
        }
    }

    private void UpdatePlayerAndCamera(GameTime gameTime)
    {
        context.Player.Update(gameTime, context);
        context.UIManager.Update(gameTime, context);
        context.Camera.CenterAndClampToPlayer(context, context.GraphicsDevice);
    }

    private void UpdateWorld(GameTime gameTime)
    {
        // Check for map exits
        foreach (var exit in context.MapManager.CurrentMap.Exits)
        {
            if (exit.Area.Intersects(context.Player.GetCollisionRect()))
            {
                LoadMap(exit.DestinationMap, exit.DestinationSpawn);
                return; // Stop further updates this frame
            }
        }

        //context.MapManager.UpdateAllTiles(gameTime);
        context.WeatherManager.Update(gameTime, context);
        context.LightingManager.Update(gameTime, context);
        context.DayManager.Update(gameTime, context);
        DroppedItem.UpdateAll(context, gameTime);
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: context.Camera.GetViewMatrix());

        // Draw all tiles and objects sorted by Y
        context.MapManager.DrawAllSorted(spriteBatch, context);
        MouseHelper.DrawPlacementPreview(context, spriteBatch);
        DroppedItem.DrawAll(context.DroppedItems, spriteBatch);

        // weather and lighting
        context.WeatherManager.Draw(spriteBatch, context.MapManager.CurrentMap.Type);
        context.LightingManager.Draw(spriteBatch, graphicsDevice, context.Camera.Position, context.MapManager.LightTiles, context.DayManager);
        spriteBatch.End();

        // non-camera dependting things, like ui
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        context.UIManager.Draw(spriteBatch, graphicsDevice.Viewport, context);
        spriteBatch.End();

        context.ScreenFader.Draw(spriteBatch, graphicsDevice);

        // Draw day transition screen on top of everything
        context.DayTransitionScreen.Draw(spriteBatch, graphicsDevice);
    }
}