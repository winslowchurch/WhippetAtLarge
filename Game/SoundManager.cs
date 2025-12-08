using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

public static class SoundPlayer
{
    private static ContentManager content;

    public static void Initialize(ContentManager contentManager)
    {
        content = contentManager;
    }

    public static void Play(string soundName)
    {
        if (content == null)
            throw new Exception("SoundPlayer not initialized. Call SoundPlayer.Initialize() with ContentManager.");

        var sound = content.Load<SoundEffect>($"sounds/{soundName}");
        sound.Play();
    }

    public static void PlayRandomSound(string baseName, int variants)
    {
        int variant = Random.Shared.Next(1, variants + 1);
        Play($"{baseName}{variant}");
    }
}
