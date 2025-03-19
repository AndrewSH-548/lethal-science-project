using System;
using Godot;

public partial class Conductor : Node
{
	// Variables
	[Export] public string TrackName { get; private set; }
	public GodotObject notifier {get; private set;}

	

	private double bpm = 120.0f; // bpm for the Conductor - will adjust to the bpm of the current phrase
	private int beatsPerMeasure = 4; // same as above
	private Pitch key;

	private int wholeBeatsThisMeasure = 1;
	private int beatSubdivisions = 0; // for eighth notes and beyond

	private MetronomePlayer clickTrack;
	[Export] public bool ClickTrackEnabled {get; set;} = true;
	[Export] public int AccentedBeat {get; set;} = 1;

	[Export] public bool PrintToConsoleEnabled {get; set;} = false; // for debugging

	// rule of thumb - dont go over 20 channels total

	public bool IsPlaying {get {return rootChannel.Playing; }}
	public int BeatsPerMeasure { get { return beatsPerMeasure; } }

	private double time = 0f;
	private double _timeBegin;
	private double _timeDelay;
	private double _lastBeatTime;

	[Export] private AudioStreamPlayer rootChannel;
	AudioStreamInteractive interactiveStream;
	AudioStreamOggVorbis currentClip;
	AudioStreamPlaybackInteractive playback = null; 

	/// <summary>
	/// The rate at which the beat will play.
	/// ex. 1 = every beat, 2 = every half beat, 4 = every quarter note.
	/// Do not set this directly.
	/// </summary>
	[Export] public int BeatRate {get; set;} = 1;
	private int queuedBeatRateChange = 0;
	public event VoidEventHandler OnBeatRateChanged;

	public delegate void BeatEventHandler(float beat); // beats can be decimals (1/2 beat, 1/4 beat)
	public event BeatEventHandler OnBeat;

	public delegate void VoidEventHandler();
	public event VoidEventHandler OnFinalBeat;

	// Godot methods

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		var myGDScript = GD.Load<GDScript>("res://addons/rhythm_notifier/rhythm_notifier.gd");
		var myGDScriptNode = (GodotObject)myGDScript.New(); // This is a GodotObject. 
		myGDScriptNode.Connect(myGDScriptNode.Call("beats", 1.0f).ToString(), Callable.From(Beat));
		

		//notifier.Beats(1.0f);

		interactiveStream = rootChannel.Stream as AudioStreamInteractive;
		// get clips out of AudioStreamInteractive
		if(rootChannel.Playing) playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		if(playback != null) currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;

		OnBeat += PrintBeat;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(playback != null) currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;
		
		if(Input.IsActionJustPressed("P") && !rootChannel.Playing)
		{
			Play();
			playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		}

	}

	// Methods

	/// <summary>
	/// Always plays from the start of the song.
	/// </summary>
	public void Play()
	{
		if(!rootChannel.Playing)
			rootChannel.Play();

		playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;

	}

	/// <summary>
	/// Plays the metronome sound. Accented beat will sound different than the others.
	/// </summary>
	private void PlayClickTrack()
	{
		if(wholeBeatsThisMeasure == AccentedBeat)
			clickTrack.PlayAccentedTick();
		else
			clickTrack.PlayTick();
	}

	public void Beat()
	{
		GD.Print("something going on");
	}


	/// <summary>
	/// Debug method tied to the OnBeat event.
	/// </summary>
	/// <param name="beat"></param>
	private void PrintBeat(float beat)
	{
		if(!PrintToConsoleEnabled) return;
		
		GD.Print("beat: " + beat);
		GD.Print("whole beats: " + wholeBeatsThisMeasure);
	}

	public int GetCurrentClipIndex()
	{
		try { return playback.GetCurrentClipIndex(); }
		catch { return 0; }
	}
}
