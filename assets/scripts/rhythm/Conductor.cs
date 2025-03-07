using System;
using Godot;

public partial class Conductor : Node
{
	// Variables

	private double bpm = 120.0f; // bpm for the Conductor - will adjust to the bpm of the current phrase
	private int beatsPerMeasure = 4; // same as above
	private Pitch key;

	private Timer beatTimer; // time between each beat
	private int wholeBeatsThisMeasure = 1;
	private int beatSubdivisions = 0; // for eighth notes and beyond

	private MetronomePlayer clickTrack;
	[Export] public bool ClickTrackEnabled {get; set;} = true;
	[Export] public int AccentedBeat {get; set;} = 1;

	[Export] public bool PrintToConsoleEnabled {get; set;} = false; // for debugging

	// rule of thumb - dont go over 20 channels total

	private bool pauseQueued = false;
	public bool IsPlaying {get; set;} = false;
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

		_timeBegin = Time.GetTicksUsec();
		_timeDelay = AudioServer.GetTimeSinceLastMix() + AudioServer.GetOutputLatency();
		_lastBeatTime = 0;

		interactiveStream = rootChannel.Stream as AudioStreamInteractive;
		if(rootChannel.Playing) playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		if(playback != null) currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;

		OnBeat += PrintBeat;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(playback != null) currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;

		time = (Time.GetTicksUsec() - _timeBegin) / 1000000.0d;
		time = Math.Max(0.0d, time - _timeDelay);
		
		GD.Print(time);
		ProcessBeats(time);

		// P will toggle the conductor
		if(Input.IsActionJustPressed("P") && rootChannel.Playing)
		{
			Pause();
		}
		else if(Input.IsActionJustPressed("P") && !rootChannel.Playing)
		{
			Play();
			playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		}

		// up and down will change the beat rate - applied at end of measure
		if(Input.IsActionJustPressed("addBeat"))
		{
			IncrementBeatRate();
		}
		else if (Input.IsActionJustPressed("subtractBeat"))
		{
			DecrementBeatRate();
		}

		// pause will take effect at end of measure
		if(pauseQueued && wholeBeatsThisMeasure == 1)
		{
			beatTimer.Stop();
			IsPlaying = false;
			pauseQueued = false;

			PlayFinalBeat();
		}

		//time += delta;
	}

	// Methods

	private void ProcessBeats(double _time)
	{
		var secondsPerBeat = 60.0 / bpm;

		if(_time >= secondsPerBeat + _lastBeatTime)
		{
			clickTrack.PlayAccentedTick();
			_lastBeatTime = _time;
		}
	}

	/// <summary>
	/// Always plays from the start of the song.
	/// </summary>
	public void Play()
	{
		if(beatTimer == null)
		{
			beatTimer = new Timer();
			AddChild(beatTimer);
			beatTimer.Timeout += () => Beat(); // play another beat AFTER the last beat
		}
		beatTimer.Stop();

		if(!rootChannel.Playing)
			rootChannel.Play();

		playback = rootChannel.GetStreamPlayback() as AudioStreamPlaybackInteractive;
		currentClip = interactiveStream.GetClipStream(playback.GetCurrentClipIndex()) as AudioStreamOggVorbis;

		SetConductorParameters(currentClip);
		UpdateBeatRate();
		Beat(); // start the first beat - this will play the next ones too
	}

	/// <summary>
	/// Adjusts the beat timer to the new bpm. Also takes into account the BeatRate.
	/// BeatRate can split whole notes into smaller subdivisions.
	/// </summary>
	private void UpdateBeatRate()
	{
		var secondsPerBeatEvent = (60.0 / currentClip.Bpm) * (1.0/BeatRate);
		if(PrintToConsoleEnabled) GD.Print("seconds per beat event: " + secondsPerBeatEvent);
		beatsPerMeasure = currentClip.BarBeats;
		beatTimer.WaitTime = secondsPerBeatEvent;
		beatTimer.OneShot = true; // do not loop automatically
		wholeBeatsThisMeasure = 1; // start on 1st beat
		beatSubdivisions = 0;

		OnBeatRateChanged?.Invoke();
	}

	/// <summary>
	/// Stops the beat timer, but will finish playing phrase until the loop is done.
	/// </summary>
	public void Pause()
	{
		rootChannel.Stop();
		playback = null;
		pauseQueued = true;
	}

	/// <summary>
	/// Public facing functions to queue up a new beat rate.
	/// </summary>
	public void IncrementBeatRate()
	{
		queuedBeatRateChange += 1;
	}

	public void DecrementBeatRate()
	{
		if(BeatRate - queuedBeatRateChange > 1)
		{
			queuedBeatRateChange -= 1;
		}
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

	/// <summary>
	/// Plays one more beat after the conductor is finished.
	/// This is the "first" beat of a measure that doesn't exist,
	/// but it serves a good purpose for UI and logic cleanup.
	/// </summary>
	private void PlayFinalBeat()
	{
		Timer finalBeatTimer = new Timer();
		AddChild(finalBeatTimer);
		finalBeatTimer.WaitTime = 60.0 / bpm;

		finalBeatTimer.OneShot = true;
		finalBeatTimer.Timeout += () => {
			OnFinalBeat?.Invoke();
			finalBeatTimer.QueueFree();
		};
		finalBeatTimer.Start();
	}

	/// <summary>
	/// Sets the conductor parameters to sync with the current phrase.
	/// </summary>
	/// <param name="phrase"></param>
	private void SetConductorParameters(AudioStreamOggVorbis stream)
	{
		beatsPerMeasure = stream.BarBeats;
		bpm = stream.Bpm;
		//key = phrase.Key;
	}

	private void ResetBeatTimer()
	{
		double timeDelta = rootChannel.GetPlaybackPosition() + AudioServer.GetTimeSinceLastMix();
    	// Compensate for output latency.
    	timeDelta -= AudioServer.GetOutputLatency();

		GD.Print("time: " + time);
		clickTrack.PlayAccentedTick();

		beatTimer.Stop();
		beatTimer.WaitTime = (60.0 / currentClip.Bpm) - (time/currentClip.BarBeats);
		beatTimer.Start();
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
		
		float beat = wholeBeatsThisMeasure + ((float)beatSubdivisions / BeatRate);
		OnBeat?.Invoke(beat);

		// start of new measure logic
		if(beat == 1)
		{
			//Play();
			SetConductorParameters(currentClip);
			ResetBeatTimer();

			if(queuedBeatRateChange != 0)
			{
				BeatRate += queuedBeatRateChange;
				queuedBeatRateChange = 0;
				UpdateBeatRate();
			}

		}

		//  end of measure logic
		if(wholeBeatsThisMeasure == beatsPerMeasure && beatSubdivisions == BeatRate - 1)
		{
			wholeBeatsThisMeasure = 1;
			beatSubdivisions = 0;

		}
		// end of beat logic
		else
		{
			beatSubdivisions += 1;

			if(beatSubdivisions / BeatRate >= 1)
			{
				beatSubdivisions = 0;
				wholeBeatsThisMeasure += 1;
			}
		}

		beatTimer.Start();
	}

	/// <summary>
	/// Sets a phrase to be played at the start of the next measure.
	/// </summary>
	/// <param name="queuedPhrase"></param>
	// private void QueuePhrase(Phrase queuedPhrase)
	// {
	// 	phraseQueued = true;
	// 	nextPhrase = queuedPhrase;
	// }

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
}
