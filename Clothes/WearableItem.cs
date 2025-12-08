namespace Honeyspur;
using System.Text.Json.Serialization;
using System.Linq;

public class WearableItem
{
    public string Name { get; set; }
    public int SpriteColumn { get; set; }
    public Texture2D SpriteSheet { get; set; }
    public int FrameWidth { get; set; } = 16;
    public int FrameHeight { get; set; } = 32;
    public int RowHeight { get; set; } = 32;

    public Rectangle GetFrameRect(string direction)
    {
        int row = direction switch
        {
            "down" => 0,
            "right" => 1,
            "left" => 2,
            "up" => 3,
            _ => 0
        };

        return new Rectangle(
            SpriteColumn * FrameWidth,
            row * RowHeight,
            FrameWidth,
            FrameHeight
        );
    }

    // Overload for items (like shoes) that need to use the current animation frame
    // frameValue should be 0,1,2 etc matching the animation sequence columns
    public Rectangle GetFrameRect(string direction, int frameValue)
    {
        int row = direction switch
        {
            "down" => 0,
            "right" => 1,
            "left" => 1,
            "up" => 2,
            _ => 0
        };

        return new Rectangle(
            (SpriteColumn*3 + frameValue) * FrameWidth,
            row * RowHeight,
            FrameWidth,
            FrameHeight
        );
    }
}

public class StyleInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("spriteColumn")]
    public int SpriteColumn { get; set; }
}

public class StyleData<T> where T : StyleInfo
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; }

    public T GetStyle(string name) => Items.First(i => i.Name == name);
}

// Specific style types (minimal implementation)
public class HairstyleData : StyleData<StyleInfo> 
{
    [JsonPropertyName("hairstyles")]
    public new List<StyleInfo> Items { get; set; }
}

public class ShirtStyleData : StyleData<StyleInfo>
{
    [JsonPropertyName("shirtstyles")]
    public new List<StyleInfo> Items { get; set; }
}

public class HatStyleData : StyleData<StyleInfo>
{
    [JsonPropertyName("hatstyles")]
    public new List<StyleInfo> Items { get; set; }
}

public class ShoeStyleData : StyleData<StyleInfo>
{
    [JsonPropertyName("shoestyles")]
    public new List<StyleInfo> Items { get; set; }
}

public class PantsStyleData : StyleData<StyleInfo>
{
    [JsonPropertyName("pantstyles")]
    public new List<StyleInfo> Items { get; set; }
}

public class ShoeItem : WearableItem
{
    public ShoeItem()
    {
        FrameWidth = 16;
        FrameHeight = 16;
        RowHeight = 16;
    }
}

public class PantsItem : WearableItem
{
    public PantsItem()
    {
        FrameWidth = 16;
        FrameHeight = 16;
        RowHeight = 16;
    }
}