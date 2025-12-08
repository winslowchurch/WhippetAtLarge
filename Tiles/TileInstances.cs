using System.Linq;
using Honeyspur;
using Honeyspur.Items;

public abstract class TileInstance
{
    public Point Position { get; set; }
    // Reference back to the MapManager that owns this tile so Draw can check neighbors.
    public MapManager Owner { get; set; }
    public string MapName { get; set; }
    public int BaseTileId { get; set; }
    public bool IsOn { get; set; } = false;
    public virtual bool OnLeftClick(GameContext context) { return false; }
    public virtual bool OnRightClick(GameContext context) { return false; }
    public virtual bool OnShear(GameContext context) { return false; }
    public virtual bool OnSpray(GameContext context) { return false; }
    public virtual bool OnHoe(GameContext context) { return false; }
    public virtual string GetImageName() => TileDefinitions.All[BaseTileId].ImagePath;
    public virtual LightType LightType => LightType.None;
    public virtual Vector2 LightOffset => Vector2.Zero;
    public bool IsInteractable { get; set; } = false;

    public virtual void DayUpdate(GameContext context) { }

    public virtual void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        var def = TileDefinitions.All[BaseTileId];
        TileRenderer.DrawTile(
            context,
            spriteBatch,
            tileTextures,
            new Vector2(Position.X * 16, Position.Y * 16),
            def
        );
    }

    public virtual bool PickUpItem(GameContext context)
    {
        // Generate the item that would be created
        Texture2D icon = TileRenderer.RenderTileIcon(context, BaseTileId);
        var item = ItemRegistry.GetFurnitureItem(TileDefinitions.All[BaseTileId].Name, icon);

        // Check if inventory has space before removing tile from map
        if (!context.UIManager.CanAddItem(item))
            return false;

        // Only remove from map if we can successfully add to inventory
        context.MapManager.ObjectTiles[Position.X, Position.Y] = null;

        // Remove from LightTiles
        context.MapManager.RemoveLightTile(this);

        // Remove collision if needed
        var def = TileDefinitions.All[BaseTileId];
        context.MapManager.UpdateCollisionGrid(Position, def, false);

        // Add to inventory (this should always succeed since we checked CanAddItem above)
        bool success = context.UIManager.AddItemToInventory(item);
        if (success)
        {
            SoundPlayer.Play("poppopUp");
            return true;
        }
        return false;
    }
}

public static class TileRenderer
{
    public static void DrawTile(
        GameContext context,
        SpriteBatch spriteBatch,
        Dictionary<string, Texture2D> tileTextures,
        Vector2 position,
        TileDefinition tileDef,
        Color? tint = null,
        Point? sourceOffset = null)
    {
        string textureKey = GetSeasonalTextureKey(context, tileDef.ImagePath);

        // 👇 use textureKey here, not tileDef.ImagePath
        if (!tileTextures.TryGetValue(textureKey, out var tex))
            return;

        Rectangle sourceRect = new(
            tileDef.ImageX,
            tileDef.ImageY,
            tileDef.ImageWidth,
            tileDef.ImageHeight
        );

        if (sourceOffset is not null)
        {
            sourceRect.X += sourceOffset.Value.X;
            sourceRect.Y += sourceOffset.Value.Y;
        }

        int defaultImageOffsetY = tileDef.ImageHeight - 16;
        int addedYOffset = tileDef.ImageOffset?.Y ?? 0;
        int imageOffsetY = defaultImageOffsetY + addedYOffset;
        int imageOffsetX = tileDef.ImageOffset?.X ?? 0;

        Color drawColor = tint ?? Color.White;
        spriteBatch.Draw(
            tex,
            new Vector2(position.X + imageOffsetX, position.Y - imageOffsetY),
            sourceRect,
            drawColor
        );
    }

    public static string GetSeasonalTextureKey(GameContext context, string originalPath)
    {
        if (context.DayManager.currentSeason != DayManager.Season.Winter)
            return originalPath;

        var swaps = new (string normal, string winter)[]
        {
            (TileDefinitions.BuildingsPath, TileDefinitions.WinterBuildingsPath),
            (TileDefinitions.OutsideDecorPath, TileDefinitions.WinterOutsideDecorPath),
            (TileDefinitions.TerrainPath, TileDefinitions.WinterTerrainPath),
        };

        foreach (var (normal, winter) in swaps)
        {
            if (originalPath.StartsWith(normal))
                return originalPath.Replace(normal, winter);
        }

        return originalPath;

    }

    public static Texture2D RenderTileIcon(GameContext context, int tileId)
    {
        var tileDef = TileDefinitions.All[tileId];
        
        // Check if this is furniture - if so, render a scaled-down version
        var tileName = tileDef.Name;
        var itemData = ItemRegistry.Items.Values.FirstOrDefault(item => 
            item.Category == ItemCategory.Furniture && item.PlacedTileName == tileName);
        
        if (itemData != null)
        {
            // This is furniture, render the full sprite scaled down
            return RenderFurnitureIcon(context, tileDef);
        }
        
        // Non-furniture: use the old method (just grab 16x16 corner)
        var tex = context.MapManager.tileTextures[tileDef.ImagePath];

        Rectangle sourceRect = new(
            tileDef.ImageX,
            tileDef.ImageY,
            Math.Min(16, tileDef.ImageWidth),
            Math.Min(16, tileDef.ImageHeight)
        );

        // Create a new 16x16 texture and copy the relevant part
        Texture2D icon = new(context.GraphicsDevice, 16, 16);
        Color[] data = new Color[16 * 16];
        tex.GetData(0, sourceRect, data, 0, data.Length);
        icon.SetData(data);
        return icon;
    }

    private static Texture2D RenderFurnitureIcon(GameContext context, TileDefinition tileDef)
    {
        var tex = context.MapManager.tileTextures[tileDef.ImagePath];
        
        // Limit to 64x64 max from top-left corner
        int iconWidth = Math.Min(tileDef.ImageWidth, 64);
        int iconHeight = Math.Min(tileDef.ImageHeight, 64);
        
        // Get the source rectangle (max 64x64 from top-left)
        Rectangle sourceRect = new(
            tileDef.ImageX,
            tileDef.ImageY,
            iconWidth,
            iconHeight
        );

        // Read the source sprite data
        Color[] iconData = new Color[iconWidth * iconHeight];
        tex.GetData(0, sourceRect, iconData, 0, iconData.Length);

        // Create a texture at the actual size (up to 64x64) - no downscaling
        Texture2D icon = new(context.GraphicsDevice, iconWidth, iconHeight);
        icon.SetData(iconData);
        return icon;
    }
}



public class LampTile : TileInstance
{
    public LampTile()
    {
        IsInteractable = true;
        IsOn = true;
    }
    public override LightType LightType => LightType.Bulb;
    public override Vector2 LightOffset => new(10f, -14f);
    public override bool OnRightClick(GameContext context)
    {
        IsOn = !IsOn;
        SoundPlayer.Play("lightSwitch");
        context.MapManager.ToggleLightTileList(this);
        return true;
    }
    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }
}

public class InvisibleLight : TileInstance
{
    public InvisibleLight()
    {
        IsOn = true;
    }
    public override LightType LightType => LightType.Flame;
    public override Vector2 LightOffset => new(14f, 10f);
}

public class UselessFurnitureTile : TileInstance
{
    public UselessFurnitureTile() { IsInteractable = true; }
    public override bool OnLeftClick(GameContext context)
    {
        return PickUpItem(context);
    }
}

// Alternates between the base tile id and the one directly after it
public class AnimatedTile : TileInstance
{
    public override void Draw(SpriteBatch spriteBatch, Dictionary<string, Texture2D> tileTextures, GameContext context)
    {
        Vector2 position = new(Position.X * 16, Position.Y * 16);

        int cycle = (int)context.DayManager.CurrentTime % 4 / 2;
        var currentTileId = BaseTileId + cycle;
        var bgDef = TileDefinitions.All[currentTileId];

        TileRenderer.DrawTile(context, spriteBatch, tileTextures, position, bgDef);
    }
}

public class BibleStand : TileInstance
{
    private static readonly string[] Messages =
    {
        "The pages are well-worn from use.",
        "You feel a quiet sense of peace.",
        "Someone's bookmarked their favorite verse.",
        "You pause for a moment of reflection.",
        "You don't read long, but it feels grounding.",
        "The message is simple: be good, do good."
    };

    private static readonly Random _rng = new();

    public BibleStand()
    {
        IsInteractable = true;
    }

    public override bool OnRightClick(GameContext context)
    {
        string message = Messages[_rng.Next(Messages.Length)];
        context.UIManager.AnnouncementBox.Show(message, null, context);
        return true;
    }
}

public class Mailbox : TileInstance
{
    public Mailbox()
    {
        IsInteractable = true;
    }

    public override bool OnRightClick(GameContext context)
    {

        context.UIManager.AnnouncementBox.Show("You have no mail.", null, context);
        return true;
    }
}

public class Wall : TileInstance
{}

public class Dirt : TileInstance
{}

public class Floor : TileInstance
{}

public class StaticTile : TileInstance { }
