using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Honeyspur;

public class TextureManager
{
    private static Dictionary<string, Texture2D> textureCache = new();
    private static ContentManager content;

    public static void Initialize(ContentManager contentManager)
    {
        content = contentManager;
    }

    public static Texture2D GetTexture(string texturePath)
    {
        if (!textureCache.ContainsKey(texturePath))
        {
            textureCache[texturePath] = content.Load<Texture2D>(texturePath);
        }
        return textureCache[texturePath];
    }

    public static void UnloadTexture(string texturePath)
    {
        if (textureCache.ContainsKey(texturePath))
        {
            textureCache[texturePath].Dispose();
            textureCache.Remove(texturePath);
        }
    }

    public static void UnloadAll()
    {
        foreach (var texture in textureCache.Values)
        {
            texture.Dispose();
        }
        textureCache.Clear();
    }
}