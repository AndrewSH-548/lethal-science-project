using Godot;
using System;

public partial class Player : CharacterBody2D
{
	int maxHealth = 50;
	int currentHealth;
	float speed = 200.0f;

	AnimatedSprite2D sprites;
	

	//Absorption variables
	bool isAbsorbing;
	bool isOnCooldown;

	[Export] ProgressBar healthBar;
    [Export] Timer absorptionTimer;
	[Export] Timer cooldownTimer;

	public bool IsAbsorbing
	{
		get { return isAbsorbing; }
	}

	public override void _Ready()
	{
		currentHealth = maxHealth;
		healthBar.MaxValue = maxHealth;
		UpdateHealthBar();

		sprites = GetChild<AnimatedSprite2D>(0);
		absorptionTimer.Timeout += () =>
		{
			isAbsorbing = false;
			Modulate = Color.FromHtml("FF0000");
			isOnCooldown = true;
			cooldownTimer.Start();
		};
		cooldownTimer.Timeout += () =>
		{
			Modulate = Color.FromHtml("FFFFFF");
			isOnCooldown = false;
		};
		sprites.Play();
	}
	
	public override void _PhysicsProcess(double delta)
	{

		// Get the input direction.
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			Velocity = new Vector2(direction.X * speed, direction.Y * speed);
			Animate(direction);
		}
		else Velocity = Vector2.Zero;

		if (Input.IsActionJustPressed("absorb") && !isOnCooldown)
		{
			isAbsorbing = true;
            absorptionTimer.Start();
            Modulate = Color.FromHtml("FFFF00");
        }
		MoveAndSlide();
	}

	public void Damage(int projectileDamage)
	{
		currentHealth -= projectileDamage;
		UpdateHealthBar();
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

	private void UpdateHealthBar()
	{
		healthBar.Value = currentHealth;
		if (healthBar.Value < 0)
		{
			healthBar.Value = healthBar.MaxValue;
		}
	}
}
