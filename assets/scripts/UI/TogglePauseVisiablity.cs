using Godot;
using System;

public partial class TogglePauseVisiablity : CanvasLayer
{
    [Export]public bool visableOnPause = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PauseManager.Instance.GamePauseToggle += ToggleVisibility;
	}
<<<<<<< Updated upstream
	public void ToggleVisibility(bool isPaused,bool playHit)
	{
        if (!playHit)
=======
	public void ToggleVisibility(bool isPaused, bool playHit)
	{
		//makes sure that menu is visable when you start 
		if (!isPaused && !playHit)
>>>>>>> Stashed changes
		{
			Show();
			return;
		}
<<<<<<< Updated upstream

=======
		//will use this otherwise
>>>>>>> Stashed changes
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
