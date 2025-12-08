namespace Honeyspur;

public class MapData
{
    public int[,] BaseLayer; // Ground and Walls
    public int[,] DecorationLayer; // Rugs and Wall Decor (and ground layering)
    public int[,] ObjectLayer; // Furniture 
    public int Width => BaseLayer.GetLength(1);
    public int Height => BaseLayer.GetLength(0);
    public string Name;
    public MapType Type { get; set; } = MapType.Outside;
    public List<MapExit> Exits { get; set; } = [];
}

public enum MapType
{
    Outside,
    Inside
}

public class MapExit
{
    public Rectangle Area; // The region that triggers the exit
    public MapData DestinationMap;
    public Vector2 DestinationSpawn;
}