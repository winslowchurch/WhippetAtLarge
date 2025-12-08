namespace Honeyspur;

public class ChurchOuterDoor : TileInstance
{
    public ChurchOuterDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.Church, context);
            context.Player.SetTilePosition(6, 14);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}

public class ChurchInnerDoor : TileInstance
{
    public ChurchInnerDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.BeechForest, context);
            context.Player.SetTilePosition(20, 15);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}

public class DotsHouseOuterDoor : TileInstance
{
    public DotsHouseOuterDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.DotHouse, context);
            context.Player.SetTilePosition(15, 9);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}

public class DotsHouseInnerDoor : TileInstance
{
    public DotsHouseInnerDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.Town, context);
            context.Player.SetTilePosition(20, 15);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}

public class CabinOuterDoor : TileInstance
{
    public CabinOuterDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.House, context);
            context.Player.SetTilePosition(3, 8);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}

public class CabinInnerDoor : TileInstance
{
    public CabinInnerDoor() { IsInteractable = true; }
    public override bool OnRightClick(GameContext context)
    {
        SoundPlayer.Play("door");
        context.ScreenFader.StartFadeOut(1.5f, () =>
        {
            context.MapManager.LoadMap(Maps.HomeBase, context);
            context.Player.SetTilePosition(14, 7);
            context.ScreenFader.StartFadeIn(0.5f);
        });
        return true;
    }
}