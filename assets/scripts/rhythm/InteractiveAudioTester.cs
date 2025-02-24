using Godot;
using System;

public partial class InteractiveAudioTester : AudioStreamPlayer
{

	AudioStreamInteractive stream;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		stream = Stream as AudioStreamInteractive;

		Finished += () =>
		{
			GD.Print("yo its done");
			Play();
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
