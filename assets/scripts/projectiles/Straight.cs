using Godot;
using System;

public partial class Straight : Area2D
{
	// Projectile's starting orientation in radians. 0 makes the projectile go straight down.
	[Export]
	public float Orientation { get; set; } = 0;
	Vector2 direction;
	int speed = 6;

	// Called when the node enters the scene tree for the first time.
	// Sets the projectile's starting orientation
	public override void _Ready()
	{
		// Ensure negative values are compatible
		if (Orientation < 0) Orientation = Mathf.Pi * 2 + Orientation;

		// -sin as x and cos as y, because the origin is down.
		Rotation += Orientation;
		direction = new Vector2(-Mathf.Sin(Orientation) * speed, Mathf.Cos(Orientation) * speed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// Simply moves the projectile in its given direction
	public override void _Process(double delta)
	{
		Position += direction;
	}

	public void OnBodyEntered(Player body)
	{
		if (body.IsAbsorbing)
		{
			body.Modulate = Color.FromHtml("00FF00");
		}
		//body.Damage();
		QueueFree();
	}
}
