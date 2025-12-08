namespace Honeyspur;

public class DayManager
{
    private int daysPlayed = 0;
    private readonly float totalDayDuration = 960f; // 16 minutes in seconds
    public float CurrentTime = 650f;
    public float TotalElapsedTime => daysPlayed * totalDayDuration + CurrentTime;

    public enum Season
    {
        Spring = 0,
        Summer = 1,
        Autumn = 2,
        Winter = 3,
    }

    public const int DaysPerSeason = 28;

    public Season currentSeason = Season.Spring;
    private int dayOfSeason = 1;
    private int year = 1;

    public int DayOfSeason => dayOfSeason;
    public int Year => year;

    public float SunriseStart => GetSeasonalTime(GetSeasonSunriseHour(currentSeason) - 0.25f);
    public float SunriseEnd => GetSeasonalTime(GetSeasonSunriseHour(currentSeason) + 0.25f);
    public float SunsetStart => GetSeasonalTime(GetSeasonSunsetHour(currentSeason) - 0.25f);
    public float SunsetEnd => GetSeasonalTime(GetSeasonSunsetHour(currentSeason) + 0.25f);

    private float GetSeasonalTime(float hour) => (hour / 24f) * totalDayDuration;

    private static float GetSeasonSunriseHour(Season s) => s switch
    {
        Season.Spring => 6.5f,
        Season.Summer => 5.5f,
        Season.Autumn => 6.75f,
        Season.Winter => 7.5f,
        _ => 6.5f
    };

    private static float GetSeasonSunsetHour(Season s) => s switch
    {
        Season.Spring => 19f,
        Season.Summer => 20f,
        Season.Autumn => 18.25f,
        Season.Winter => 17f,
        _ => 19f
    };

    public void NewDay(GameContext context)
    {
        AdvanceCalendarDay();
        context.WeatherManager.GetNewWeather(context);
        context.MapManager.DayUpdateAllTiles(context);
        context.Player.Health = context.Player.MaxHealth;
    }

    private void AdvanceCalendarDay()
    {
        daysPlayed++;
        dayOfSeason++;
        if (dayOfSeason > DaysPerSeason)
        {
            dayOfSeason = 1;
            currentSeason = NextSeason(currentSeason);
            if (currentSeason == Season.Spring)
                year++;
        }
    }

    private static Season NextSeason(Season s)
        => (Season)(((int)s + 1) % 4);

    public bool IsDaytime() => CurrentTime > SunriseEnd && CurrentTime < SunsetStart;

    public float GetTime() => CurrentTime;

    public void FastForwardToHour(int hour)
    {
        hour = Math.Clamp(hour, 0, 23);
        CurrentTime = hour * (totalDayDuration / 24f);
    }

    public string GetDateString()
        => $"{currentSeason} {dayOfSeason}, Year {year}";

    public float GetDarknessTransition()
    {
        if (CurrentTime >= SunriseStart && CurrentTime <= SunriseEnd)
        {
            float t = (CurrentTime - SunriseStart) / (SunriseEnd - SunriseStart);
            return MathHelper.Lerp(0.85f, 0f, t);
        }

        if (CurrentTime >= SunsetStart && CurrentTime <= SunsetEnd)
        {
            float t = (CurrentTime - SunsetStart) / (SunsetEnd - SunsetStart);
            return MathHelper.Lerp(0f, 0.85f, t);
        }

        return IsDaytime() ? 0f : 0.85f;
    }

    public string GetClockTime()
    {
        float hoursFloat = (CurrentTime / totalDayDuration) * 24f;
        int hours = (int)hoursFloat;
        float minutesFloat = (hoursFloat - hours) * 60f;
        int minutes = (int)Math.Round(minutesFloat / 15) * 15;
        if (minutes == 60)
        {
            minutes = 0;
            hours = (hours + 1) % 24;
        }

        string period = hours >= 12 ? "PM" : "AM";
        int displayHours = hours == 0 ? 12 : (hours > 12 ? hours - 12 : hours);
        return $"{displayHours}:{minutes:D2} {period}";
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (CurrentTime > totalDayDuration)
        {
            CurrentTime -= totalDayDuration;

            // Trigger day transition screen
            string currentDate = GetDateString();
            NewDay(context);
            string nextDate = GetDateString();

            // Freeze game and show transition screen
            context.FreezeGame();
            context.ScreenFader.StartFadeOut(1.0f, () =>
            {
                context.DayTransitionScreen.Show(currentDate, nextDate);
            });
        }
    }
}
