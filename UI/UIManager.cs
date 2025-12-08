using Honeyspur.Items;
using Honeyspur.UI;
using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Text.Json;

namespace Honeyspur;

public class UIManager
{
    private readonly CoinBar CoinBar = new();
    private readonly InventoryBar InventoryBar = new();
    private readonly LifeBar LifeBar = new();
    public PromptBox PromptBox = new();
    public AnnouncementBox AnnouncementBox = new();
    private readonly ItemInfoBox ItemInfoBox = new();
    
    private Texture2D uiTexture;
    private readonly UIAtlas uiAtlas = new();

    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
        // Load shared UI texture and atlas
        uiTexture = TextureManager.GetTexture("graphics/ui");
        uiAtlas.Load(Path.Combine("data", "ui_atlas.json"));
        
        // Pass shared resources to UI components
        CoinBar.LoadContent(uiTexture, uiAtlas);
        InventoryBar.LoadContent(uiTexture, uiAtlas);
        LifeBar.LoadContent(content, uiTexture, uiAtlas);
        PromptBox.LoadContent(graphicsDevice, uiTexture, uiAtlas);
        AnnouncementBox.LoadContent(graphicsDevice, uiTexture, uiAtlas);
        ItemInfoBox.LoadContent(uiTexture, uiAtlas);
    }

    // Return true when any modal UI that should block world input is visible
    public bool IsModalOpen()
    {
        return (PromptBox != null && PromptBox.IsVisible()) ||
               (AnnouncementBox != null && AnnouncementBox.IsVisible());
    }

    public void AddCoins(int coins)
    {
        CoinBar.AddCoins(coins);
        SoundPlayer.Play("coinJingle");
    }

    public bool AddItemToInventory(ItemInstance item)
    {
        return InventoryBar.TryAddItem(item);
    }

    public void SetInventoryBarSelectedIndex(int index)
    {
        InventoryBar.SetSelectedIndex(index);
    }

    public void RemoveSelectedInventoryItem()
    {
        InventoryBar.RemoveSelectedInventoryItem();
    }

    public ItemInstance GetSelectedInventoryItem()
    {
        return InventoryBar.GetSelectedItem();
    }

    public void Draw(SpriteBatch spriteBatch, Viewport viewport, GameContext context)
    {
        CoinBar.Draw(spriteBatch, context);
        InventoryBar.Draw(spriteBatch, viewport);
        LifeBar.Draw(spriteBatch, viewport);
        PromptBox.Draw(spriteBatch, viewport);
        AnnouncementBox.Draw(spriteBatch, viewport);
        ItemInfoBox.Draw(spriteBatch, viewport);
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        CoinBar.TickUpdate(gameTime, context);
        LifeBar.TickUpdate(gameTime, context.Player.Health, context.Player.MaxHealth);
        AnnouncementBox.TickUpdate(gameTime);

        // Update item info box with hovered inventory item
        MouseState mouseState = Mouse.GetState();
        Point mousePos = new(mouseState.X, mouseState.Y);
        ItemInstance hoveredItem = GetHoveredInventoryItem(mousePos, context.GraphicsDevice.Viewport);
        ItemInfoBox.SetItem(hoveredItem);
    }

    // Returns whether any of ui were clicked
    public bool HandleClick(Point mouseScreenPos, Viewport viewport, GameContext context)
    {
        if (PromptBox != null && PromptBox.IsVisible())
        {
            bool promptBoxClicked = PromptBox.HandleClick(mouseScreenPos, context);
            if (promptBoxClicked) return true;
        }
        
        if (AnnouncementBox != null && AnnouncementBox.IsVisible())
        {
            bool announcementBoxClicked = AnnouncementBox.HandleClick(mouseScreenPos, context);
            if (announcementBoxClicked) return true;
        }
        
        return InventoryBar.HandleClick(mouseScreenPos, viewport);
    }

    public bool CanAddItem(ItemInstance item)
    {
        return InventoryBar.CanAddItem(item);
    }


    public bool IsHoveringOverUI(Point mouseScreenPos, GameContext context)
    {
        if (PromptBox.IsHovering(mouseScreenPos, context))
            return true;

        if (AnnouncementBox.IsHovering(mouseScreenPos, context))
            return true;

        if (InventoryBar.IsHovering(mouseScreenPos, context))
            return true;

        return false;
    }

    private ItemInstance GetHoveredInventoryItem(Point mousePos, Viewport viewport)
    {
        float scale = 4f;
        
        // Get inventory bar rectangle from atlas
        Rectangle barSrc = uiAtlas.GetRect("inventoryBar");
        Vector2 barPosition = new(viewport.Width / 2f - (barSrc.Width * scale / 2f), viewport.Height - barSrc.Height * scale - 10);
        float slotSize = 19 * scale;
        float padding = 4 * scale;

        for (int i = 0; i < InventoryBar.SlotCount; i++)
        {
            Vector2 slotPos = new(
                barPosition.X + padding + i * slotSize,
                barPosition.Y + padding
            );

            Rectangle slotRect = new(
                (int)slotPos.X, (int)slotPos.Y,
                (int)slotSize, (int)slotSize
            );

            if (slotRect.Contains(mousePos))
            {
                return InventoryBar.GetItemAt(i);
            }
        }
        return null;
    }
}

public class UIAtlas
{
    private class AtlasEntry { public int X { get; set; } public int Y { get; set; } public int W { get; set; } public int H { get; set; } }

    private Dictionary<string, AtlasEntry> entries = [];

    public void Load(string jsonPath)
    {
        if (!File.Exists(jsonPath)) return;
        var json = File.ReadAllText(jsonPath);
        try
        {
            entries = JsonSerializer.Deserialize<Dictionary<string, AtlasEntry>>(json) ?? [];
        }
        catch
        {
            entries = new Dictionary<string, AtlasEntry>();
        }
    }

    public Rectangle GetRect(string name)
    {
        if (entries.TryGetValue(name, out var e))
            return new Rectangle(e.X, e.Y, e.W, e.H);

        return Rectangle.Empty;
    }
}

