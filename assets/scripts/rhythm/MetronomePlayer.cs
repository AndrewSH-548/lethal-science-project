using Godot;
using System;

public partial class MetronomePlayer : AudioStreamPlayer
{
    [Export] public AudioStreamMP3 tickSound;
    [Export] public AudioStreamMP3 accentedTickSound;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

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
}