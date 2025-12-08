using Honeyspur.Items;
using Honeyspur.UI;

namespace Honeyspur;

public class InventoryBar
{
    public const int SlotCount = 10;
    private Texture2D uiTexture;
    private UIAtlas atlas;
    private readonly List<ItemInstance> items = [.. new ItemInstance[SlotCount]];

    private int selectedIndex = 0;

    public void LoadContent(Texture2D sharedUiTexture, UIAtlas sharedAtlas)
    {
        // Use shared UI texture and atlas from UIManager
        uiTexture = sharedUiTexture;
        atlas = sharedAtlas;

        TryAddItem(ItemRegistry.Get("Shears"));
        TryAddItem(ItemRegistry.Get("Spray Bottle"));
        TryAddItem(ItemRegistry.Get("Dahlia Seed"));
        TryAddItem(ItemRegistry.Get("Dahlia Seed"));
        TryAddItem(ItemRegistry.Get("Rocket Pop"));
        TryAddItem(ItemRegistry.Get("Wooden Hoe"));
    }

    public void Draw(SpriteBatch spriteBatch, Viewport viewport)
    {
        float scale = 4f;

        // Get inventory bar rectangle from atlas
        Rectangle barSrc = atlas.GetRect("inventoryBar");
        Vector2 barPosition = new(viewport.Width / 2f - (barSrc.Width * scale / 2f), viewport.Height - barSrc.Height * scale - 10);
        spriteBatch.Draw(uiTexture, barPosition, barSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

        // Get mouse position for hover detection
        MouseState mouseState = Mouse.GetState();
        Point mousePos = new(mouseState.X, mouseState.Y);

        DrawItems(spriteBatch, barPosition, scale, mousePos, viewport);
    }

    private void DrawItems(SpriteBatch spriteBatch, Vector2 barPosition, float scale, Point mousePos, Viewport viewport)
    {
        float slotSize = 17 * scale;
        float spacing = 2 * scale;
        float padding = 4 * scale;

        Rectangle inventoryBoxSrc = atlas.GetRect("inventoryBox");
        Rectangle inventoryBoxHoverSrc = atlas.GetRect("inventoryBoxHover");

        for (int i = 0; i < SlotCount; i++)
        {
            Vector2 slotPos = new(
                barPosition.X + padding + i * (slotSize + spacing),
                barPosition.Y + padding
            );

            // Check if mouse is hovering over this slot
            Rectangle slotRect = new(
                (int)slotPos.X,
                (int)slotPos.Y,
                (int)slotSize,
                (int)slotSize
            );
            bool isHovered = slotRect.Contains(mousePos);

            // Draw slot box (normal or hover version)
            Rectangle slotBoxSrc = isHovered ? inventoryBoxHoverSrc : inventoryBoxSrc;
            spriteBatch.Draw(uiTexture, slotPos, slotBoxSrc, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // Highlight selected slot
            if (i == selectedIndex)
            {
                Rectangle highlightSrc = atlas.GetRect("inventorySlotHighlight");
                Vector2 highlightPos = slotPos + new Vector2(-scale, -scale);
                spriteBatch.Draw(uiTexture, highlightPos, highlightSrc, Color.White * 0.8f, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            // Draw item if available
            if (items[i] != null)
            {
                var item = items[i]!;
                
                // Handle furniture items differently to preserve pixel quality
                if (item.Category == ItemCategory.Furniture)
                {
                    // Furniture icons are up to 64x64, draw them with calculated scale
                    int iconWidth = item.Icon.Width;
                    int iconHeight = item.Icon.Height;
                    
                    // Calculate scale to fit within slot (target 64 pixels max, which fits in the 68 pixel slot)
                    float furnitureScale = Math.Min(64f / iconWidth, 64f / iconHeight);
                    
                    // Center the furniture icon in the slot
                    float scaledWidth = iconWidth * furnitureScale;
                    float scaledHeight = iconHeight * furnitureScale;
                    float offsetX = (slotSize - scaledWidth) / 2f;
                    float offsetY = (slotSize - scaledHeight) / 2f;
                    
                    Vector2 furniturePos = slotPos + new Vector2(offsetX, offsetY);
                    var sourceRect = new Rectangle(item.IconX, item.IconY, iconWidth, iconHeight);
                    spriteBatch.Draw(item.Icon, furniturePos, sourceRect, Color.White, 0f, Vector2.Zero, furnitureScale, SpriteEffects.None, 0f);
                }
                else
                {
                    // Normal items are 16x16, scale by 4
                    var sourceRect = new Rectangle(item.IconX, item.IconY, 16, 16);
                    spriteBatch.Draw(item.Icon, slotPos, sourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }

                // Only show quantity if more than 1
                if (item.Quantity > -1)
                {
                    var font = Game1.BorderFont;
                    var quantityText = item.Quantity.ToString();
                    var textPos = slotPos + new Vector2(0, 10) * scale;
                    var textScale = 0.15f * scale;

                    TextHelper.DrawBorderedText(
                        spriteBatch,
                        font,
                        quantityText,
                        textPos,
                        Color.White,
                        Game1.Colors.DarkRed,
                        textScale
                    );
                }
            }
        }
    }

    public bool TryAddItem(ItemInstance item)
    {
        if (item == null)
            return false;
        // Try to stack with existing item
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i]!.Name == item.Name)
            {
                items[i]!.Quantity += item.Quantity;
                return true;
            }
        }
        // Find first empty slot
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return true;
            }
        }
        // No available slots
        return false;
    }

    public bool CanAddItem(ItemInstance item)
    {
        if (item == null)
            return false;
        // Can stack with existing item
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i]!.Name == item.Name)
                return true;
        }
        // Or has empty slot
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                return true;
        }
        return false;
    }

    public void SetSelectedIndex(int index)
    {
        selectedIndex = Math.Clamp(index, 0, SlotCount - 1);
    }

    public void RemoveSelectedInventoryItem()
    {
        if (selectedIndex < items.Count && items[selectedIndex] != null)
        {
            var item = items[selectedIndex]!;
            item.Quantity--;
            if (item.Quantity <= 0)
                items[selectedIndex] = null;
        }
    }

    public ItemInstance GetSelectedItem()
    {
        return selectedIndex < items.Count ? items[selectedIndex] : null;
    }

    public bool HandleClick(Point mouseScreenPos, Viewport viewport)
    {
        float scale = 4f;

        // Get inventory bar rectangle from atlas
        Rectangle barSrc = atlas.GetRect("inventoryBar");
        Vector2 barPosition = new(viewport.Width / 2f - (barSrc.Width * scale / 2f), viewport.Height - barSrc.Height * scale - 10);

        float slotSize = 17 * scale;
        float spacing = 2 * scale;
        float padding = 4 * scale;

        for (int i = 0; i < SlotCount; i++)
        {
            Vector2 slotPos = new(
                barPosition.X + padding + i * (slotSize + spacing),
                barPosition.Y + padding
            );

            Rectangle slotRect = new(
                (int)slotPos.X, (int)slotPos.Y,
                (int)slotSize, (int)slotSize
            );

            if (slotRect.Contains(mouseScreenPos))
            {
                selectedIndex = i;
                SoundPlayer.Play("woo");
                return true;
            }
        }
        return false;
    }

    public ItemInstance GetItemAt(int index)
    {
        return index < items.Count ? items[index] : null;
    }

    public bool IsHovering(Point mouseScreenPos, GameContext context)
    {
        float scale = 4f;
        Viewport viewport = context.GraphicsDevice.Viewport;

        // Get inventory bar rectangle from atlas
        Rectangle barSrc = atlas.GetRect("inventoryBar");
        Vector2 barPosition = new(viewport.Width / 2f - (barSrc.Width * scale / 2f), viewport.Height - barSrc.Height * scale - 10);

        Rectangle inventoryBarRect = new Rectangle(
            (int)barPosition.X,
            (int)barPosition.Y,
            (int)(barSrc.Width * scale),
            (int)(barSrc.Height * scale)
        );

        return inventoryBarRect.Contains(mouseScreenPos);
    }
}