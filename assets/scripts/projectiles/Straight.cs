using Godot;
using System;

public partial class Straight : CharacterBody2D
{
	// Projectile's starting orientation in radians. 0 makes the projectile go straight down.
	[Export]
	public float Orientation { get; set; } = 0;
	Vector2 direction;

	// Called when the node enters the scene tree for the first time.
	// Sets the projectile's starting orientation
	public override void _Ready()
	{
		// Ensure negative values are compatible
		if (Orientation < 0) Orientation = Mathf.Pi * 2 + Orientation;

		// -sin as x and cos as y, because the origin is down.
		Rotation += Orientation;
		direction = new Vector2(-Mathf.Sin(Orientation) * 300, Mathf.Cos(Orientation) * 300);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// Simply moves the projectile in its given direction
	public override void _Process(double delta)
	{
		Velocity = direction;
		MoveAndSlide();
	}
}
