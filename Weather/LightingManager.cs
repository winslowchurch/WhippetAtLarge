using Microsoft.Xna.Framework.Content;
using static Honeyspur.WeatherManager;

namespace Honeyspur;

public class LightingManager
{
    private WeatherType currentWeatherType;
    private Texture2D blackTexture;
    private Texture2D debugDotTexture;

    private int flickerIndex;
    private float flickerTimer;
    private const float FlickerInterval = 0.5f;
    private static readonly float[] FlickerPattern = [-1f, -3f, -1.5f, 0f];

    public bool ShowDebugLightPositions = false;

    public void LoadContent(GraphicsDevice graphicsDevice)
    {
        // 1x1 white pixel for tinting shadows or glow
        blackTexture = new Texture2D(graphicsDevice, 1, 1);
        blackTexture.SetData([Color.White]);

        // Small red dot for debug light visualization
        debugDotTexture = new Texture2D(graphicsDevice, 4, 4);
        Color[] dotData = new Color[16];
        Array.Fill(dotData, Color.Red);
        debugDotTexture.SetData(dotData);
    }

    public void SetWeatherType(WeatherType type) => currentWeatherType = type;

    public float GetCurrentDarkness(DayManager dayManager)
    {
        float baseDarkness = currentWeatherType == WeatherType.Rain ? 0.6f : 0f;
        float timeDarkness = dayManager.GetDarknessTransition();
        return Math.Max(baseDarkness, timeDarkness);
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        context.DayManager.Update(gameTime, context);
        SetWeatherType(context.WeatherManager.CurrentWeather);

        flickerTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (flickerTimer >= FlickerInterval)
        {
            flickerTimer = 0f;
            flickerIndex = (flickerIndex + 1) % FlickerPattern.Length;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Vector2 cameraPosition, List<TileInstance> lightTiles, DayManager dayManager)
    {
        int sampleSize = 4;
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;

        // Snap starting point slightly before camera to cover edges
        Vector2 topLeftWorld = new(
            (float)Math.Floor(cameraPosition.X / sampleSize) * sampleSize - sampleSize,
            (float)Math.Floor(cameraPosition.Y / sampleSize) * sampleSize - sampleSize
        );

        // Add *extra* coverage in both directions
        int stepsX = screenWidth / sampleSize + 4;
        int stepsY = screenHeight / sampleSize + 4;

        float globalDarkness = GetCurrentDarkness(dayManager);

        for (int y = 0; y < stepsY; y++)
        {
            for (int x = 0; x < stepsX; x++)
            {
                Vector2 samplePos = topLeftWorld + new Vector2(x * sampleSize, y * sampleSize);
                float minAlpha = globalDarkness;
                Color flameTint = Color.Transparent;

                foreach (var light in lightTiles)
                {
                    if (!light.IsOn) continue;

                    float baseRadius = light.LightType switch
                    {
                        LightType.Flame => 65f,
                        LightType.Bulb => 55f,
                        _ => 32f
                    };

                    float radius = baseRadius + (light.LightType == LightType.Flame ? FlickerPattern[flickerIndex] : 0f);

                    Vector2 lightWorldPos = new Vector2(light.Position.X * TileDefinitions.TileSize, light.Position.Y * TileDefinitions.TileSize) + light.LightOffset;
                    float dist = Vector2.Distance(samplePos, lightWorldPos);

                    if (dist >= radius) continue;

                    float t = MathHelper.Clamp(1f - (dist / radius), 0f, 1f);
                    float alpha = MathHelper.Lerp(0.1f, 0.85f, 1f - t);
                    minAlpha = Math.Min(minAlpha, alpha);

                    if (light.LightType == LightType.Flame)
                    {
                        Color orange = new Color(1f, 0.4f, 0.1f) * (t * 0.5f);
                        flameTint = Color.Lerp(flameTint, orange, t * 0.5f);
                    }
                }

                DrawLightSample(spriteBatch, samplePos, sampleSize, minAlpha, flameTint);
            }
        }

        if (ShowDebugLightPositions)
            DrawDebugLights(spriteBatch, lightTiles);
    }

    private void DrawLightSample(SpriteBatch spriteBatch, Vector2 position, int size, float shadowAlpha, Color flameTint)
    {
        if (shadowAlpha > 0f)
        {
            spriteBatch.Draw(
                blackTexture,
                position,
                null,
                new Color(0f, 0f, 0f, shadowAlpha),
                0f,
                Vector2.Zero,
                size,
                SpriteEffects.None,
                1f
            );
        }

        if (flameTint != Color.Transparent)
        {
            spriteBatch.Draw(
                blackTexture,
                position,
                null,
                flameTint,
                0f,
                Vector2.Zero,
                size,
                SpriteEffects.None,
                0.9f
            );
        }
    }

    private void DrawDebugLights(SpriteBatch spriteBatch, List<TileInstance> lightTiles)
    {
        foreach (var light in lightTiles)
        {
            Vector2 lightWorldPos = new Vector2(light.Position.X * TileDefinitions.TileSize, light.Position.Y * TileDefinitions.TileSize) + light.LightOffset;
            spriteBatch.Draw(
                debugDotTexture,
                lightWorldPos - new Vector2(debugDotTexture.Width / 2f),
                Color.White
            );
        }
    }
}

public enum LightType
{
    None,
    Bulb,
    Flame
}
