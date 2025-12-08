namespace Honeyspur;

public static class Maps
{
    public static MapData House => HouseMapData.House;
    public static MapData HomeBase => HomeBaseMapData.HomeBase;
    public static MapData BeechForest => BeechForestMapData.BeechForest;
    public static MapData Church => ChurchMapData.Church;
    public static MapData Town => TownMapData.Town;
    public static MapData DotHouse => DotHouseMapData.DotHouse;

    public static IEnumerable<MapData> All => [House, HomeBase, BeechForest, Church, Town, DotHouse];
}