using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 200.0f;

	AnimatedSprite2D sprites;

	public override void _Ready()
	{
		sprites = GetChild<AnimatedSprite2D>(0);
	}
	
	public override void _PhysicsProcess(double delta)
	{

		// Get the input direction.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			Velocity = new Vector2(direction.X * Speed, direction.Y * Speed);
			Animate(direction);
		}
		else Velocity = Vector2.Zero;
		
		MoveAndSlide();
	}

	private void Animate(Vector2 direction)
	{
		if (direction.Y < 0)
		{
			sprites.Animation = "up";
            sprites.FlipH = false;
        }
		else if (direction.Y > 0)
		{
			sprites.Animation = "down";
            sprites.FlipH = false;
        }
		else if (direction.X > 0)
		{
			sprites.Animation = "side";
			sprites.FlipH = false;
		}
		else if (direction.X < 0)
		{
			sprites.Animation = "side";
			sprites.FlipH = true;
		}
	}
}
