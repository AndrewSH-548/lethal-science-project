using Godot;
using System;

public partial class TogglePauseVisiablity : CanvasLayer
{
    [Export]public bool visableOnPause = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PauseManager.Instance.GamePauseToggle += ToggleVisibility;
		if (!visableOnPause) return;
		Hide();
	}
	public void ToggleVisibility(bool isPaused)
	{
		if(visableOnPause==isPaused)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

}
