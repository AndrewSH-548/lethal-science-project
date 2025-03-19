using Godot;
using System;
using System.Collections.Generic;

[Tool]
[Icon("icon.svg")]
public partial class RhythmNotifier2 : Node
{
	public class Rhythm
	{
		[Signal]
		public delegate void IntervalChangedEventHandler(int currentInterval);
		public event IntervalChangedEventHandler OnIntervalChanged;

		public bool Repeating { get; private set; }
		public double BeatCount { get; private set; }
		public double StartBeat { get; private set; }
		private int? lastFrameInterval;

		public Rhythm(bool repeating, double beatCount, double startBeat)
		{
			Repeating = repeating;
			BeatCount = beatCount;
			StartBeat = startBeat;
		}

		private const double TOO_LATE = 0.1f;

		public void EmitIfNeeded(double position, double secsPerBeat)
		{
			double intervalSecs = BeatCount * secsPerBeat;
			int currentInterval = (int)Math.Floor((position - StartBeat) / intervalSecs);
			double secsPastInterval = (position - StartBeat) % intervalSecs;
			bool validInterval = currentInterval > 0 && (Repeating || currentInterval == 1);
			bool tooLate = secsPastInterval >= TOO_LATE;

			if (!validInterval || tooLate)
			{
				lastFrameInterval = null;
			}
			else if (lastFrameInterval != currentInterval)
			{
				//EmitSignal(nameof(ChangeInterval), currentInterval);
				OnIntervalChanged?.Invoke(currentInterval);
				lastFrameInterval = currentInterval;
			}
		}

		public void ChangeInterval(int currentInterval)
		{
			OnIntervalChanged?.Invoke(currentInterval);
		}
	}

	[Signal]
	public delegate void BeatEventHandler(double currentBeat, bool repeating, double beatCount);
	public event BeatEventHandler OnBeat2;

	[Export]
	public double Bpm
	{
		get => bpm;
		set
		{
			if (value == 0) return;
			bpm = value;
			NotifyPropertyListChanged(); // updates the field in the inspector
		}
	}
	private double bpm = 60.0f;

	[Export]
	public double BeatLength
	{
		get => 60.0f / bpm;
		set
		{
			if (value == 0) return;
			bpm = 60.0f / value;
		}
	}

	[Export]
	public AudioStreamPlayer AudioStreamPlayer { get; set; }

	[Export]
	public bool Running
	{
		get => silentRunning || StreamIsPlaying();
		set
		{
			if (value == Running) return;
			if (StreamIsPlaying()) return;
			silentRunning = value;
			position = 0.0f;
		}
	}

	public int CurrentBeat => (int)Math.Floor(position / BeatLength);

	public double CurrentPosition
	{
		get => position;
		set
		{
			if (StreamIsPlaying())
			{
				AudioStreamPlayer.Seek((float)value);
			}
			else if (silentRunning)
			{
				position = value;
			}
		}
	}
	private double position = 0.0f;

	private double cachedOutputLatency;
	private ulong invalidateCachedOutputLatencyBy = 0;
	private bool silentRunning;
	private List<Rhythm> rhythms = new List<Rhythm>();

	public override void _Ready()
	{
		//Beats(1.0f).Connect(nameof(Beat), this, nameof(OnBeat));

		OnBeat2 += Beats;
		
		//OnBeat2 += Beats(1.0f);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (silentRunning && StreamIsPlaying())
		{
			silentRunning = false;
		}
		if (!Running) return;

		if (silentRunning)
		{
			position += delta;
		}
		else
		{
			position = AudioStreamPlayer.GetPlaybackPosition();
			position += AudioServer.GetTimeSinceLastMix() - CachedOutputLatency;
		}

		foreach (var rhythm in rhythms)
		{
			rhythm.EmitIfNeeded(position, BeatLength);
		}
	}

	public void Beats(double beatCount, bool repeating = true, double startBeat = 0.0f)
	{
		foreach (var rhythm in rhythms)
		{
			if (rhythm.BeatCount == beatCount && rhythm.Repeating == repeating && rhythm.StartBeat == startBeat)
			{
				rhythm.ChangeInterval(CurrentBeat);
			}
		}

		var newRhythm = new Rhythm(repeating, beatCount, startBeat);
		rhythms.Add(newRhythm);
		newRhythm.ChangeInterval(CurrentBeat);
	}

	private bool StreamIsPlaying()
	{
		return AudioStreamPlayer != null && AudioStreamPlayer.Playing;
	}

	private double CachedOutputLatency
	{
		get
		{
			if (Time.GetTicksMsec() >= invalidateCachedOutputLatencyBy)
			{
				cachedOutputLatency = AudioServer.GetOutputLatency();
				invalidateCachedOutputLatencyBy = Time.GetTicksMsec() + 1000;
			}
			return cachedOutputLatency;
		}
	}

	// private void OnBeat(int currentBeat)
	// {
	// 	//EmitSignal(nameof(Beat), currentBeat);
	// 	//BeatEventHandler.Invoke(currentBeat);
	// 	OnBeat2?.Invoke(currentBeat);
	// }
}