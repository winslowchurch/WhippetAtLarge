using System.Linq;

namespace Honeyspur;

public class MapManager
{
    public MapData CurrentMap { get; private set; }

    public bool[,] CollisionGrid { get; private set; }
    public List<TileInstance> LightTiles { get; private set; } = [];
    public TileInstance[,] BaseTiles { get; private set; }
    public TileInstance[,] DecorationTiles { get; private set; }
    public TileInstance[,] ObjectTiles { get; private set; }

    public readonly Dictionary<string, Texture2D> tileTextures = [];
    private readonly int tileSize = TileDefinitions.TileSize;

    public class MapState
    {
        public TileInstance[,] BaseTiles;
        public TileInstance[,] DecorationTiles;
        public TileInstance[,] ObjectTiles;
        public bool[,] CollisionGrid;
        public List<TileInstance> LightTiles = [];
    }
    private readonly Dictionary<string, MapState> mapCache = [];

    public void LoadContent(Microsoft.Xna.Framework.Content.ContentManager Content)
    {
        foreach (var def in TileDefinitions.All.Values)
            if (!tileTextures.ContainsKey(def.ImagePath))
                tileTextures[def.ImagePath] = TextureManager.GetTexture(def.ImagePath);
    }

    public void LoadMap(MapData map, GameContext context)
    {
        // Cache current map state before switching
        if (CurrentMap != null && !string.IsNullOrEmpty(CurrentMap.Name))
        {
            mapCache[CurrentMap.Name] = new MapState
            {
                BaseTiles = BaseTiles,
                DecorationTiles = DecorationTiles,
                ObjectTiles = ObjectTiles,
                CollisionGrid = CollisionGrid,
                LightTiles = [.. LightTiles]
            };
        }

        CurrentMap = map;

        // Find map in cache
        if (!string.IsNullOrEmpty(map.Name) && mapCache.TryGetValue(map.Name, out MapState cachedState))
        {
            BaseTiles = cachedState.BaseTiles;
            DecorationTiles = cachedState.DecorationTiles;
            ObjectTiles = cachedState.ObjectTiles;
            CollisionGrid = cachedState.CollisionGrid;
            LightTiles = [.. cachedState.LightTiles];
        }
        else
            throw new Exception($"Map '{map.Name}' was not found in the cache.");
    }

    // Toggles the light given tile in the list
    public void ToggleLightTileList(TileInstance lightTile)
    {
        var current = LightTiles.FirstOrDefault(tile =>
        tile.Position == lightTile.Position && tile.BaseTileId == lightTile.BaseTileId);
        if (current != null)
            current.IsOn = lightTile.IsOn;
    }

    public void RemoveLightTile(TileInstance tile)
    {
        if (tile.LightType != LightType.None)
        {
            LightTiles.RemoveAll(t =>
                t.Position == tile.Position && t.BaseTileId == tile.BaseTileId);
        }
    }

    public void AddLightTile(TileInstance tile)
    {
        if (tile.LightType != LightType.None && !LightTiles.Contains(tile))
        {
            LightTiles.Add(tile);
        }
    }

    private TileInstance[,] CreateLayerTileInstances(int[,] layer, int width, int height, string mapName, GameContext context)
    {
        var result = new TileInstance[width, height];
        CollisionGrid = new bool[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int id = layer[y, x];
                if (id == 0 || !TileDefinitions.All.TryGetValue(id, out var def))
                    continue;

                var pos = new Point(x, y);
                var instance = TileFactory.Create(def, pos, id);
                instance.MapName = mapName;
                result[x, y] = instance;

                if (def.Collides)
                {
                    // Get collider size in tiles (default to 1x1)
                    Point size = def.ColliderSize ?? new Point(1, 1);
                    Point offset = def.ColliderOffset ?? Point.Zero;

                    // Mark all tiles that this collider covers as blocked
                    for (int blockY = 0; blockY < size.Y; blockY++)
                    {
                        for (int blockX = 0; blockX < size.X; blockX++)
                        {
                            int gridX = x + offset.X + blockX;
                            int gridY = y + offset.Y + blockY;

                            if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height)
                            {
                                CollisionGrid[gridX, gridY] = true;
                            }
                        }
                    }
                }

                if (instance.LightType != LightType.None)
                    LightTiles.Add(instance);
            }
        }

        return result;
    }

    public static Rectangle? GetColliderRect(TileInstance tile)
    {
        var def = TileDefinitions.All[tile.BaseTileId];
        if (!def.Collides)
            return null;

        int tileSize = TileDefinitions.TileSize;

        // Collider sizes/offsets are stored in tiles. Convert to pixels here.
        Point sizeInTiles = def.ColliderSize ?? new Point(1, 1);
        Point offsetInTiles = def.ColliderOffset ?? Point.Zero;

        int pixelWidth = sizeInTiles.X * tileSize;
        int pixelHeight = sizeInTiles.Y * tileSize;

        // For multi-tile tall objects we treat the tile.Position as the bottom-left anchor
        // and extend the collider upward when calculating the rectangle's Y.
        int startYTile = tile.Position.Y - (sizeInTiles.Y - 1);

        return new Rectangle(
            tile.Position.X * tileSize + offsetInTiles.X * tileSize,
            startYTile * tileSize + offsetInTiles.Y * tileSize,
            pixelWidth,
            pixelHeight
        );
    }

    public int GetTileIdAt(Point tilePosition, int[,] layer)
    {
        if (tilePosition.X >= 0 && tilePosition.X < CurrentMap.Width &&
            tilePosition.Y >= 0 && tilePosition.Y < CurrentMap.Height)
        {
            return layer[tilePosition.Y, tilePosition.X];
        }
        return 0;
    }

    public bool IsInBounds(int x, int y)
    {
        return x >= 0 && y >= 0 &&
               x < ObjectTiles.GetLength(0) &&
               y < ObjectTiles.GetLength(1);
    }

    public bool IsWalkable(int x, int y)
    {
        // If out of bounds, it's walkable (to allow leaving the map)
        if (!IsInBounds(x, y))
            return true;
        return !CollisionGrid[x, y];
    }

    public void UpdateCollisionGrid(Point tilePos, TileDefinition tileDef, bool setValue)
    {
        if (!tileDef.Collides) return;

        Point size = tileDef.ColliderSize ?? new Point(1, 1);
        Point offset = tileDef.ColliderOffset ?? Point.Zero;

        // Adjust Y to extend upward instead of downward
        int startY = tilePos.Y - (size.Y - 1);

        // Set collision for all affected tiles
        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
            {
                int gridX = tilePos.X + offset.X + x;
                int gridY = startY + offset.Y + y;

                if (IsInBounds(gridX, gridY))
                {
                    CollisionGrid[gridX, gridY] = setValue;
                }
            }
        }
    }

    public void DrawAllSorted(SpriteBatch spriteBatch, GameContext context)
    {
        var drawables = new List<(float y, Action drawAction)>();
        int width = CurrentMap.Width;
        int height = CurrentMap.Height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var baseTile = BaseTiles[x, y];
                if (baseTile != null)
                    drawables.Add((-1, () => baseTile.Draw(spriteBatch, tileTextures, context)));

                var decoTile = DecorationTiles[x, y];
                if (decoTile != null)
                    drawables.Add((0, () => decoTile.Draw(spriteBatch, tileTextures, context)));

                var objTile = ObjectTiles[x, y];
                if (objTile is Bed bed) // special bed case bc its so janky
                {
                    drawables.Add(((bed.Position.Y + 1) * tileSize, () => bed.Draw(spriteBatch, tileTextures, context)));
                    drawables.Add(((bed.Position.Y - 1) * tileSize, () => bed.DrawTop(spriteBatch, tileTextures, context)));
                }
                else if (objTile is FlowerPlant flowerPlant)
                    drawables.Add(((flowerPlant.Position.Y + 0.5f) * tileSize, () => flowerPlant.Draw(spriteBatch, tileTextures, context)));
                
                else if (objTile != null)
                    drawables.Add(((objTile.Position.Y + 1) * tileSize, () => objTile.Draw(spriteBatch, tileTextures, context)));
            }
        }

        if (context.Player != null)
            drawables.Add((context.Player.Position.Y + context.Player.Height, () => context.Player.Draw(spriteBatch)));

        drawables.Sort((a, b) => a.y.CompareTo(b.y));

        foreach (var (_, draw) in drawables)
            draw();
    }

    public TileInstance GetBaseTileAt(Point tilePos)
    {
        if (IsInBounds(tilePos.X, tilePos.Y))
            return BaseTiles[tilePos.X, tilePos.Y];
        return null;
    }

    public TileInstance GetBaseTileAt(Point tilePos, string mapName)
    {
        if (mapCache.TryGetValue(mapName, out MapState mapState))
        {
            if (tilePos.X >= 0 && tilePos.Y >= 0 &&
                tilePos.X < mapState.BaseTiles.GetLength(0) &&
                tilePos.Y < mapState.BaseTiles.GetLength(1))
            {
                return mapState.BaseTiles[tilePos.X, tilePos.Y];
            }
        }
        return null;
    }

    // Less robust version of GetTileInstanceAt if we know the item there is one tile big
    // Mostly for surroundings dependent rendering (like fences)
    public TileInstance GetObjectTileAt(Point tilePos)
    {
        if (IsInBounds(tilePos.X, tilePos.Y))
            return ObjectTiles[tilePos.X, tilePos.Y];
        return null;
    }

    public TileInstance GetObjectTileAt(Point tilePos, string mapName)
    {
        if (mapCache.TryGetValue(mapName, out MapState mapState))
        {
            if (tilePos.X >= 0 && tilePos.Y >= 0 &&
                tilePos.X < mapState.ObjectTiles.GetLength(0) &&
                tilePos.Y < mapState.ObjectTiles.GetLength(1))
            {
                return mapState.ObjectTiles[tilePos.X, tilePos.Y];
            }
        }
        return null;
    }

    // Returns the first interactable tile
    public TileInstance GetInteractableTileAt(Point tilePos, TileInstance[,] tileLayer)
    {
        int clickX = tilePos.X * tileSize; // pixel-space top-left of clicked tile
        int clickY = tilePos.Y * tileSize;

        int maxColliderWidth = TileDefinitions.All.Values.Max(d => d.ColliderSize?.X ?? 1);
        int maxColliderHeight = TileDefinitions.All.Values.Max(d => d.ColliderSize?.Y ?? 1);

        TileInstance TryResolveFromLayer(TileInstance[,] layer)
        {
            if (layer == null) return null;
            int width = layer.GetLength(0);
            int height = layer.GetLength(1);

            // Fast path: direct tile at clicked position.
            if (tilePos.X >= 0 && tilePos.X < width && tilePos.Y >= 0 && tilePos.Y < height)
            {
                var direct = layer[tilePos.X, tilePos.Y];
                if (direct?.IsInteractable == true)
                    return direct;
            }

            // Backtrack possible anchors.
            // Anchors may be up to (maxColliderWidth-1) tiles LEFT and (maxColliderHeight-1) tiles BELOW the clicked tile.
            for (int dx = 0; dx <= maxColliderWidth - 1; dx++) // move left from click
            {
                for (int dy = 0; dy <= maxColliderHeight - 1; dy++) // move down from click (since anchor is below for tall objects)
                {
                    int anchorX = tilePos.X - dx;
                    int anchorY = tilePos.Y + dy;

                    if (anchorX < 0 || anchorY < 0 || anchorX >= width || anchorY >= height)
                        continue;

                    var candidate = layer[anchorX, anchorY];
                    if (candidate == null || !candidate.IsInteractable)
                        continue;

                    var collider = GetColliderRect(candidate);
                    if (!collider.HasValue)
                        continue;

                    if (collider.Value.Contains(clickX, clickY))
                        return candidate;
                }
            }

            return null;
        }

        return TryResolveFromLayer(tileLayer);
    }

    public void PrecacheAllMaps(IEnumerable<MapData> allMaps, GameContext context)
    {
        foreach (var map in allMaps)
        {
            if (string.IsNullOrEmpty(map.Name) || mapCache.ContainsKey(map.Name))
                continue;

            // Create tile layers for this map
            var baseTiles = CreateLayerTileInstances(map.BaseLayer, map.Width, map.Height, map.Name, context);
            var decoTiles = CreateLayerTileInstances(map.DecorationLayer, map.Width, map.Height, map.Name, context);
            var objTiles = CreateLayerTileInstances(map.ObjectLayer, map.Width, map.Height, map.Name, context);

            // Create collision grid and gather light tiles for this map
            var lightTiles = new List<TileInstance>();
            var collisionGrid = new bool[map.Width, map.Height];

            ProcessLayerForLights(baseTiles, lightTiles);
            ProcessLayerForLights(decoTiles, lightTiles);
            ProcessLayerForLights(objTiles, lightTiles);

            // Update collision grid
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    void CheckTileCollision(TileInstance tile)
                    {
                        if (tile?.BaseTileId > 0)
                        {
                            var def = TileDefinitions.All[tile.BaseTileId];
                            if (def.Collides)
                            {
                                Point size = def.ColliderSize ?? new Point(1, 1);
                                Point offset = def.ColliderOffset ?? Point.Zero;

                                // Adjust Y to extend upward instead of downward
                                int startY = y - (size.Y - 1);

                                for (int blockY = 0; blockY < size.Y; blockY++)
                                {
                                    for (int blockX = 0; blockX < size.X; blockX++)
                                    {
                                        int gridX = x + offset.X + blockX;
                                        int gridY = startY + offset.Y + blockY;

                                        if (gridX >= 0 && gridX < map.Width && gridY >= 0 && gridY < map.Height)
                                        {
                                            collisionGrid[gridX, gridY] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    CheckTileCollision(baseTiles[x, y]);
                    CheckTileCollision(decoTiles[x, y]);
                    CheckTileCollision(objTiles[x, y]);
                }
            }

            mapCache[map.Name] = new MapState
            {
                BaseTiles = baseTiles,
                DecorationTiles = decoTiles,
                ObjectTiles = objTiles,
                CollisionGrid = collisionGrid,
                LightTiles = lightTiles
            };
        }
    }

    private static void ProcessLayerForLights(TileInstance[,] layerTiles, List<TileInstance> lightTiles)
    {
        int width = layerTiles.GetLength(0);
        int height = layerTiles.GetLength(1);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var tile = layerTiles[x, y];
                if (tile != null && tile.LightType != LightType.None)
                {
                    lightTiles.Add(tile);
                }
            }
        }
    }

    public void DayUpdateAllTiles(GameContext context)
    {
        foreach (var state in mapCache.Values)
        {
            DayUpdateTilesInLayer(state.ObjectTiles, context);
            DayUpdateTilesInLayer(state.DecorationTiles, context);
            DayUpdateTilesInLayer(state.BaseTiles, context);
        }
    }

    private static void DayUpdateTilesInLayer(TileInstance[,] tiles, GameContext context)
    {
        if (tiles == null) return;
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y]?.DayUpdate(context);
            }
        }
    }
}