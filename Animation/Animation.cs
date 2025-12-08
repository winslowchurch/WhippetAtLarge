public class Animation
{
    private int[] frameSequence;
    private int currentFrameIndex;
    private int row;
    private int frameRate;
    private double timer;

    public Animation(int[] sequence, int row, int frameRate)
    {
        frameSequence = sequence;
        this.row = row;
        this.frameRate = frameRate;
        currentFrameIndex = 0;
        timer = 0;
    }

    public void Update(GameTime gameTime)
    {
        timer += gameTime.ElapsedGameTime.TotalSeconds;
        if (timer > 1.0 / frameRate)
        {
            currentFrameIndex = (currentFrameIndex + 1) % frameSequence.Length;
            timer = 0;
        }
    }

    public Rectangle GetCurrentFrameRect()
    {
        return new Rectangle(
            frameSequence[currentFrameIndex] * 16,
            row * 32,
            16,
            32
        );
    }

    // Return the actual frame value from the sequence (e.g. 0,1,0,2)
    public int GetCurrentFrameValue()
    {
        return frameSequence[currentFrameIndex];
    }

    public int GetCurrentFrameIndex()
    {
        return currentFrameIndex;
    }
}