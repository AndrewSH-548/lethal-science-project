using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

public partial class Conductor : Node
{
	// Variables

	private double bpm = 120.0f; // bpm for the Conductor - will adjust to the bpm of the current phrase
	private int beatsPerMeasure = 4; // same as above
	private Pitch key;

	private Timer beatTimer; // time between each beat
	private int beatsThisMeasure = 1;

	private MetronomePlayer clickTrack;
	[Export] public bool ClickTrackEnabled {get; set;} = true;
	[Export] public int AccentedBeat {get; set;} = 1;

	// rule of thumb - dont go over 20 channels total

	[Export] public Phrase intro;
	[Export] public Phrase phrase;

	private Phrase[] loopsToPlay; // variable for testing - in future PR this will be dynamic

	public delegate void BeatEventHandler(int beat);
	public event BeatEventHandler OnBeat;

	// Godot methods

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		beatTimer = new Timer();
		AddChild(beatTimer);

		loopsToPlay = new Phrase[] {intro, phrase};

		OnBeat += PrintBeat;

		Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("P"))
		{
			Pause();
		}
	}

	// Methods

	/// <summary>
	/// Starting off point for the Conductor, call this to make things happen.
	/// </summary>
	private void Play()
	{
		var secondsPerBeat = 60.0f / bpm;
		GD.Print("seconds per beat: " + secondsPerBeat);
		beatTimer.WaitTime = secondsPerBeat;
		beatTimer.OneShot = true; // do not loop automatically
		beatTimer.Timeout += () => Beat(); // play another beat AFTER the last beat
		beatTimer.Start();

		beatsThisMeasure = 1; // start on 1st beat
	}

	/// <summary>
	/// Stops the beat timer, but will play out any more stems until they are done with their loop.
	/// </summary>
	private void Pause()
	{
		GD.Print("pause conductor");
		beatTimer.Stop();
	}

	/// <summary>
	/// Plays the metronome sound. Accented beat will sound different than the others.
	/// </summary>
	private void PlayClickTrack()
	{
		if(beatsThisMeasure == AccentedBeat)
			clickTrack.PlayAccentedTick();
		else
			clickTrack.PlayTick();
	}


	private void SetNextMeasurePhrase(Phrase phrase)
	{
		SetConductorParameters(phrase);

		var channel1 = GetNode<AudioStreamPlayer>("Channel_1");

		// TODO - remove this in future to transition to other loops
		if(channel1.Stream != loopsToPlay[1].loop)
			channel1.Stream = loopsToPlay[1].loop;
	}

	/// <summary>
	/// Sets the conductor parameters to sync with the current phrase.
	/// </summary>
	/// <param name="phrase"></param>
	private void SetConductorParameters(Phrase phrase)
	{
		beatsPerMeasure = phrase.loop.BeatCount;
		bpm = phrase.loop.Bpm;
		key = phrase.Key;

		beatTimer.WaitTime = 60.0 / bpm;
	}

	/// <summary>
	/// Logic performed every beat. (Like Process() but for beats instead of frames.)
	/// </summary>
	private void Beat()
	{
		if (ClickTrackEnabled)
		{
			PlayClickTrack();
		}
		
		OnBeat?.Invoke(beatsThisMeasure);

		// start of loop logic
		if(beatsThisMeasure == 1)
		{
			GD.Print("new bar");
			GetNode<AudioStreamPlayer>("Channel_1").Play();
		}
		
		//  end of loop logic
		if(beatsThisMeasure >= beatsPerMeasure)
		{
			beatsThisMeasure = 0;
			SetNextMeasurePhrase(loopsToPlay[1]);
		}

		beatsThisMeasure++;

		beatTimer.Start();
	}

	/// <summary>
	/// Debug method tied to the OnBeat event.
	/// </summary>
	/// <param name="beat"></param>
	private void PrintBeat(int beat)
	{
		GD.Print("beat: " + beat);
	}

}
