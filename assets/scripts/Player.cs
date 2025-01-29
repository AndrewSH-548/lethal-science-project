using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 200.0f;

	AnimatedSprite2D sprites;

	//Absorption variables
	bool isAbsorbing;
	const float absorptionLength = 0.4f;
	float absorptionTimer = absorptionLength;
	bool isOnCooldown;
	const float absorptionCooldown = 1f;
	float cooldownTimer = absorptionCooldown;

	public bool IsAbsorbing
	{
		get { return isAbsorbing; }
	}

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

		if (Input.IsActionJustPressed("absorb") && !isOnCooldown)
		{
			isAbsorbing = true;
            Modulate = Color.FromHtml("FFFF00");
        }

		if (isAbsorbing) {
			
			absorptionTimer -= (float)delta;
			if (absorptionTimer < 0)
			{
				isOnCooldown = true;
				isAbsorbing = false;
				absorptionTimer = absorptionLength;
                Modulate = Color.FromHtml("FF0000");
            }
		}

		if (isOnCooldown)
		{
			cooldownTimer -= (float)delta;
			if (cooldownTimer < 0)
			{
				Modulate = Color.FromHtml("FFFFFF");
				isOnCooldown = false;
				cooldownTimer = absorptionCooldown;
			}
		}
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
