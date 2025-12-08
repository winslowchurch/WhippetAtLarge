using Honeyspur;

public class Bed : TileInstance
{
    public Bed() { IsInteractable = true; }
    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }

    public override bool OnRightClick(GameContext context)
    {
        context.UIManager.PromptBox.Show(
            "Go to sleep for the night?",
            () => Player.GoToSleep(context), context
        );
        return true;
    }

    // Draw two halves of the bed
    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        Vector2 position = new(Position.X * 16, Position.Y * 16);
        var def = TileDefinitions.All[BaseTileId];
        int xOffset = def.ImageWidth; // beds halves are always the same length
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, def, sourceOffset: new Point(xOffset, 0));
    }

    public void DrawTop(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        Vector2 position = new(Position.X * 16, Position.Y * 16);
        var topDef = TileDefinitions.All[BaseTileId];
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, topDef);
    }
}