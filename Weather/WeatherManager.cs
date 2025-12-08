using Microsoft.Xna.Framework.Audio;
namespace Honeyspur;

public class WeatherManager
{
    public enum WeatherType { Sunny, Rain, Snow }
    public WeatherType CurrentWeather { get; set; } = WeatherType.Sunny;
    private readonly RainEffect rainEffect = new();
    private readonly SnowEffect snowEffect = new();
    private readonly ButterflyEffect butterflyEffect = new();
    private SoundEffect rainSoundEffect;
    private SoundEffectInstance rainSoundInstance;

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager content, int width, int height)
    {
        rainEffect.LoadContent(content, width, height);
        snowEffect.LoadContent(content, width, height);
        butterflyEffect.LoadContent(TextureManager.GetTexture("graphics/weather"), width, height);

        rainSoundEffect = content.Load<SoundEffect>("sounds/rain");
        rainSoundInstance = rainSoundEffect.CreateInstance();
        rainSoundInstance.IsLooped = true;
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        // Manage rain sound
        bool isRaining = CurrentWeather == WeatherType.Rain;
        bool isSnowing = CurrentWeather == WeatherType.Snow;

        if (isRaining && rainSoundInstance.State != SoundState.Playing)
            rainSoundInstance.Play();
        else if (!isRaining && rainSoundInstance.State == SoundState.Playing)
            rainSoundInstance.Stop();

        // Update visuals only if outside
        MapType mapType = context.MapManager.CurrentMap.Type;
        if (mapType == MapType.Outside)
        {
            if (isRaining)
                rainEffect.Update(gameTime);
            else if (isSnowing)
                snowEffect.Update(gameTime);
            else if (CurrentWeather == WeatherType.Sunny)
                butterflyEffect.Update(gameTime);
        }
    }

    public void Draw(SpriteBatch spriteBatch, MapType mapType)
    {
        if (mapType == MapType.Inside) return;
        if (CurrentWeather == WeatherType.Rain)
            rainEffect.Draw(spriteBatch);
        else if (CurrentWeather == WeatherType.Snow)
            snowEffect.Draw(spriteBatch);
        else if (CurrentWeather == WeatherType.Sunny)
            butterflyEffect.Draw(spriteBatch);
    }

    public bool IsWetWeather()
    {
        if (CurrentWeather == WeatherType.Rain || CurrentWeather == WeatherType.Snow)
            return true;
        return false;
    }

    public void GetNewWeather(GameContext context)
    {
        var rand = new Random();

        // In Winter allow only Sunny or Snow. Otherwise allow only Sunny or Rain.
        if (context?.DayManager?.currentSeason == DayManager.Season.Winter)
        {
            var options = new[] { WeatherType.Sunny, WeatherType.Snow };
            CurrentWeather = options[rand.Next(options.Length)];
        }
        else
        {
            var options = new[] { WeatherType.Sunny, WeatherType.Rain };
            CurrentWeather = options[rand.Next(options.Length)];
        }
    }
}
