using Honeyspur;

public class Bridge : TileInstance
{
    public Bridge()
    {
        IsInteractable = true;
    }

    public override bool OnRightClick(GameContext context)
    {
        context.UIManager.AnnouncementBox.Show("The bridge is broken.", null, context);
        return true;
    }
}
