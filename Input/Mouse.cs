using System.Linq;

namespace Honeyspur;

public static class MouseHelper
{
    private const int TileSize = 16;

    public static void UpdateCursorHover(GameContext context, Point tilePos)
    {
        Point mouseScreenPos = Mouse.GetState().Position;
        var selectedItem = context.UIManager.GetSelectedInventoryItem();

        if (context.UIManager.IsHoveringOverUI(mouseScreenPos, context))
        {
            Game1.UseHandCursor = true;
            return;
        }

        if (!context.UIManager.IsModalOpen() && ShouldShowWorldHandCursor(context, tilePos, selectedItem))
        {
            Game1.UseHandCursor = true;
            return;
        }

        Game1.UseHandCursor = false;
    }

    private static bool ShouldShowWorldHandCursor(GameContext context, Point tilePos, Items.ItemInstance selectedItem)
    {
        if (!context.MapManager.IsInBounds(tilePos.X, tilePos.Y))
            return false;

        // Check if holding a placeable item and can place at cursor position
        if (selectedItem != null)
        {
            var (canPlace, _, _, _) = CanPlaceItem(context, tilePos, selectedItem);
            if (canPlace)
                return true;

            // Check if holding a tool that can be used here
            if (IsUsableToolAtPosition(selectedItem, context, tilePos))
                return true;
        }

        // Check world tiles for interactable objects
        return IsInteractableTileAtPosition(context, tilePos);
    }

    private static bool IsUsableToolAtPosition(Items.ItemInstance item, GameContext context, Point tilePos)
    {
        if (item.ToolType != Items.ToolType.Hoe && item.ToolType != Items.ToolType.WateringCan)
            return false;

        return context.Player.IsTileWithinRange(tilePos);
    }

    private static bool IsInteractableTileAtPosition(GameContext context, Point tilePos)
    {
        if (!context.Player.IsTileWithinRange(tilePos))
            return false;

        var objectTile = context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.ObjectTiles);
        var decorationTile = context.MapManager.GetInteractableTileAt(tilePos, context.MapManager.DecorationTiles);

        return objectTile != null || decorationTile != null;
    }

    private static Point GetEffectiveColliderSize(TileDefinition tileDef)
    {
        if (tileDef?.InstanceType == typeof(Bed))
            return new Point(3, 2);

        return tileDef?.ColliderSize ?? new Point(1, 1);
    }

    private static bool IsTileAvailableForPlacement(GameContext context, int gridX, int gridY, Type requiredBase, TileDefinition tileDef)
    {
        if (!context.MapManager.IsInBounds(gridX, gridY) || context.MapManager.CollisionGrid[gridX, gridY])
            return false;

        if (context.MapManager.ObjectTiles[gridX, gridY] is FlowerPlant)
            return false;

        if (tileDef.Collides && IsPlayerAtPosition(context, gridX, gridY))
            return false;

        return IsValidBaseTile(context, gridX, gridY, requiredBase);
    }

    private static bool IsPlayerAtPosition(GameContext context, int gridX, int gridY)
    {
        Rectangle playerRect = context.Player.GetCollisionRect();
        Rectangle cellRect = new(gridX * TileSize, gridY * TileSize, TileSize, TileSize);
        return playerRect.Intersects(cellRect);
    }

    private static bool IsValidBaseTile(GameContext context, int gridX, int gridY, Type requiredBase)
    {
        var baseTile = context.MapManager.GetBaseTileAt(new Point(gridX, gridY));
        
        if (requiredBase != null)
            return baseTile?.GetType() == requiredBase;

        return baseTile is Floor || baseTile is Grass;
    }

    private static bool RequiresTilledGrass(GameContext context, int gridX, int gridY, Items.ItemInstance item)
    {
        if (item.SeedType != Items.SeedType.FlowerPlant)
            return true;

        var baseTile = context.MapManager.GetBaseTileAt(new Point(gridX, gridY));
        return baseTile is not Grass grassTile || grassTile.IsTilled();
    }

    private static (Point offset, int startY, int tilesWide, int tilesHigh) GetPlacementArea(Point tilePos, TileDefinition tileDef)
    {
        Point colliderSize = GetEffectiveColliderSize(tileDef);
        Point offset = tileDef.ColliderOffset ?? Point.Zero;
        int tilesWide = colliderSize.X;
        int tilesHigh = colliderSize.Y;
        int startY = tilePos.Y - (tilesHigh - 1);

        return (offset, startY, tilesWide, tilesHigh);
    }

    public static (bool canPlace, TileDefinition tileDef, string placementTileName, Point colliderSize) CanPlaceItem(GameContext context, Point tilePos, Items.ItemInstance selectedItem)
    {
        if (selectedItem == null)
            return (false, null, null, Point.Zero);

        if (!PlacementMappings.TryGetPlacementInfo(selectedItem.Name, out string placementTileName, out Type requiredBase))
            return (false, null, null, Point.Zero);

        var tileDef = TileDefinitions.All.Values.FirstOrDefault(def => def.Name == placementTileName);
        if (tileDef == null)
            return (false, null, null, Point.Zero);

        Point colliderSize = GetEffectiveColliderSize(tileDef);
        var (offset, startY, tilesWide, tilesHigh) = GetPlacementArea(tilePos, tileDef);

        bool canPlace = CheckAllTilesAvailable(context, tilePos, selectedItem, tileDef, requiredBase, offset, startY, tilesWide, tilesHigh);

        return (canPlace, tileDef, placementTileName, colliderSize);
    }

    private static bool CheckAllTilesAvailable(GameContext context, Point tilePos, Items.ItemInstance selectedItem, 
        TileDefinition tileDef, Type requiredBase, Point offset, int startY, int tilesWide, int tilesHigh)
    {
        for (int x = 0; x < tilesWide; x++)
        {
            for (int y = 0; y < tilesHigh; y++)
            {
                int gridX = tilePos.X + offset.X + x;
                int gridY = startY + offset.Y + y;

                if (!IsTileAvailableForPlacement(context, gridX, gridY, requiredBase, tileDef))
                    return false;

                if (!RequiresTilledGrass(context, gridX, gridY, selectedItem))
                    return false;
            }
        }

        return true;
    }

    public static void DrawPlacementPreview(GameContext context, SpriteBatch spriteBatch)
    {
        Point mouseScreenPos = Mouse.GetState().Position;
        if (context.UIManager.IsHoveringOverUI(mouseScreenPos, context))
            return;

        Point tilePos = GetMouseTilePosition(context);
        var selectedItem = context.UIManager.GetSelectedInventoryItem();

        if (selectedItem == null)
            return;

        if (!PlacementMappings.TryGetPlacementInfo(selectedItem.Name, out _, out Type requiredBase))
            return;

        var (_, tileDef, _, _) = CanPlaceItem(context, tilePos, selectedItem);
        if (tileDef == null)
            return;

        var (offset, startY, tilesWide, tilesHigh) = GetPlacementArea(tilePos, tileDef);
        var greenBox = TileDefinitions.All[1];
        var redBox = TileDefinitions.All[2];

        for (int x = 0; x < tilesWide; x++)
        {
            for (int y = 0; y < tilesHigh; y++)
            {
                int gridX = tilePos.X + offset.X + x;
                int gridY = startY + offset.Y + y;
                Vector2 drawPos = new(gridX * TileSize, gridY * TileSize);

                bool tileAvailable = IsTileAvailableForPlacement(context, gridX, gridY, requiredBase, tileDef) 
                                     && RequiresTilledGrass(context, gridX, gridY, selectedItem);

                TileRenderer.DrawTile(context, spriteBatch, context.MapManager.tileTextures, drawPos,
                    tileAvailable ? greenBox : redBox);
            }
        }
    }

    public static Point GetMouseTilePosition(GameContext context)
    {
        MouseState mouse = Mouse.GetState();
        Point mousePos = new(mouse.X, mouse.Y);
        Vector2 worldPosition = Vector2.Transform(mousePos.ToVector2(), Matrix.Invert(context.Camera.GetViewMatrix()));
        return new Point(
            (int)Math.Floor(worldPosition.X / TileSize),
            (int)Math.Floor(worldPosition.Y / TileSize)
        );
    }

    public static Rectangle GetPlacementRect(Point tilePos, Point colliderSize)
    {
        return new Rectangle(
            tilePos.X * TileSize,
            tilePos.Y * TileSize,
            colliderSize.X,
            colliderSize.Y
        );
    }
}