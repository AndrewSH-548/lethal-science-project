using Godot;
using System;

public partial class MetronomePlayer : AudioStreamPlayer
{
    [Export] public AudioStreamMP3 tickSound;
    [Export] public AudioStreamMP3 accentedTickSound;

    public void PlayTick()
    {
        Stream = tickSound;
        Play();
    }

    public void PlayAccentedTick()
    {
        Stream = accentedTickSound;
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