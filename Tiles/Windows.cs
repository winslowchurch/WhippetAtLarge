namespace Honeyspur;

public abstract class WindowTile : TileInstance
{
    protected int[] FgIds;
    protected int BgDayId;
    protected int BgNightId;
    protected int _cycleIndex = 0;

    public WindowTile() { IsInteractable = true; }

    public override bool OnRightClick(GameContext context)
    {
        _cycleIndex = (_cycleIndex + 1) % FgIds.Length;
        BaseTileId = FgIds[_cycleIndex];
        return true;
    }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        Vector2 position = new(Position.X * 16, Position.Y * 16);

        var bgDef = TileDefinitions.All[BgDayId];
        var fgDef = TileDefinitions.All[BaseTileId];

        bool isDay = context.DayManager.IsDaytime();
        int seasonOffset = context.DayManager.currentSeason switch
        {
            DayManager.Season.Autumn => 2,
            DayManager.Season.Winter => 3,
            _ => 0
        };

        int xOffset = bgDef.ImageWidth * (isDay ? seasonOffset : 1);
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, bgDef, sourceOffset: new Point(xOffset, 0));

        // Draw window foreground as normal
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, fgDef);
    }
}

public class SmallWindowTile : WindowTile
{
    public SmallWindowTile()
    {
        FgIds = [107, 108, 109];
        BgDayId = 116;
        BaseTileId = FgIds[0];
    }
}

public class MediumWindowTile : WindowTile
{
    public MediumWindowTile()
    {
        FgIds = [110, 111, 112];
        BgDayId = 117;
        BaseTileId = FgIds[0];
    }
}

public class LargeWindowTile : WindowTile
{
    public LargeWindowTile()
    {
        FgIds = [113, 114, 115];
        BgDayId = 118;
        BaseTileId = FgIds[0];
    }
}
