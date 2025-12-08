namespace Honeyspur;

public class TileDefinition
{
    public string Name { get; set; }
    public string ImagePath { get; set; }
    public bool Collides { get; set; } = false;
    public Point? ColliderSize { get; set; }
    public Point? ColliderOffset { get; set; }
    public Point? ImageOffset { get; set; }
    public int ImageX { get; set; } = 0;
    public int ImageY { get; set; } = 0;
    public int ImageWidth { get; set; } = TileDefinitions.TileSize;
    public int ImageHeight { get; set; } = TileDefinitions.TileSize;
    public Type InstanceType { get; set; }
    public object InstanceParam { get; set; }
}

public static class TileDefinitions
{
    // Reusable image path constants to avoid repeating string literals
    private const string WindowsPath = "graphics/walls/windows";
    private const string DecorationDecorPath = "graphics/decoration/decor";
    private const string RugsPath = "graphics/decoration/rugs";
    private const string TownDecorPath = "graphics/decoration/townDecor";
    public const string TerrainPath = "graphics/nature/terrain";
    public const string WinterTerrainPath = "graphics/nature/terrainWinter";
    private const string UIPath = "graphics/ui";
    private const string WallsPath = "graphics/walls/walls";
    private const string NatureFlowerTreesPath = "graphics/nature/flowerTrees";
    private const string FlowerPlantPath = "graphics/nature/plants";
    private const string NatureTreesPath = "graphics/nature/trees";
    private const string NatureFlowerBushesPath = "graphics/nature/flowerBushes";
    private const string NatureSkyPath = "graphics/nature/sky";
    public const string OutsideDecorPath = "graphics/decoration/outsideDecor";
    public const string WinterOutsideDecorPath = "graphics/decoration/outsideDecorWinter";
    public const string BuildingsPath = "graphics/decoration/buildings";
    public const string WinterBuildingsPath = "graphics/decoration/buildingsWinter";
    public const string CityBuildingsPath = "graphics/decoration/cityBuildings";
    public const string CityWinterBuildingsPath = "graphics/decoration/cityBuildingsWinter";
    private const string WallsChurchWallPath = "graphics/walls/churchWall";

    public static readonly int TileSize = 16;
    public static readonly Dictionary<int, TileDefinition> All = new()
    {
        { 1, new TileDefinition { Name="Green Placement", ImagePath = UIPath, ImageX=128 } },
        { 2, new TileDefinition { Name="Red Placement", ImagePath = UIPath, ImageX=144 } },
        { 3, new TileDefinition { Name="Emtpy Collider", ImagePath = BuildingsPath, Collides=true} },
        { 4, new TileDefinition { Name="Invisible Light", ImagePath = BuildingsPath, InstanceType = typeof(InvisibleLight) } },

        // --- Room Borders ---
        { 5, new TileDefinition { Name="Left Right Bottom Border", ImagePath = WallsPath, ImageX=16,ImageY=80, Collides = true } },
        { 6, new TileDefinition { Name="Left Right Top Border", ImagePath = WallsPath, ImageX=32, ImageY = 80, Collides = true } },
        { 7, new TileDefinition { Name="Top Border", ImagePath = WallsPath, Collides=true, ImageX=16, ImageY=64, ImageOffset = new Point(0, -1) } },
        { 8, new TileDefinition { Name="Top Right Border", ImagePath = WallsPath, ImageX=128, ImageY=64, Collides = true } },
        { 9, new TileDefinition { Name="Right Border", ImagePath = WallsPath, ImageX=160, ImageY=64, Collides = true } },
        { 10, new TileDefinition { Name="Bottom Right Angled Border", ImagePath = WallsPath, ImageX=128, ImageY=80,ImageWidth=32,ImageHeight=32,Collides = true, ImageOffset=new Point (-11,2) } },
        { 11, new TileDefinition { Name="Bottom Border", ImagePath = WallsPath, ImageX=64, ImageY=64, Collides = true, ImageOffset=new Point (0,6) } },
        { 12, new TileDefinition { Name="Bottom Left Angled Border", ImagePath = WallsPath, ImageX=160, ImageY=80,Collides = true, ImageWidth=32,ImageHeight=32,ImageOffset=new Point (11,2) } },
        { 13, new TileDefinition { Name="Left Border", ImagePath = WallsPath, ImageX=176, ImageY=64,Collides = true } },
        { 14, new TileDefinition { Name="Top Left Border", ImagePath = WallsPath, ImageX=144, ImageY=64, Collides = true } },
        { 15, new TileDefinition { Name="Inner Bottom Left Border", ImagePath = WallsPath, ImageX=80, ImageY=64, Collides = true, ImageOffset=new Point (0,6) } },
        { 16, new TileDefinition { Name="Inner Bottom Right Border", ImagePath = WallsPath, ImageX=48, ImageY=64, Collides = true, ImageHeight=32,ImageOffset=new Point (0,-10) } },
        { 17, new TileDefinition { Name="Bottom Left Corner Border", ImagePath = WallsPath, ImageX=96, ImageY=64,ImageOffset=new Point (0,6) } },
        { 18, new TileDefinition { Name="Bottom Right Corner Border", ImagePath = WallsPath, ImageX=112, ImageY=64,ImageOffset=new Point (0,6) } },
        { 19, new TileDefinition { Name="Left Right Double Border", ImagePath = WallsPath, ImageY=80, Collides = true } },

        // GRASS
        { 20, new TileDefinition { Name="Light Grass Corner Bottom Right",ImagePath = TerrainPath, ImageY=96, InstanceType = typeof(Grass)} },
        { 21, new TileDefinition { Name="Light Grass Corner Bottom Left",ImagePath = TerrainPath, ImageX=16,ImageY=96,InstanceType = typeof(Grass)} },
        { 22, new TileDefinition { Name="Light Grass Corner Top Right",ImagePath = TerrainPath, ImageX=32,ImageY=96,InstanceType = typeof(Grass)} },
        { 23, new TileDefinition { Name="Light Grass Corner Top Left",ImagePath = TerrainPath, ImageX=48,ImageY=96,InstanceType = typeof(Grass)} },
        { 24, new TileDefinition { Name="Light Grass Top Left",ImagePath = TerrainPath, ImageY=48,InstanceType = typeof(Grass)} },
        { 25, new TileDefinition { Name="Light Grass Top",ImagePath = TerrainPath, ImageX=16,ImageY=48,InstanceType = typeof(Grass) }},
        { 26, new TileDefinition { Name="Light Grass Top Right",ImagePath = TerrainPath, ImageX=32,ImageY=48,InstanceType = typeof(Grass) }},
        { 27, new TileDefinition { Name="Light Grass Left",ImagePath = TerrainPath, ImageY=64, InstanceType = typeof(Grass) }},
        { 28, new TileDefinition { Name="Light Grass",ImagePath = TerrainPath, ImageX=16,ImageY=64, InstanceType = typeof(Grass)} },
        { 29, new TileDefinition { Name="Light Grass Right",ImagePath = TerrainPath, ImageX=32,ImageY=64, InstanceType = typeof(Grass)} },
        { 30, new TileDefinition { Name="Light Grass Bottom Left",ImagePath = TerrainPath, ImageY=80, InstanceType = typeof(Grass) }},
        { 31, new TileDefinition { Name="Light Grass Bottom",ImagePath = TerrainPath, ImageX=16,ImageY=80, InstanceType = typeof(Grass) }},
        { 32, new TileDefinition { Name="Light Grass Bottom Right",ImagePath = TerrainPath, ImageX=32,ImageY=80, InstanceType = typeof(Grass) }},

        { 33, new TileDefinition { Name="Watered Shadow", ImagePath = TerrainPath,ImageY=16} },
        { 34, new TileDefinition { Name="Grass1",ImagePath = TerrainPath,ImageY=32, InstanceType = typeof(Grass)  } },
        { 35, new TileDefinition { Name="Grass2",ImagePath = TerrainPath, ImageX = 16,ImageY=32, InstanceType = typeof(Grass)  } },
        { 36, new TileDefinition { Name="Grass Flower 1",ImagePath = TerrainPath, ImageX = 32,ImageY=32, InstanceType = typeof(Grass) } },
        { 37, new TileDefinition { Name="Grass Flower 2",ImagePath = TerrainPath, ImageX = 48, ImageY=32, InstanceType = typeof(Grass) } },
        { 38, new TileDefinition { Name="Tilled Soil", ImagePath = TerrainPath,ImageX=64,ImageY=16} },
        { 39, new TileDefinition { Name="Wet Fertilizer Specs", ImagePath = TerrainPath,ImageX=16,ImageY=16} },
        { 40, new TileDefinition { Name="Grass Path",ImagePath = TerrainPath, ImageX = 64,ImageY=32, InstanceType = typeof(Grass) } },
        { 41, new TileDefinition { Name="Grass Wall Left",ImagePath = TerrainPath, ImageX=176,ImageY=32,Collides = true, InstanceType = typeof(Grass) } },
        { 42, new TileDefinition { Name="Grass Cliff Top",ImagePath = TerrainPath, ImageX=112,ImageY=32,Collides = true, ColliderOffset=new Point(0,-1), InstanceType = typeof(Grass) } },
        // add funky water to cliff thing
        { 44, new TileDefinition { Name="Grass Wall Corner Right", ImagePath = TerrainPath, Collides = true,ImageX=128,ImageY=32,InstanceType = typeof(Grass) } },
        { 45, new TileDefinition { Name="Grass Wall Right", ImagePath = TerrainPath, ImageX=208,ImageY=32,Collides = true, InstanceType = typeof(Grass) } },
        { 46, new TileDefinition { Name="Grass Wall Top", ImagePath = TerrainPath, ImageX=96,ImageY=32,Collides = true, InstanceType = typeof(Grass) } },
        { 47, new TileDefinition { Name="Grass Wall Corner Left", ImagePath = TerrainPath, Collides = true,ImageX=80,ImageY=32, InstanceType = typeof(Grass) } },
        { 48, new TileDefinition { Name="Flower Field 1", ImagePath = TerrainPath,ImageY=16,ImageX=80, InstanceType = typeof(Grass) } },
        { 49, new TileDefinition { Name="Flower Field 2", ImagePath = TerrainPath,ImageY=16,ImageX=96, InstanceType = typeof(Grass) } },
        { 50, new TileDefinition { Name="Flower Field Bottom", ImagePath = TerrainPath,ImageY=16,ImageX=112, InstanceType = typeof(Grass) } },
        { 51, new TileDefinition { Name="Flower Field Top", ImagePath = TerrainPath,ImageY=16,ImageX=128,InstanceType = typeof(Grass) } },
        { 52, new TileDefinition { Name="Flower Field Right", ImagePath = TerrainPath,ImageY=16,ImageX=144, InstanceType = typeof(Grass)} },
        { 53, new TileDefinition { Name="Flower Field Left", ImagePath = TerrainPath,ImageY=16,ImageX=160, InstanceType = typeof(Grass) } },
        { 54, new TileDefinition { Name="Growth Fertilizer Specs", ImagePath = TerrainPath,ImageX=48,ImageY=16} },
        { 55, new TileDefinition { Name="Bloom Fertilizer Specs", ImagePath = TerrainPath,ImageX=32,ImageY=16} },
        { 56, new TileDefinition { Name="Growth Fertilizer Specs", ImagePath = TerrainPath,ImageX=48,ImageY=16} },
        { 57, new TileDefinition { Name="Grass Wall Bottom Left", ImagePath = TerrainPath, Collides = true,ImageY=48,ImageX=176, ImageHeight=32, ColliderSize=new Point(1,2), InstanceType = typeof(Grass) } },
        { 58, new TileDefinition { Name="Grass Wall Bottom", ImagePath = TerrainPath, Collides = true,ImageY=64,ImageX=192,InstanceType = typeof(Grass) } },
        { 59, new TileDefinition { Name="Grass Wall Bottom Right", ImagePath = TerrainPath, Collides = true,ImageY=48,ImageX=208, ImageHeight=32, ColliderSize=new Point(1,2),InstanceType = typeof(Grass) } },

        // TREES and BUSHES (60-80)
        { 60, new TileDefinition { Name="Brugmansia Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageY=320,ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Brugmansia" } },
        { 61, new TileDefinition { Name="Cherry Blossom Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Cherry Blossom" } },
        { 62, new TileDefinition { Name="Dead Tree", ImagePath = NatureTreesPath, Collides = true, ImageWidth=48,ImageHeight=80,ImageY=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(TreeTile) } },
        { 63, new TileDefinition { Name="Lemon Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageY=160,ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Lemon" } },
        { 64, new TileDefinition { Name="Pear Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageY=80,ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Pear" } },
        { 65, new TileDefinition { Name="Beech Tree", ImagePath = NatureTreesPath, Collides = true, ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(TreeTile) } },
        { 66, new TileDefinition { Name="Lime Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageY=240,ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Lime" } },
        { 67, new TileDefinition { Name="Wisteria Tree", ImagePath = NatureFlowerTreesPath, Collides = true, ImageY=400,ImageHeight=80,ImageOffset = new Point(-16, 0), InstanceType = typeof(FlowerTree), InstanceParam = "Wisteria" } },

        { 68, new TileDefinition { Name="Daffodil Plant", ImagePath = FlowerPlantPath, ImageHeight=32, InstanceType = typeof(FlowerPlant), InstanceParam = "Daffodil" } },
        { 69, new TileDefinition { Name="Dahlia Plant", ImagePath = FlowerPlantPath, ImageY=32,ImageHeight=32, InstanceType = typeof(FlowerPlant), InstanceParam = "Dahlia" } },
        { 70, new TileDefinition { Name="Orchid Plant", ImagePath = FlowerPlantPath, ImageY=64,ImageHeight=32, InstanceType = typeof(FlowerPlant), InstanceParam = "Orchid" } },
        { 71, new TileDefinition { Name="Lavander Plant", ImagePath = FlowerPlantPath, ImageY=96,ImageHeight=32, InstanceType = typeof(FlowerPlant), InstanceParam = "Lavender" } },

        { 72, new TileDefinition { Name="Orange Hibiscus Bush", ImagePath = NatureFlowerBushesPath, ImageY=336,ImageHeight=48,Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Orange Hibiscus" } },
        { 73, new TileDefinition { Name="White Lillies Bush", ImagePath = NatureFlowerBushesPath, ImageY=144, ImageHeight=48, Collides=true, ColliderSize=new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "White Lily" } },
        { 74, new TileDefinition { Name="Pink Lillies Bush", ImagePath = NatureFlowerBushesPath, ImageY=96, ImageHeight=48, Collides=true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Pink Lily" } },
        { 75, new TileDefinition { Name="Moonflower Bush", ImagePath = NatureFlowerBushesPath, ImageY=192, ImageHeight=48, Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Moonflower" } },
        { 76, new TileDefinition { Name="Red Roses Bush", ImagePath = NatureFlowerBushesPath, ImageY=384, ImageHeight=48, Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Red Rose" } },
        { 77, new TileDefinition { Name="Yellow Roses Bush", ImagePath = NatureFlowerBushesPath, ImageY=432,ImageHeight=48, Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Yellow Rose" } },
        { 78, new TileDefinition { Name="Purple Roses Bush", ImagePath = NatureFlowerBushesPath, ImageY=480,ImageHeight=48, Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Purple Rose" } },
        { 79, new TileDefinition { Name="Blue Hydrengas Bush", ImagePath = NatureFlowerBushesPath,Collides = true, ImageHeight=48,ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Blue Hydrenga" } },
        { 80, new TileDefinition { Name="Pink Hydrengas Bush", ImagePath = NatureFlowerBushesPath, ImageY=48,ImageHeight=48,Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(FlowerBush), InstanceParam = "Pink Hydrenga" } },
        // 81

        // WATER 
        { 82, new TileDefinition { Name="Lily Pad", ImagePath = TerrainPath, Collides = true,ImageX=112,ImageY=64 } },
        { 83, new TileDefinition { Name="Water Edge Right", ImagePath = TerrainPath, ImageX=128, ImageY=64,Collides = true, } },
        { 84, new TileDefinition { Name="Water Edge Left", ImagePath = TerrainPath, ImageX=96,ImageY=64,Collides = true, } },
        { 85, new TileDefinition { Name="Water",ImagePath = TerrainPath, InstanceType = typeof(AnimatedTile) } },
        { 86, new TileDefinition { Name="Water2",ImagePath = TerrainPath, ImageX = 16 } },
        { 87, new TileDefinition { Name="Waterfall Top", ImagePath = TerrainPath, ImageX = 32, InstanceType = typeof(AnimatedTile) } },
        { 88, new TileDefinition { Name="Waterfall Top2", ImagePath = TerrainPath, ImageX = 48 } },
        { 89, new TileDefinition { Name="Water Edge Top Left", ImagePath = TerrainPath, ImageX=96,ImageY=48,Collides = true, } },
        { 90, new TileDefinition { Name="Water Edge Top Middle", ImagePath = TerrainPath, Collides = true,ImageX=112,ImageY=48} },
        { 91, new TileDefinition { Name="Water Edge Top Right", ImagePath = TerrainPath, ImageX=128,ImageY=48,Collides = true, } },
        { 92, new TileDefinition { Name="Water Edge Bottom Left", ImagePath = TerrainPath, Collides = true, ImageX=96, ImageY=80 } },
        { 93, new TileDefinition { Name="Water Edge Bottom", ImagePath = TerrainPath, Collides = true, ImageX=112,ImageY=80 } },
        { 94, new TileDefinition { Name="Water Edge Bottom Right", ImagePath = TerrainPath, ImageX=128,ImageY=80,Collides = true} },
        { 95, new TileDefinition { Name="Water Corner Bottom Left", ImagePath = TerrainPath, ImageX=144, ImageY=64,Collides = true } },
        { 96, new TileDefinition { Name="Water Corner Bottom Right", ImagePath = TerrainPath, ImageX=160, ImageY=64,Collides = true } },
        { 97, new TileDefinition { Name="Water Corner Top Left", ImagePath = TerrainPath, ImageX=144, ImageY=48,Collides = true } },
        { 98, new TileDefinition { Name="Water Corner Top Right", ImagePath = TerrainPath, ImageX=160,ImageY=48,Collides = true } },
        // leave 99 - 106 for more water cuz ill prob need them

        // --- Windows --- (106-129)
        { 107, new TileDefinition { Name="Small Window", ImagePath = WindowsPath, ImageHeight = 32, Collides=true, ImageOffset = new Point(0, -5), InstanceType = typeof(SmallWindowTile)} },
        { 108, new TileDefinition { Name="Small Curtain Window", ImagePath = WindowsPath, ImageX=16, ImageHeight = 32, Collides=true,ImageOffset = new Point(0, -5), InstanceType = typeof(SmallWindowTile) } },
        { 109, new TileDefinition { Name="Small Drawn Window", ImagePath = WindowsPath, ImageX=32, ImageHeight = 32, Collides=true,ImageOffset = new Point(0, -5), InstanceType = typeof(SmallWindowTile) } },
        { 110, new TileDefinition { Name="Medium Window", ImagePath = WindowsPath, ImageY = 32, ImageWidth = 32, ImageHeight = 32, Collides=true,  ColliderSize = new Point(2, 1),ImageOffset = new Point(0, -5), InstanceType = typeof(MediumWindowTile) } },
        { 111, new TileDefinition { Name="Medium Curtain Window", ImagePath = WindowsPath, ImageX=32, ImageY = 32, ImageWidth = 32, ImageHeight = 32, Collides=true, ColliderSize = new Point(2, 1),ImageOffset = new Point(0, -5), InstanceType = typeof(MediumWindowTile) } },
        { 112, new TileDefinition { Name="Medium Drawn Window", ImagePath = WindowsPath, ImageX=32, ImageY = 32, ImageWidth = 32, ImageHeight = 32, Collides=true, ColliderSize = new Point(2, 1),ImageOffset = new Point(0, -5), InstanceType = typeof(MediumWindowTile) } },
        { 113, new TileDefinition { Name="Large Window", ImagePath = WindowsPath,  ImageY=64, ImageWidth = 48, ImageHeight = 48, Collides=true, ColliderSize = new Point(3, 1),ImageOffset = new Point(0, -16), InstanceType = typeof(LargeWindowTile) } },
        { 114, new TileDefinition { Name="Large Curtain Window", ImagePath = WindowsPath, ImageX=48,ImageY=64, ImageWidth = 48,ImageHeight = 48, Collides=true, ColliderSize = new Point(3, 1),ImageOffset = new Point(0, -16), InstanceType = typeof(LargeWindowTile) } },
        { 115, new TileDefinition { Name="Large Drawn Window", ImagePath = WindowsPath, ImageY=64,ImageX=96, ImageWidth = 48,ImageHeight = 48, Collides=true, ColliderSize = new Point(3, 1),ImageOffset = new Point(0, -16), InstanceType = typeof(LargeWindowTile) } },
        { 116, new TileDefinition { Name="Small Window View", ImagePath = WindowsPath, ImageX=48, ImageHeight = 32,ImageOffset = new Point(0, -5) } },
        { 117, new TileDefinition { Name="Medium Window View", ImagePath = WindowsPath, ImageX = 96, ImageY = 32, ImageWidth = 32, ImageHeight = 32, ImageOffset = new Point(0, -5) } },
        { 118, new TileDefinition { Name="Large Window View", ImagePath = WindowsPath, ImageX = 144, ImageY = 64, ImageWidth = 48, ImageHeight = 48, ImageOffset = new Point(0, -16) } },
        { 119, new TileDefinition { Name="Green Stained Glass Window", ImagePath = WindowsPath, ImageX=144,ImageHeight = 32,ImageWidth=32,ImageOffset = new Point(0, -5), InstanceType = typeof(UselessFurnitureTile) } },
        
        // SKY BACKGROUND (120-139) gonna add clouds and stuff later
        { 131, new TileDefinition { Name="Mountains Background", ImagePath = NatureSkyPath,ImageWidth=272,ImageHeight=80, InstanceType = typeof(Sky) } },
        { 132, new TileDefinition { Name="Day Background 1", ImageY=80,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 133, new TileDefinition { Name="Night Background 1", ImageY=160,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 134, new TileDefinition { Name="Day Background 2", ImageY=240,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 135, new TileDefinition { Name="Night Background 2", ImageY=320,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 136, new TileDefinition { Name="Day Background 3", ImageY=400,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 137, new TileDefinition { Name="Night Background 3", ImageY=480,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 138, new TileDefinition { Name="Night Background 4", ImageY=560,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },
        { 139, new TileDefinition { Name="Night Background 4", ImageY=640,ImagePath=NatureSkyPath,ImageWidth=448,ImageHeight=80 } },

        // DIRT GROUND (140-154)
        { 140, new TileDefinition { Name="Dirt Path Top Left", ImagePath = TerrainPath, ImageX=176, InstanceType = typeof(Dirt)} },
        { 141, new TileDefinition { Name="Dirt Path Vertical",ImagePath = TerrainPath, ImageX=192, InstanceType = typeof(Dirt)} },
        { 142, new TileDefinition { Name="Dirt Path Top Right",ImagePath = TerrainPath, ImageX=208, InstanceType = typeof(Dirt) } },
        { 143, new TileDefinition { Name="Dirt Path bottom Left",ImagePath = TerrainPath, ImageX=176,ImageY=16, InstanceType = typeof(Dirt)} },
        { 144, new TileDefinition { Name="Dirt Path Horizontal",ImagePath = TerrainPath, ImageX=192,ImageY=16, InstanceType = typeof(Dirt)} },
        { 145, new TileDefinition { Name="Dirt Path Bottom Right",ImagePath = TerrainPath, ImageX=208,ImageY=16, InstanceType = typeof(Dirt)} },
        { 146, new TileDefinition { Name="Dirt",ImagePath = TerrainPath, ImageX=64,ImageY=64, InstanceType = typeof(Dirt)} },
        { 147, new TileDefinition { Name="Dirt Top",ImagePath = TerrainPath, ImageX=64,ImageY=48, InstanceType = typeof(Dirt)} },
        { 148, new TileDefinition { Name="Dirt Right",ImagePath = TerrainPath, ImageX=80,ImageY=64, InstanceType = typeof(Dirt)} },
        { 149, new TileDefinition { Name="Dirt Bottom",ImagePath = TerrainPath, ImageX=64,ImageY=80, InstanceType = typeof(Dirt)} },
        { 150, new TileDefinition { Name="Dirt Left",ImagePath = TerrainPath, ImageX=48,ImageY=64, InstanceType = typeof(Dirt)} },
        { 151, new TileDefinition { Name="Dirt Top Left",ImagePath = TerrainPath, ImageX=48,ImageY=48, InstanceType = typeof(Dirt)} },
        { 152, new TileDefinition { Name="Dirt Top Right",ImagePath = TerrainPath, ImageX=80,ImageY=48, InstanceType = typeof(Dirt)} },
        { 153, new TileDefinition { Name="Dirt Bottom Left",ImagePath = TerrainPath, ImageX=48,ImageY=80, InstanceType = typeof(Dirt)} },
        { 154, new TileDefinition { Name="Dirt Bottom Right",ImagePath = TerrainPath, ImageX=80,ImageY=80, InstanceType = typeof(Dirt)} },

        // BUILDINGS (160-199)
        { 160, new TileDefinition { Name="Marigold Hotel", ImagePath = CityBuildingsPath, ImageWidth=224,ImageHeight=240,Collides = true, ColliderSize = new Point(14, 5) } },
        { 161, new TileDefinition { Name="Le Bakery", ImagePath = CityBuildingsPath, ImageX=224,ImageY=48,ImageWidth=96,ImageHeight=192,Collides = true, ColliderSize = new Point(6, 4) } },
        { 162, new TileDefinition { Name="Flower Shop", ImagePath = BuildingsPath, ImageX=416,ImageY=112,ImageWidth=112,ImageHeight=160,Collides = true, ColliderSize = new Point(7, 4) } },
        { 163, new TileDefinition { Name="Teapot Cafe", ImagePath = BuildingsPath, ImageX=272,ImageY=160,ImageWidth=128,ImageHeight=112,Collides = true, ColliderSize = new Point(13, 2),ColliderOffset= new Point(1, 0) } },
        { 164, new TileDefinition { Name="Mansion", ImagePath = BuildingsPath, ImageY=384,ImageWidth=448,ImageHeight=320,Collides = true, ColliderSize = new Point(28, 4),ImageOffset= new Point(0, -32) } },
        { 165, new TileDefinition { Name="Kitt's Cabin", ImagePath = BuildingsPath, ImageX=198,ImageWidth=160,ImageHeight=144,Collides = true, ColliderSize = new Point(10, 3) } },
        { 166, new TileDefinition { Name="Townhouse", ImagePath = BuildingsPath, ImageWidth=104,ImageHeight=208,Collides = true, ColliderSize = new Point(7, 4),ColliderOffset= new Point(1, 0) } },
        { 167, new TileDefinition { Name="Post Office", ImagePath = BuildingsPath, ImageWidth=96,ImageHeight=144,Collides = true, ColliderSize = new Point(6, 4) } },
        { 168, new TileDefinition { Name="Dot's House", ImagePath = BuildingsPath, ImageX=256,ImageY=272,ImageWidth=112,ImageHeight=112,Collides = true, ColliderSize = new Point(7, 4) } },
        { 169, new TileDefinition { Name="Sadie's House", ImagePath = BuildingsPath, ImageX=368,ImageY=272,ImageWidth=128,ImageHeight=112,Collides = true, ColliderSize = new Point(8, 4) } },
        { 170, new TileDefinition { Name="Jackie's House", ImagePath = BuildingsPath, ImageX=128,ImageY=256,ImageWidth=128,ImageHeight=128,Collides = true, ColliderSize = new Point(8, 4) } },
        { 171, new TileDefinition { Name="Inn", ImagePath = BuildingsPath, ImageX=528,ImageY=96,ImageWidth=224,ImageHeight=192,Collides = true, ColliderSize = new Point(14, 5) } },
        { 172, new TileDefinition { Name="Inn Fence", ImagePath = BuildingsPath, ImageX=528,ImageY=288,ImageWidth=224,ImageHeight=80,Collides = true, ColliderSize = new Point(6, 1) } },
        { 173, new TileDefinition { Name="Emporium", ImagePath = BuildingsPath, ImageX=448,ImageY=384,ImageWidth=112,ImageHeight=160,Collides = true, ColliderSize = new Point(7, 3), ImageOffset=new Point(0,-16) } },
        { 174, new TileDefinition { Name="Lil Cabin", ImagePath = BuildingsPath, ImageX=128,ImageY=144,Collides = true, ImageWidth=80, ImageHeight=96,ColliderSize = new Point(5, 2) } },
        { 175, new TileDefinition { Name="Church", ImagePath = BuildingsPath, ImageY=160,ImageWidth=128,ImageHeight=224,Collides = true, ColliderSize = new Point(8, 3) } },
        { 176, new TileDefinition { Name="General Store", ImagePath = BuildingsPath, ImageX=256,ImageWidth=112,ImageHeight=144,Collides = true, ColliderSize = new Point(7, 4) } },
        { 177, new TileDefinition { Name="Trolley Car", ImagePath = BuildingsPath, ImageX=480,ImageY=16,ImageWidth=64,ImageHeight=80, Collides = true, ColliderSize = new Point(4, 2) } },
        { 178, new TileDefinition { Name="Glenn's House", ImagePath = BuildingsPath, ImageX=576,ImageY=368,ImageWidth=176,ImageHeight=192,Collides = true, ColliderSize = new Point(11, 4), ImageOffset=new Point(0,-16) } },
        { 179, new TileDefinition { Name="Shack", ImagePath = BuildingsPath, ImageX=208,ImageY=176,ImageWidth=48,ImageHeight=64,Collides = true, ColliderSize = new Point(3, 2) } },

        // FLOORS (200 - 220)
        {200, new TileDefinition { Name="Checkered Tile Floor",ImagePath = WallsPath, ImageY=48, InstanceType = typeof(Floor)} },
        {201, new TileDefinition { Name="Brown Tile Floor",ImagePath = WallsPath, ImageX=16, ImageY=48, InstanceType = typeof(Floor) } },
        {202, new TileDefinition { Name="Green Tile Floor",ImagePath = WallsPath, ImageX=32, ImageY=48, InstanceType = typeof(Floor) } },
        {203, new TileDefinition { Name="Wooden Floor",ImagePath = WallsPath, ImageX=48, ImageY=48, InstanceType = typeof(Floor) } },
        {204, new TileDefinition { Name="Red Brick Floor",ImagePath = WallsPath, ImageX=64, ImageY=48, InstanceType = typeof(Floor) } },
        {205, new TileDefinition { Name="White Tile Floor",ImagePath = WallsPath, ImageX=80, ImageY=48, InstanceType = typeof(Floor) } },
        {206, new TileDefinition { Name="Blue Brick Floor",ImagePath = WallsPath, ImageX=96, ImageY=48, InstanceType = typeof(Floor) } },
        {207, new TileDefinition { Name="Grey Brick Floor",ImagePath = WallsPath, ImageX=112, ImageY=48, InstanceType = typeof(Floor) } },
        {208, new TileDefinition { Name="Blue Stripe Floor",ImagePath = WallsPath, ImageX=128, ImageY=48, InstanceType = typeof(Floor) } },
        {209, new TileDefinition { Name="White Wood Floor",ImagePath = WallsPath, ImageX=144, ImageY=48, InstanceType = typeof(Floor) } },
        {210, new TileDefinition { Name="Purple Carpet",ImagePath = WallsPath, ImageX=160, ImageY=48, InstanceType = typeof(Floor) } },
        {211, new TileDefinition { Name="Navy Tile Floor",ImagePath = WallsPath, ImageX=176, ImageY=48, InstanceType = typeof(Floor) } },
        {212, new TileDefinition { Name="Hardwood Floor",ImagePath = WallsPath, ImageX=192, ImageY=48, InstanceType = typeof(Floor) } },

        // WALLS (220 - 240)
        {220, new TileDefinition { Name="Wood Wall", ImagePath = WallsPath, ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {221, new TileDefinition { Name="Blue Stripe Wall", ImagePath = WallsPath, ImageX=16,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {222, new TileDefinition { Name="Purple Wall", ImagePath = WallsPath, ImageX=32,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {223, new TileDefinition { Name="Sunset Wall", ImagePath = WallsPath, ImageX=48,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {224, new TileDefinition { Name="Yellow Wall", ImagePath = WallsPath, ImageX=64,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {225, new TileDefinition { Name="Green Wall", ImagePath = WallsPath, ImageX=80,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {226, new TileDefinition { Name="Blue Wall", ImagePath = WallsPath, ImageX=96,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {227, new TileDefinition { Name="Red Stripe Wall", ImagePath = WallsPath, ImageX=112,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {228, new TileDefinition { Name="Night Sky Wall", ImagePath = WallsPath, ImageX=128,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {229, new TileDefinition { Name="Pink Stripe Wall", ImagePath = WallsPath, ImageX=144,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {230, new TileDefinition { Name="White Wall", ImagePath = WallsPath, ImageX=160,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {231, new TileDefinition { Name="Red Wall", ImagePath = WallsPath, ImageX=176,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        {232, new TileDefinition { Name="Baby Blue Wall", ImagePath = WallsPath, ImageX=192,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },

        // CEMETARY (241-245)
        {241, new TileDefinition { Name="Small Gravestone", ImagePath = OutsideDecorPath, ImageX=48,ImageY=80,ImageHeight=32, Collides = true} },
        {242, new TileDefinition { Name="Big Gravestone", ImagePath = OutsideDecorPath, ImageX=64,ImageY=64,ImageHeight=48, ImageWidth=32,Collides = true, ColliderSize = new Point(2, 1)} },
        {243, new TileDefinition { Name="Grave Bone", ImagePath = OutsideDecorPath, ImageX=32,ImageY=112,Collides = true} },
        {244, new TileDefinition { Name="Angel Gravestone", ImagePath = OutsideDecorPath, ImageX=96,ImageY=64,ImageHeight=80, ImageWidth=32,Collides = true, ColliderSize = new Point(2, 1)} },
        {245, new TileDefinition { Name="Fancy Streetlamp", ImagePath = OutsideDecorPath, ImageX=48,ImageY=112,ImageHeight=32, Collides = true, InstanceType = typeof(LampTile) } },

        // FURNITURE (246-300) 
        { 246, new TileDefinition { Name="Cuckoo Clock", ImagePath = DecorationDecorPath, ImageX=224,ImageY=112,ImageHeight=32, ImageOffset = new Point(0, -5) } },
        { 247, new TileDefinition { Name="Wooden Side Table", ImagePath = DecorationDecorPath, ImageX=240,ImageY=112,ImageHeight=32,Collides = true, InstanceType = typeof(UselessFurnitureTile) } },
        { 248, new TileDefinition { Name="Lamp Side Table", ImagePath = DecorationDecorPath, ImageX=256,ImageY=112,ImageHeight=32,Collides = true, InstanceType = typeof(LampTile) } },
        { 249, new TileDefinition { Name="Valley Painting", ImagePath = DecorationDecorPath, ImageX=192, ImageY=128,ImageWidth=32, ImageHeight=32 } },
        { 250, new TileDefinition { Name="Red Bed",ImagePath = DecorationDecorPath, ImageX=96,ImageWidth=48, ImageHeight=64, Collides=true, ColliderSize = new Point(3, 1), InstanceType = typeof(Bed)} },
        { 251, new TileDefinition { Name="White Potted Plant", ImagePath = DecorationDecorPath, ImageX=160, ImageY=80,Collides = true,ImageHeight = 32, InstanceType = typeof(UselessFurnitureTile) } },
        { 252, new TileDefinition { Name="Bamboo Potted Plant", ImagePath = DecorationDecorPath, ImageX=176, ImageY=80,Collides = true, ImageHeight = 32, InstanceType = typeof(UselessFurnitureTile) } },
        { 253, new TileDefinition { Name="Light Stove", ImagePath = DecorationDecorPath, ImageX=208,ImageY=176,ImageHeight=32, Collides = true, InstanceType = typeof(UselessFurnitureTile) } },
        { 254, new TileDefinition { Name="Pink Framed Butterfly", ImagePath = DecorationDecorPath, ImageX=128, ImageY=112,ImageWidth=32, ImageHeight=32, ImageOffset = new Point(0, -5), InstanceType = typeof(UselessFurnitureTile)} },
        { 255, new TileDefinition { Name="Blue Framed Butterfly", ImagePath = DecorationDecorPath, ImageX=160, ImageY=112,ImageWidth=32, ImageHeight=32,ImageOffset = new Point(0, -5), InstanceType = typeof(UselessFurnitureTile)} },
        { 256, new TileDefinition { Name="Blue Couch", ImagePath = DecorationDecorPath, ImageX=240, ImageY=80,ImageWidth=48, ImageHeight=32,Collides=true,ColliderSize=new Point (3,1)} },
        { 257, new TileDefinition { Name="Blue Square Table", ImagePath = DecorationDecorPath, ImageY=64,ImageWidth=32, ImageHeight=48,Collides=true,ColliderSize=new Point (2,2)} },
        { 258, new TileDefinition { Name="Red Square Table", ImagePath = DecorationDecorPath, ImageX=32,ImageY=64,ImageWidth=32, ImageHeight=48,Collides=true,ColliderSize=new Point (2,2)} },
        { 259, new TileDefinition { Name="Wood Square Table", ImagePath = DecorationDecorPath, ImageX=64,ImageY=64,ImageWidth=32, ImageHeight=48,Collides=true,ColliderSize=new Point (2,2)} },
        { 260, new TileDefinition { Name="Fridge", ImagePath = DecorationDecorPath, ImageX=128,ImageY=64,Collides = true, ImageHeight = 48, InstanceType = typeof(UselessFurnitureTile) } },
        { 261, new TileDefinition { Name="Sink", ImagePath = DecorationDecorPath, ImageX=144, ImageY=80,Collides = true, ImageHeight = 32, InstanceType = typeof(UselessFurnitureTile) } },
        { 262, new TileDefinition { Name="Blue Bed",ImagePath = DecorationDecorPath, ImageWidth=48, ImageHeight=64, Collides=true, ColliderSize = new Point(3, 1), InstanceType = typeof(Bed)} },
        { 263, new TileDefinition { Name="Green Table", ImagePath = DecorationDecorPath, ImageX=96,ImageY=112,ImageWidth = 32, ImageHeight = 32, Collides = true, ColliderSize = new Point(2, 1) } },
        { 264, new TileDefinition { Name="Blue Table", ImagePath = DecorationDecorPath, ImageX=64,ImageY=112,ImageWidth = 32, ImageHeight = 32, Collides = true, ColliderSize = new Point(2, 1) } },
        { 265, new TileDefinition { Name="Candlestick", ImagePath = DecorationDecorPath, ImageX=144,ImageY=184,ImageHeight=26, Collides = true, InstanceType = typeof(ThreeWickCandle) } },
        { 266, new TileDefinition { Name="Green Counter", ImagePath = DecorationDecorPath, ImageX=160,ImageY=176,ImageHeight=32, Collides = true } },
        { 267, new TileDefinition { Name="Blue Counter", ImagePath = DecorationDecorPath, ImageX=176,ImageY=176,ImageHeight=32, Collides = true } },
        { 268, new TileDefinition { Name="Pink Counter", ImagePath = DecorationDecorPath, ImageX=192,ImageY=176,ImageHeight=32, Collides = true } },
        { 269, new TileDefinition { Name="Package", ImagePath = DecorationDecorPath, ImageX=224, ImageY=80, ImageHeight=32,Collides = true } },
        { 270, new TileDefinition { Name="Wall Candle", ImagePath = DecorationDecorPath, ImageX=128, ImageY=192, Collides = true, InstanceType = typeof(TwoWickCandle) } },
        { 271, new TileDefinition { Name="Bowl of Lemons", ImagePath = DecorationDecorPath, ImageX=240, ImageY=160, Collides = true } },
        { 272, new TileDefinition { Name="Hanging Pans", ImagePath = DecorationDecorPath, ImageX=128, ImageY=176, Collides = true } },
        { 273, new TileDefinition { Name="Furnace", ImagePath = DecorationDecorPath, ImageX=112,ImageY=64,Collides = true, ImageHeight = 48, InstanceType = typeof(Fireplace) } },
        { 274, new TileDefinition { Name="Purple Lamp", ImagePath = DecorationDecorPath, ImageX=96,ImageY=144,ImageHeight=48, Collides = true, InstanceType = typeof(LampTile) } },
        { 275, new TileDefinition { Name="Blue Lamp", ImagePath = DecorationDecorPath, ImageX=112,ImageY=144,ImageHeight=48, Collides = true, InstanceType = typeof(LampTile) } },
        { 276, new TileDefinition { Name="Laundry Basket", ImagePath = DecorationDecorPath, ImageX=96,ImageY=80,ImageHeight=32, Collides = true } },
        { 277, new TileDefinition { Name="Mesh Divider", ImagePath = DecorationDecorPath, ImageX=256,ImageY=144,ImageWidth=32,ImageHeight=48, Collides = true, ColliderSize= new Point(2,1) } },
        { 278, new TileDefinition { Name="Tile Counter", ImagePath = DecorationDecorPath, ImageX=224,ImageY=176,ImageHeight=32, Collides = true } },
        { 279, new TileDefinition { Name="Dark Stove", ImagePath = DecorationDecorPath, ImageX=240,ImageY=176,ImageHeight=32, Collides = true } },

        // OUTSIDE DECOR (300)
        { 300, new TileDefinition { Name="Fountain", ImagePath = BuildingsPath, Collides = true, ImageX=416, ImageY=16,ImageWidth=64,ImageHeight=80,ColliderSize = new Point(4, 3) } },
        { 301, new TileDefinition { Name="Totem Pole", ImagePath = BuildingsPath, Collides = true, ImageX=368, ImageWidth=48,ImageHeight=144,ColliderOffset = new Point(1, 0) } },
        { 302, new TileDefinition { Name="White Fence", ImagePath = OutsideDecorPath, Collides = true, ImageX=128,ImageY=80,ImageHeight=32 } },
        { 303, new TileDefinition { Name="Decorative Flower Bush 1", ImagePath = OutsideDecorPath, Collides = true, ImageX=144,ImageY=80,ImageWidth=32,ImageHeight=48 } },
        { 304, new TileDefinition { Name="Decorative Flower Bush 2", ImagePath = OutsideDecorPath, Collides = true, ImageX=176,ImageY=80,ImageWidth=32,ImageHeight=48 } },
        { 305, new TileDefinition { Name="Wooden Bench", ImagePath = OutsideDecorPath, Collides = true, ImageY=64,ImageWidth=32,ImageHeight=32, ColliderSize = new Point(2, 1) } },
        { 306, new TileDefinition { Name="Leafy Potted Plant", ImagePath = OutsideDecorPath, Collides = true, ImageX=16,ImageY=32,ImageHeight=32} },
        { 307, new TileDefinition { Name="Flower Wheel Barrel", ImagePath = OutsideDecorPath, Collides = true,  ColliderSize = new Point(2, 1),ImageY=96,ImageWidth=32,ImageHeight=32} },
        { 308, new TileDefinition { Name="Wild Flowers", ImagePath = OutsideDecorPath, ImageX=80,ImageY=112,ImageHeight=32} },
        { 309, new TileDefinition { Name="Task Board", ImagePath = OutsideDecorPath, ImageY=128,ImageWidth=48,ImageHeight=80, Collides=true,ColliderSize = new Point(3, 1)} },
        { 310, new TileDefinition { Name="Swirly Bush", ImagePath = OutsideDecorPath, ImageX=48,ImageY=144,ImageHeight=48, Collides=true} },
        { 311, new TileDefinition { Name="Bush of Balls", ImagePath = OutsideDecorPath, ImageX=64,ImageY=160,ImageHeight=32, Collides=true} },
        { 312, new TileDefinition { Name="Bridge", ImagePath = OutsideDecorPath, ImageX=176,ImageHeight=48,ImageWidth=48, Collides=true, ColliderSize = new Point(2, 3), InstanceType = typeof(Bridge)} },
        { 313, new TileDefinition { Name="Bridge Wall Bottom", ImagePath = OutsideDecorPath, ImageX=176, ImageY=48,ImageHeight=32,ImageWidth=48,Collides = true, ColliderSize = new Point(3, 1) } },
        { 314, new TileDefinition { Name="Bridge Wall Top", ImagePath = OutsideDecorPath, ImageX=176, ImageWidth=48,Collides = true, ColliderSize = new Point(3, 1) } },
        { 315, new TileDefinition { Name="Mailbox", ImagePath = OutsideDecorPath, ImageY=32, Collides = true, ImageHeight = 32, InstanceType = typeof(Mailbox) } },
        { 316, new TileDefinition { Name="Wooden Fence", ImagePath = OutsideDecorPath,ImageX=80, ImageHeight=32, Collides=true, InstanceType = typeof(Fence) } },
        { 317, new TileDefinition { Name="Stone Fence", ImagePath = OutsideDecorPath,ImageX=80, ImageY=32, ImageHeight=32, Collides=true, InstanceType = typeof(Fence) } },
        { 318, new TileDefinition { Name="Flower Display", ImagePath = OutsideDecorPath, Collides = true, ImageWidth=32,ImageHeight=32,ColliderSize = new Point(2, 2), InstanceType = typeof(UselessFurnitureTile) } },
        { 319, new TileDefinition { Name="Metal Street Lamp", ImagePath = OutsideDecorPath, Collides = true, ImageWidth=32,ImageHeight=32,ColliderSize = new Point(2, 2), InstanceType = typeof(LampTile) } },

        // TOWN DECOR (400)
        { 400, new TileDefinition { Name="Church Bench", ImagePath = TownDecorPath, ImageX=48,ImageWidth = 48, ImageHeight = 32, Collides = true, ColliderSize = new Point(3, 1) } },
        { 401, new TileDefinition { Name="Marble Counter", ImagePath = TownDecorPath, ImageX=112,ImageHeight=32,Collides = true } },
        { 402, new TileDefinition { Name="Cloth Counter", ImagePath = TownDecorPath, ImageX=96, ImageHeight=32,Collides = true } },
        { 403, new TileDefinition { Name="Pantry", ImagePath = TownDecorPath, ImageX=48, ImageWidth=32,ImageHeight=64,Collides = true, ColliderSize = new Point(2, 1)} },
        { 404, new TileDefinition { Name="Bible Stand", ImagePath = TownDecorPath, ImageX=128, ImageY=96,ImageHeight=32,Collides = true, InstanceType = typeof(BibleStand)} },
        { 405, new TileDefinition { Name="Microwave", ImagePath = TownDecorPath, ImageX=112, ImageY=32,Collides = true} },
        { 406, new TileDefinition { Name="Metal Fridge", ImagePath = TownDecorPath,ImageX=160,ImageHeight=48, Collides=true } },
        { 407, new TileDefinition { Name="Pink Yellow Stripe Wall Left", ImagePath = TownDecorPath,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 408, new TileDefinition { Name="Pink Yellow Stripe Wall", ImagePath = TownDecorPath, ImageX=16,ImageY=128,ImageHeight=64, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        { 409, new TileDefinition { Name="Pink Yellow Stripe Wall Right", ImagePath = TownDecorPath,ImageX=32,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 410, new TileDefinition { Name="Lacy Wood Table", ImagePath = TownDecorPath, ImageWidth = 16, ImageHeight = 48, Collides = true, ColliderSize = new Point(1, 2) } },
        { 411, new TileDefinition { Name="Cat Painting", ImagePath = TownDecorPath, ImageX=16,ImageWidth = 32, ImageHeight = 32, Collides = true, ColliderSize = new Point(2, 2), ImageOffset = new Point(0, -5) } },
        { 412, new TileDefinition { Name="Wood Armoire", ImagePath = TownDecorPath, ImageX=16, ImageY=32,ImageWidth=32, ImageHeight=48, Collides=true, ColliderSize = new Point(2, 1) } },
        { 413, new TileDefinition { Name="Flag Quilt", ImagePath = TownDecorPath, ImageX=144, ImageY=96,ImageWidth=32, ImageHeight=32, Collides=true, ColliderSize = new Point(2, 2), ImageOffset = new Point(0, -5) } },
        { 414, new TileDefinition { Name="Brick Fireplace", ImagePath = TownDecorPath, ImageX=176, ImageY=80,ImageWidth=32, ImageHeight=48, Collides=true, ColliderSize = new Point(2, 2) } },
        { 415, new TileDefinition { Name="Ironing Board", ImagePath = TownDecorPath, ImageX=160, ImageY=48,ImageWidth=32, ImageHeight=32, Collides=true, ColliderSize = new Point(2, 1) } },
        { 416, new TileDefinition { Name="Dark Purple Wall Left", ImagePath = TownDecorPath,ImageX=48,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 417, new TileDefinition { Name="Dark Purple Wall", ImagePath = TownDecorPath, ImageX=64,ImageY=128,ImageHeight=64, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        { 418, new TileDefinition { Name="Dark Purple Wall Right", ImagePath = TownDecorPath,ImageX=80,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 419, new TileDefinition { Name="Blue Green Stripe Wall Left", ImagePath = TownDecorPath,ImageX=96,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 420, new TileDefinition { Name="Blue Green Stripe Wall", ImagePath = TownDecorPath, ImageX=112,ImageY=128,ImageHeight=64, Collides = true, ColliderSize = new Point(1, 3), InstanceType = typeof(Wall)} },
        { 421, new TileDefinition { Name="Blue Green Stripe Wall Right", ImagePath = TownDecorPath,ImageX=128,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 422, new TileDefinition { Name="Yellow Wall Left", ImagePath = TownDecorPath,ImageX=144,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 423, new TileDefinition { Name="Yellow Wall Right", ImagePath = TownDecorPath,ImageX=160,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 424, new TileDefinition { Name="Baby Blue Wall Left", ImagePath = TownDecorPath,ImageX=176,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 425, new TileDefinition { Name="Baby Blue Wall Right", ImagePath = TownDecorPath,ImageX=192,ImageY=128,ImageHeight=80, Collides = true, ColliderSize = new Point(1, 3), ImageOffset=new Point (0,-16), ColliderOffset = new Point(0,1) } },
        { 426, new TileDefinition { Name="Cooler", ImagePath = TownDecorPath,ImageY=48,ImageHeight=32, Collides = true } },
        { 427, new TileDefinition { Name="Comfy Pink Chair", ImagePath = TownDecorPath,ImageX=240,ImageY=96,ImageWidth=32,ImageHeight=32, Collides = true, ColliderSize = new Point(2, 1) } },
        { 428, new TileDefinition { Name="White Curtain", ImagePath = TownDecorPath,ImageX=272,ImageY=80,ImageHeight=48, Collides = true, ColliderSize = new Point(1, 3) } },
        { 429, new TileDefinition { Name="Quilted Chair Down", ImagePath = TownDecorPath,ImageX=192,ImageY=16,ImageHeight=32, Collides = true } },
        { 430, new TileDefinition { Name="Quilted Chair Right", ImagePath = TownDecorPath,ImageX=208,ImageY=16,ImageHeight=32, Collides = true } },
        { 431, new TileDefinition { Name="Hat Rack", ImagePath = TownDecorPath,ImageX=224,ImageHeight=48, Collides = true } },
        { 432, new TileDefinition { Name="Purple Couch", ImagePath = TownDecorPath,ImageX=192,ImageY=48,ImageWidth=48,ImageHeight=32, Collides = true, ColliderSize=new Point(3,1)} },
        { 433, new TileDefinition { Name="White Bed", ImagePath = TownDecorPath,ImageX=240,ImageWidth=48,ImageHeight=64, Collides = true, ColliderSize=new Point(3,1), InstanceType = typeof(Bed) } },
        { 434, new TileDefinition { Name="Bamboo Side Table", ImagePath = TownDecorPath,ImageX=176,ImageHeight=32, Collides = true } },

        // RUGS
        { 500, new TileDefinition { Name="Large Purple Rug", ImagePath = RugsPath,ImageWidth=96,ImageHeight=64 } },
        { 501, new TileDefinition { Name="Blue Rug", ImagePath = RugsPath,ImageY=64,ImageWidth=48,ImageHeight=32 } },
        { 502, new TileDefinition { Name="Orange Rug", ImagePath = RugsPath,ImageY=64,ImageWidth=48,ImageHeight=32 } },
        { 503, new TileDefinition { Name="Red Rug", ImagePath = RugsPath,ImageX=96, ImageY=32,ImageWidth=48, ImageHeight=96, ImageOffset = new Point(8, 0) } },
        { 504, new TileDefinition { Name="Marigold Rug", ImagePath = RugsPath,ImageX=96,ImageWidth=32,ImageHeight=16 } },
        { 505, new TileDefinition { Name="Blue Stripe Rug", ImagePath = RugsPath,ImageX=96,ImageY=16,ImageWidth=32,ImageHeight=16 } },
        { 506, new TileDefinition { Name="Round Pink Rug", ImagePath = RugsPath,ImageX=48, ImageY=96,ImageWidth=48,ImageHeight=32 } },
        { 507, new TileDefinition { Name="Small Purple Rug", ImagePath = RugsPath,ImageX=48, ImageY=64,ImageWidth=48,ImageHeight=32 } },
        { 508, new TileDefinition { Name="White Woven Rug", ImagePath = RugsPath,ImageX=144,ImageWidth=80,ImageHeight=48 } },

        // DOORS (600)
        { 600, new TileDefinition { Name="Cabin Inside Door", ImagePath = WallsPath, ImageX=192,ImageY=64,Collides = true, ImageOffset=new Point (0,6),InstanceType = typeof(CabinInnerDoor) } },
        { 601, new TileDefinition { Name="Cabin Door", ImageX=96, ImageY=16,ImagePath = WinterOutsideDecorPath, Collides = true, InstanceType = typeof(CabinOuterDoor) } },
        { 602, new TileDefinition { Name="Church Door", ImagePath = WinterBuildingsPath, Collides = true, ColliderSize = new Point(2, 1), InstanceType = typeof(ChurchOuterDoor)} },
        { 603, new TileDefinition { Name="Church Inside Door", ImagePath = WallsPath,  ImageX=192, ImageY=64,Collides = true, ImageOffset=new Point (0,6),InstanceType = typeof(ChurchInnerDoor) } },
        { 604, new TileDefinition { Name="Church Wall", ImagePath = WallsChurchWallPath, ImageWidth = 12*TileSize,ImageHeight = 7*TileSize, Collides = true, ColliderSize = new Point(12, 1)} },
        { 605, new TileDefinition { Name="Dot's Door", ImagePath = WinterTerrainPath, ImageX=240,ImageY=48,ImageWidth=128, InstanceType = typeof(DotsHouseOuterDoor) } },
        { 606, new TileDefinition { Name="Dot's Inside Door", ImagePath = WallsPath,  ImageX=192, ImageY=64,Collides = true, ImageOffset=new Point (0,6),InstanceType = typeof(DotsHouseInnerDoor) } },
        { 607, new TileDefinition { Name="General Store Door", ImagePath = WinterTerrainPath, ImageX=240,ImageY=48,ImageWidth=128 } },
        { 608, new TileDefinition { Name="General Store Inside Door", ImagePath = WallsPath,  ImageX=192, ImageY=64,Collides = true, ImageOffset=new Point (0,6) } },
        { 609, new TileDefinition { Name="Post Office Door", ImagePath = WinterTerrainPath, ImageX=240,ImageY=48,ImageWidth=128 } },
        { 610, new TileDefinition { Name="Post Office Inside Door", ImagePath = WallsPath,  ImageX=192, ImageY=64,Collides = true, ImageOffset=new Point (0,6) } },
    };
}