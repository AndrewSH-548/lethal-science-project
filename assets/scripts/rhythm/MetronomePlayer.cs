using Godot;
using System;

public partial class MetronomePlayer : AudioStreamPlayer
{
    [Export] public AudioStreamMP3 tickSound;

    public void PlayTick()
    {
        PitchScale = 1;
        Play();
    }

    public void PlayAccentedTick()
    {
        PitchScale = 1.5f;
        Play();
    }

    public void PlayTick(int tick)
    {
        if (tick % 4 == 0)
        {
            PlayAccentedTick();
        }
        else
        {
            PlayTick();
        }
    }
}
