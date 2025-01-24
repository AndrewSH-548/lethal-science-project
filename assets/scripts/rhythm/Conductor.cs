using Godot;
using System;

public partial class Conductor : Node
{
	// Variables

	private float bpm = 120.0f;
	private int beatsBerMeasure = 4;

	private Timer beatTimer;
	private int beatsThisMeasure = 0;

	private MetronomePlayer clickTrack;
	[Export] public bool clickTrackEnabled = true;

	// SoundStem resource with bpm and beats per measure

	// Mono

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		beatTimer = new Timer();
		GD.Print("seconds per beat: " + 60.0f / bpm);
		beatTimer.WaitTime = 60.0f / bpm;
		beatTimer.OneShot = true;
		beatTimer.Timeout += () => Beat();
		AddChild(beatTimer);
		beatTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Methods

	private void Beat()
	{
		if (clickTrackEnabled)
			clickTrack.PlayTick(); // plays metronome sound

		GD.Print("Beat");

		beatsThisMeasure++;
		if(beatsThisMeasure >= beatsBerMeasure)
		{
			beatsThisMeasure = 0;
			GD.Print("Measure");
			clickTrack.PlayAccentedTick();
		}

		beatTimer.Start();
	}

}
