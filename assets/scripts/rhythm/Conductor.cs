using Godot;
using System;
using System.Collections.Generic;

public partial class Conductor : Node
{
	// Variables

	private float bpm = 120.0f;
	private int beatsBerMeasure = 4;
	private Pitch key;

	private Timer beatTimer;
	private int beatsThisMeasure = 0;

	private MetronomePlayer clickTrack;
	[Export] public bool clickTrackEnabled = true;

	// dont go over 20 channels total

	private Queue<Phrase> loopQueue = new();
	private int loopsLeft = 0;
	

	// Mono

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		//QueuePhrase(new Phrase(120, 4, Pitch.C));
		Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Methods

	private void Play()
	{
		beatTimer = new Timer();
		GD.Print("seconds per beat: " + 60.0f / bpm);
		beatTimer.WaitTime = 60.0f / bpm;
		beatTimer.OneShot = true;
		beatTimer.Timeout += () => Beat();
		AddChild(beatTimer);
		beatTimer.Start();
	}

	private void QueuePhrase(Phrase phrase, int times = 1)
	{
		loopsLeft = times;

		beatsBerMeasure = phrase.Meter;
		bpm = phrase.BPM;
		key = phrase.Key;
	}

	private void PlayClickTrack()
	{
		if(beatsThisMeasure == 0)
			clickTrack.PlayAccentedTick();
		else
			clickTrack.PlayTick(); // plays metronome sound
	}

	private void Beat()
	{
		if (clickTrackEnabled)
		{
			PlayClickTrack();
		}

		beatsThisMeasure++;
		if(beatsThisMeasure >= beatsBerMeasure)
		{
			beatsThisMeasure = 0;

			// logic for next loop
			if(loopsLeft > 0)
			{
				loopsLeft--;
				// play next loop
			}
			else
			{
				loopQueue.Dequeue();

				if(loopQueue.Count > 0)
				{
					QueuePhrase(loopQueue.Peek());
				}

			}
			
		}

		beatTimer.Start();
	}

}
