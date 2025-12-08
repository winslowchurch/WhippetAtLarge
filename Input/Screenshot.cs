namespace Honeyspur;

public static class ScreenshotHelper
{
    public static void TakeScreenshot(GraphicsDevice graphicsDevice)
    {
        int width = graphicsDevice.PresentationParameters.BackBufferWidth;
        int height = graphicsDevice.PresentationParameters.BackBufferHeight;

        Texture2D screenshot = new Texture2D(graphicsDevice, width, height, false, SurfaceFormat.Color);
        Color[] data = new Color[width * height];
        graphicsDevice.GetBackBufferData(data);
        screenshot.SetData(data);

        string path = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"Honeyspur_Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png"
        );
        using (var stream = System.IO.File.Create(path))
        {
            screenshot.SaveAsPng(stream, width, height);
        }

        screenshot.Dispose();
    }
}