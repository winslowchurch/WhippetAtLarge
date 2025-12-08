using System.Linq;
using Honeyspur;
using Honeyspur.Items;
using Microsoft.Xna.Framework.Content;

public class DroppedItem
{
    private static readonly Random random = new();
    private static Texture2D shadowTexture;

    public Vector2 Position;
    public Vector2 Velocity;
    public ItemInstance Item;
    public bool IsCollected = false;

    private Vector2 groundPosition;
    private bool hasLanded = false;
    private bool hasCompletedInitialArc = false;
    private float initialHeight;

    public static void LoadContent(ContentManager content)
    {
        shadowTexture = TextureManager.GetTexture("graphics/objects");
    }

    public DroppedItem(ItemInstance item, Vector2 position, float heightOffset = 16f)
    {
        Item = item;
        initialHeight = heightOffset;

        int bushTileX = (int)(position.X / 16);
        int bushTileY = (int)(position.Y / 16);

        int tileOffsetX = random.Next(-1, 2);
        int tileOffsetY = random.Next(-1, 2);

        int dropTileX = bushTileX + tileOffsetX;
        int dropTileY = bushTileY + tileOffsetY;

        float tilePosX = dropTileX * 16 + 8; // Center on tile
        float tilePosY = dropTileY * 16 + 8;

        groundPosition = new Vector2(tilePosX, tilePosY);
        Position = new Vector2(position.X, position.Y - heightOffset); // Start higher

        // Calculate initial velocity for a nice arc
        float gravity = 1200f;
        float duration = 0.6f;
        float minInitialYVelocity = -200f; // Minimum upward velocity

        Vector2 displacement = groundPosition - Position;
        float initialXVelocity = displacement.X / duration;
        float calculatedYVelocity = (displacement.Y - 0.5f * gravity * duration * duration) / duration;

        // Use the more upward of the two velocities
        float initialYVelocity = Math.Min(calculatedYVelocity, minInitialYVelocity);

        Velocity = new Vector2(initialXVelocity, initialYVelocity);
    }

    public void Update(GameTime gameTime, GameContext context)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!hasLanded)
        {
            Velocity.Y += 1200f * dt;
            Position += Velocity * dt;

            // Check for landing
            if (Position.Y >= groundPosition.Y)
            {
                Position.Y = groundPosition.Y;
                hasCompletedInitialArc = true;

                if (Math.Abs(Velocity.Y) > 100f)
                {
                    Velocity.Y *= -0.4f;
                    Velocity.X *= 0.6f;
                }
                else
                {
                    Velocity = Vector2.Zero;
                    hasLanded = true;
                }
            }
        }
        else if (hasCompletedInitialArc) // Only allow magnetism after completing initial arc
        {
            if (context.UIManager.CanAddItem(Item) &&
                Vector2.Distance(Position, context.Player.Position) < 40)
            {
                Vector2 toPlayer = context.Player.Position - Position;
                Velocity += Vector2.Normalize(toPlayer) * 0.5f;
                Position += Velocity;
                Velocity *= 0.75f;
            }
        }
    }

    public bool TryCollect(GameContext context)
    {
        // Only allow collection after landing and completing initial arc
        if (hasLanded && hasCompletedInitialArc &&
            Vector2.Distance(Position, context.Player.Position) < 16)
        {
            if (context.UIManager.AddItemToInventory(Item))
            {
                IsCollected = true;
                SoundPlayer.Play("woo");
                return true;
            }
        }
        return false;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (Item?.Icon == null) return;

        // Draw shadow at ground position
        if (shadowTexture != null)
        {
            float shadowScale = 1f;
            if (!hasLanded)
            {
                // Scale shadow based on height from ground
                float heightDiff = Math.Abs(Position.Y - groundPosition.Y);
                shadowScale = Math.Max(0.5f, 1f - (heightDiff / initialHeight * 0.5f));
            }

            var shadowSourceRect = new Rectangle(0, 0, 16, 16);
            spriteBatch.Draw(shadowTexture,
                groundPosition,
                shadowSourceRect,
                Color.White,
                0f,
                new Vector2(8, 8),
                shadowScale,
                SpriteEffects.None,
                0f);
        }

        // Draw item
        var sourceRect = new Rectangle(Item.IconX, Item.IconY, 16, 16);
        spriteBatch.Draw(Item.Icon,
            Position,
            sourceRect,
            Color.White,
            0f,
            new Vector2(8, 8), // Center the sprite
            1f,
            SpriteEffects.None,
            0f);
    }

    public static void UpdateAll(GameContext context, GameTime gameTime)
    {
        foreach (var drop in context.DroppedItems.ToList())
        {
            drop.Update(gameTime, context);
            if (drop.TryCollect(context) || drop.IsCollected)
                context.DroppedItems.Remove(drop);
        }
    }

    public static void DrawAll(List<DroppedItem> droppedItems, SpriteBatch spriteBatch)
    {
        foreach (var drop in droppedItems)
            drop.Draw(spriteBatch);
    }
}