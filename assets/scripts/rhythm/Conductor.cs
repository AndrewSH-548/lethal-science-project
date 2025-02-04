using Godot;

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

	private Phrase currentPhrase;

	private bool pauseQueued = false;
	public bool IsPlaying {get; set;} = false;

	[Export] private AudioStreamPlayer rootChannel;

	public delegate void BeatEventHandler(int beat);
	public event BeatEventHandler OnBeat;

	public delegate void VoidEventHandler();
	public event VoidEventHandler OnFinalBeat;

	// Godot methods

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		

		currentPhrase = phrase;

		OnBeat += PrintBeat;

		//Play();
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("P") && IsPlaying)
		{
			Pause();
		}
		else if(Input.IsActionJustPressed("P") && !IsPlaying)
		{
			Play();
		}

		if(pauseQueued && beatsThisMeasure == 1)
		{
			beatTimer.Stop();
			IsPlaying = false;
			pauseQueued = false;

			GD.Print("pause conductor");
			PlayFinalBeat();
			
		}
	}

	// Methods

	/// <summary>
	/// Starting off point for the Conductor, call this to make things happen.
	/// </summary>
	public void Play()
	{
		//SetConductorParameters(currentPhrase);
		if(beatTimer == null)
		{
			beatTimer = new Timer();
			AddChild(beatTimer);
			beatTimer.Timeout += () => Beat(); // play another beat AFTER the last beat
		}

		var secondsPerBeat = 60.0f / bpm;
		GD.Print("seconds per beat: " + secondsPerBeat);
		beatTimer.WaitTime = secondsPerBeat;
		beatTimer.OneShot = true; // do not loop automatically
		//beatTimer.Start();
		beatsThisMeasure = 1; // start on 1st beat
		Beat(); // start the first beat - this will play the next ones too


		rootChannel.Stream = currentPhrase.loop;

		IsPlaying = true;
	}

	/// <summary>
	/// Stops the beat timer, but will play out any more stems until they are done with their loop.
	/// </summary>
	public void Pause()
	{
		pauseQueued = true;
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

	/// <summary>
	/// Plays one more beat after the conductor is finished.
	/// This is the "first" beat of a measure that doesn't exist,
	/// but it serves a good purpose for UI and logic cleanup.
	/// </summary>
	private void PlayFinalBeat()
	{
		// make this final beat timer for cleanup purposes
		Timer finalBeatTimer = new Timer();
		AddChild(finalBeatTimer);
		finalBeatTimer.WaitTime = 60.0 / bpm;

		finalBeatTimer.OneShot = true;
		finalBeatTimer.Timeout += () => {
			GD.Print("final beat hit");
			OnFinalBeat?.Invoke();
			finalBeatTimer.QueueFree();
		};
		finalBeatTimer.Start();
	}


	private void SetNextMeasurePhrase(Phrase phrase)
	{
		SetConductorParameters(phrase);

		// TODO - remove this in future to transition to other loops
		if(rootChannel.Stream != currentPhrase.loop)
		{
			GD.Print("play a loop");
			rootChannel.Stream = currentPhrase.loop;
		}
		else
		{
			GD.Print("keep the looop");
		}
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
			//GetNode<AudioStreamPlayer>("Channel_1").Play();
		}
		
		//  end of loop logic
		if(beatsThisMeasure >= beatsPerMeasure)
		{
			beatsThisMeasure = 0;
			//SetNextMeasurePhrase(currentPhrase);
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
