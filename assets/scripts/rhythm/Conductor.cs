using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Channels;

public partial class Conductor : Node
{
	// Variables

	private double bpm = 120.0f;
	private int beatsPerMeasure = 4;
	private Pitch key;

	private Timer beatTimer;
	private int beatsThisMeasure = 1;

	private MetronomePlayer clickTrack;
	[Export] public bool clickTrackEnabled = true;
	[Export] public int accentedBeat = 1;

	// dont go over 20 channels total

	[Export] public Phrase intro;
	[Export] public Phrase phrase;

	//private Queue<Phrase> loopQueue = new();
	private Phrase[] loopsToPlay;
	private int loopsLeft = 0;
	

	// Mono

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		clickTrack = GetNode<MetronomePlayer>("Metronome");

		beatTimer = new Timer();
		AddChild(beatTimer);

		//QueuePhrase(new Phrase(120, 4, Pitch.C));
		loopsToPlay = new Phrase[] {intro, phrase};

		Play();
		//QueuePhrase(intro, 1);
		//QueuePhrase(phrase, 1);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Methods

	private void Play()
	{
		
		GD.Print("seconds per beat: " + 60.0f / bpm);
		beatTimer.WaitTime = 60.0f / bpm;
		beatTimer.OneShot = true;
		beatTimer.Timeout += () => Beat();
		beatTimer.Start();

		beatsThisMeasure = 1;
	}

	/// <summary>
	/// Queues a phrase to be played a certain number of times.
	/// </summary>
	/// <param name="phrase"></param>
	/// <param name="times"></param>
	private void QueuePhrase(Phrase phrase, int times = 1)
	{
		loopsLeft = times;

		beatsPerMeasure = phrase.loop._GetBeatCount();
		bpm = phrase.loop._GetBpm();
		key = phrase.Key;

		beatTimer.WaitTime = 60.0f / bpm;

		//loopQueue.Enqueue(phrase);
		GD.Print("enqueue: " + phrase.loop.ResourcePath);
	}

	/// <summary>
	/// Plays the metronome sound.
	/// </summary>
	private void PlayClickTrack()
	{
		if(beatsThisMeasure == accentedBeat)
			clickTrack.PlayAccentedTick();
		else
			clickTrack.PlayTick(); // plays metronome sound
	}

	private void PlayPhraseOnNextBeat(Phrase phrase)
	{
		beatsPerMeasure = phrase.loop.BeatCount;
		bpm = phrase.loop.Bpm;
		key = phrase.Key;

		beatTimer.WaitTime = 60.0 / bpm;

		var channel1 = GetNode<AudioStreamPlayer>("Channel_1");
		//channel1.Stream = loopQueue.Peek().loop;
		if(channel1.Stream != loopsToPlay[1].loop)
			channel1.Stream = loopsToPlay[1].loop;
		//beatTimer.Timeout += () => {channel1.Play();};
	}

	/// <summary>
	/// Logic performed every beat. (Like Process() but for beats instead of frames.)
	/// </summary>
	private void Beat()
	{
		if (clickTrackEnabled)
		{
			PlayClickTrack();
		}
		
		GD.Print("beat " + beatsThisMeasure);

		// start of loop logic
		if(beatsThisMeasure == 1)
		{
			GD.Print("new bar");
			//beatTimer.Timeout -= () => GetNode<AudioStreamPlayer>("Channel_1").Play();
			GetNode<AudioStreamPlayer>("Channel_1").Play();
			
			
			

			// if(!channel1.Playing)
			// {
			// 	GD.Print("play the stream");
			// 	channel1.Finished += () => channel1.Play();;
			// 	//channel1.Play();
			// }
			// else
			// {
			// 	GD.Print("channel 1 is already playing");
			// }
		}
		
		//  stem loop logic
		if(beatsThisMeasure >= beatsPerMeasure)
		{
			
			beatsThisMeasure = 0;
			PlayPhraseOnNextBeat(loopsToPlay[1]);
			
			// if the loops are done
			// if(loopsLeft == 1)
			// {
			// 	if(loopQueue.Count > 0)
			// 	{
			// 		loopQueue.Dequeue();
			// 		GD.Print("dequeue and play next: " + loopQueue.Peek().loop.ResourcePath);
			// 		var channel1 = GetNode<AudioStreamPlayer>("Channel_1");
			// 		channel1.Stream = loopQueue.Peek().loop;
			// 		channel1.Play();
			// 		channel1.Finished += () => channel1.Play();
					

			// 		if(loopQueue.Count > 0)
			// 		{
			// 			//QueuePhrase(loopQueue.Peek(), 2);
			// 		}
			// 		else
			// 		{
			// 			GD.PrintErr("The Conductor loop queue is empty.");
			// 		}
			// 	}
			// 	else
			// 	{
			// 		GD.PrintErr("No loops are queued in the Conductor.");
			// 	}
			// }
			loopsLeft--;
		}

		beatsThisMeasure++;

		beatTimer.Start();
	}

}
