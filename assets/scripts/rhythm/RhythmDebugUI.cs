using Godot;
using System;

public partial class RhythmDebugUI : Control
{
	[Export] Conductor conductor;

	Label beat1;
	Label beat2;
	Label beat3;
	Label beat4;

	private Label[] beatLabels;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		beat1 = GetNode<Label>("1");
		beat2 = GetNode<Label>("2");
		beat3 = GetNode<Label>("3");
		beat4 = GetNode<Label>("4");

		beatLabels = new Label[] { null, beat1, beat2, beat3, beat4 };

		conductor.OnBeat += Beat;
		conductor.OnFinalBeat += ResetColors;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void Beat(int beatIndex)
	{
		ColorOnBeat(beatIndex);
	}

	private void ColorOnBeat(int beatThatIsOn)
	{
		beatLabels[beatThatIsOn].SelfModulate = Colors.Red;

		foreach (Label label in beatLabels)
		{
			if (label != null && label != beatLabels[beatThatIsOn])
			{
				label.SelfModulate = Colors.White;
			}
		}
		
	}

	private void ResetColors()
	{
		if(!conductor.IsPlaying)
		{
			foreach (Label label in beatLabels)
			{
				if(label != null)
				{
					label.SelfModulate = Colors.White;
				}
			}
		}
	}

	/// <summary>
	/// These are functions to rig up with button signals.
	/// </summary>
	public void PressPlay()
	{
		conductor.Play();
	}

	public void PressPause()
	{
		conductor.Pause();
	}
}
