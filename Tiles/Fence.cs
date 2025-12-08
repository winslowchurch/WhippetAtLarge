using Honeyspur;

public class Fence : TileInstance
{
    private readonly Texture2D decorTexture = TextureManager.GetTexture("graphics/decoration/outsideDecor");
    public Fence() { IsInteractable = true; }

    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        // Draw the base fence tile first
        var def = TileDefinitions.All[BaseTileId];
        Vector2 position = new(Position.X * 16, Position.Y * 16);
        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, def);

        int connectorWidth = 16;
        int connectorHeight = 32;

        Point abovePos = new(Position.X, Position.Y - 1);
        Point leftPos = new(Position.X - 1, Position.Y);

        // Check above
        var aboveTile = context.MapManager.GetObjectTileAt(abovePos);
        string textureKey = TileRenderer.GetSeasonalTextureKey(context, def.ImagePath);
        if (!tileTextures.TryGetValue(textureKey, out var tex))
            return;

        if (aboveTile is Fence)
        {
            // Choose connector row based on the neighbor's fence type (wood vs stone).
            var aboveDef = TileDefinitions.All[aboveTile.BaseTileId];
            int yOffset = aboveDef.Name.Contains("Stone") ? 32 : 0;
            var src = new Rectangle(96, yOffset, connectorWidth, connectorHeight);
            var destPos = position + new Vector2(0, -16);
            spriteBatch.Draw(tex, destPos, src, Color.White);
        }

        // Check left
        var leftTile = context.MapManager.GetObjectTileAt(leftPos);
        if (leftTile is Fence)
        {
            // Choose connector row based on the neighbor's fence type (wood vs stone).
            var leftDef = TileDefinitions.All[leftTile.BaseTileId];
            int yOffset = leftDef.Name.Contains("Stone") ? 32 : 0;
            var src = new Rectangle(108, yOffset, connectorWidth, connectorHeight);
            var destPos = position + new Vector2(-12, -16);
            spriteBatch.Draw(tex, destPos, src, Color.White);
        }
    }

    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }
}