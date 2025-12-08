namespace Honeyspur;

public static class PlayerSprites
{
    // Base sprite dimensions
    public const int FRAME_WIDTH = 16;
    public const int FRAME_HEIGHT = 32;
    public const int SMALL_FRAME_HEIGHT = 16; // For eyes, etc.

    // Base body animations (main character sprite)
    public static class Body
    {
        public static Rectangle GetFrame(string direction, int frameIndex)
        {
            int row = Utils.GetRowForDirection(direction);
            return new Rectangle(frameIndex * FRAME_WIDTH, row * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
        }
    }

    // Arm animations (overlay on top of body)
    public static class Arms
    {
        private const int ARMS_X_OFFSET = 48; // Arms start at x=48 in sprite sheet
        
        public static Rectangle GetFrame(string direction, int frameIndex)
        {
            int row = Utils.GetRowForDirection(direction);
            return new Rectangle(ARMS_X_OFFSET + (frameIndex * FRAME_WIDTH), row * FRAME_HEIGHT, FRAME_WIDTH, FRAME_HEIGHT);
        }
    }

    // Special eating animations
    public static class Eating
    {
        private const int EATING_Y_OFFSET = 96; // Eating arms at y=96
        
        public static Rectangle GetFrame(int frameIndex)
        {
            int xOffset = frameIndex == 0 ? 48 : 64; // Frame 0 = x48, Frame 1 = x64
            return new Rectangle(xOffset, EATING_Y_OFFSET, FRAME_WIDTH, FRAME_HEIGHT);
        }
    }

    // Eye animations
    public static class Eyes
    {
        private const int EYES_X_OFFSET = 96; // Eyes start at x=96
        
        // Closed eyes (blinking)
        public static Rectangle GetBlinkFrame(string direction)
        {
            return direction == "down" 
                ? new Rectangle(EYES_X_OFFSET, 0, FRAME_WIDTH, SMALL_FRAME_HEIGHT)
                : new Rectangle(EYES_X_OFFSET, 48, FRAME_WIDTH, SMALL_FRAME_HEIGHT);
        }
        
        // Eye whites
        public static Rectangle GetEyeWhites(string direction)
        {
            return direction == "down"
                ? new Rectangle(EYES_X_OFFSET, 16, FRAME_WIDTH, SMALL_FRAME_HEIGHT)
                : new Rectangle(EYES_X_OFFSET, 64, FRAME_WIDTH, SMALL_FRAME_HEIGHT);
        }
        
        // Eye pupils (colored)
        public static Rectangle GetEyePupils(string direction)
        {
            return direction == "down"
                ? new Rectangle(EYES_X_OFFSET, 32, FRAME_WIDTH, SMALL_FRAME_HEIGHT)
                : new Rectangle(EYES_X_OFFSET, 80, FRAME_WIDTH, SMALL_FRAME_HEIGHT);
        }
    }

    // Helper methods for common operations
    public static class Utils
    {
        /// <summary>
        /// Gets the row index for a given direction
        /// </summary>
        public static int GetRowForDirection(string direction)
        {
            return direction switch
            {
                "down" => 0,
                "right" => 1,
                "up" => 2,
                "left" => 1, // Uses right row but gets flipped
                _ => 0
            };
        }

        /// <summary>
        /// Determines if a direction should be horizontally flipped
        /// </summary>
        public static bool ShouldFlip(string direction)
        {
            return direction == "left";
        }
    }
}