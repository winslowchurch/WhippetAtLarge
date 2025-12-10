namespace Honeyspur;

public static class Maps
{
    public static MapData House => HouseMapData.House;

    public static IEnumerable<MapData> All => [House];
}