using Godot;
using System;

public partial class Break : Node2D
{
	public Color glowColor = Color.FromHtml("FFFFFF");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D breakGlow = GetChild<AnimatedSprite2D>(0);
		AnimatedSprite2D breakSprite = GetChild<AnimatedSprite2D>(1);
        breakGlow.Modulate = glowColor;
		breakSprite.Play();
		breakGlow.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void OnSpriteAnimFinished()
	{
		QueueFree();
	}
}
