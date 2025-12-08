namespace Honeyspur;

public class Camera2D
{
    public Vector2 Position { get; set; } = Vector2.Zero;
    public float Zoom { get; set; } = 5f;

    public Matrix GetViewMatrix()
    {
        // Round position to nearest integer for pixel-perfect rendering
        var roundedPos = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
        return
            Matrix.CreateTranslation(new Vector3(-roundedPos, 0f)) *
            Matrix.CreateScale(Zoom, Zoom, 1f);
    }

    public void ClampToMap(int mapWidth, int mapHeight, int viewportWidth, int viewportHeight, int tileSize)
    {
        float cameraWidth = viewportWidth / Zoom;
        float cameraHeight = viewportHeight / Zoom;
        float mapPixelWidth = mapWidth * tileSize;
        float mapPixelHeight = mapHeight * tileSize;
        float maxX = mapPixelWidth - cameraWidth;
        float maxY = mapPixelHeight - cameraHeight;

        float newX, newY;

        // Center map horizontally
        if (mapPixelWidth < cameraWidth)
            newX = (mapPixelWidth - cameraWidth) / 2f;
        else
            newX = MathHelper.Clamp(Position.X, 0, Math.Max(0, maxX));
        
        // Center map vertically
        if (mapPixelHeight < cameraHeight)
            newY = (mapPixelHeight - cameraHeight) / 2f;
        else
            newY = MathHelper.Clamp(Position.Y, 0, Math.Max(0, maxY));

        Position = new Vector2(newX, newY);
    }

    public void CenterOnPlayer(Player player, int viewportWidth, int viewportHeight)
    {
        Position = player.Position + new Vector2(player.Width / 2f, player.Height / 2f)
                   - new Vector2(viewportWidth / (2f * Zoom), viewportHeight / (2f * Zoom));
    }

    public void CenterAndClampToPlayer(GameContext context, GraphicsDevice graphicsDevice)
    {
        CenterOnPlayer(context.Player, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height);
        ClampToMap(
            context.MapManager.CurrentMap.Width,
            context.MapManager.CurrentMap.Height,
            graphicsDevice.Viewport.Width,
            graphicsDevice.Viewport.Height,
            16
        );
    }
}