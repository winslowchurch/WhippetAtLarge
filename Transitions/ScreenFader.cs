namespace Honeyspur;

public class ScreenFader
{
    private float alpha = 0f;
    private float speed = 0.8f;
    private bool fadingIn = false;
    private bool fadingOut = false;
    private Action onFadeOutComplete;

    public bool IsFading => fadingIn || fadingOut;

    public void StartFadeOut(float speed, Action onComplete)
    {
        this.speed = speed;
        fadingOut = true;
        fadingIn = false;
        onFadeOutComplete = onComplete;
    }

    public void StartFadeIn(float speed)
    {
        this.speed = speed;
        fadingIn = true;
        fadingOut = false;
    }

    public void Update(GameTime gameTime)
    {
        float delta = (float)gameTime.ElapsedGameTime.TotalSeconds * speed;

        if (fadingOut)
        {
            alpha += delta;
            if (alpha >= 1f)
            {
                alpha = 1f;
                fadingOut = false;
                onFadeOutComplete?.Invoke();
                StartFadeIn(speed);
            }
        }
        else if (fadingIn)
        {
            alpha -= delta;
            if (alpha <= 0f)
            {
                alpha = 0f;
                fadingIn = false;
            }
        }
    }

    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (alpha <= 0f) return;

        Texture2D blackTexture = new Texture2D(graphicsDevice, 1, 1);
        blackTexture.SetData(new[] { Color.Black });

        spriteBatch.Begin();
        spriteBatch.Draw(blackTexture,
            new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
            Color.Black * alpha);
        spriteBatch.End();
    }
}
