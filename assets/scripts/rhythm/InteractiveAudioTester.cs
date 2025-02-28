using Godot;
using System;

public partial class InteractiveAudioTester : AudioStreamPlayer
{

	[Export] AudioStreamPlayer player;
	[Export] public Label label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//stream = GetNode<AudioStreamInteractive>("AudioStreamPla");
		GD.Print("lets go");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		label.Text = player.Stream._GetStreamName();
	}
}
