using Godot;
using System;
using System.Linq.Expressions;

public partial class Player : CharacterBody2D
{
	int maxHealth = 50;
	int currentHealth;
	float speed = 200.0f;

	AnimatedSprite2D sprites;
	
	//status variables
	bool isAbsorbing;
	bool isOnCooldown;
	bool isDamaged;

	[Export] ProgressBar healthBar;
    Timer absorptionTimer;
	Timer cooldownTimer;
	Timer damageBuffer;
	double damageTime;

	public bool IsAbsorbing
	{
		get { return isAbsorbing; }
	}
	public bool IsDamaged
	{
		get { return isDamaged; }
	}

	public override void _Ready()
	{
		currentHealth = maxHealth;
		healthBar.MaxValue = maxHealth;
		UpdateHealthBar();

		sprites = GetChild<AnimatedSprite2D>(0);
		absorptionTimer = CreateTimer(0.4f, () =>
		{
			isAbsorbing = false;
			Modulate = Color.FromHtml("FF0000");
			isOnCooldown = true;
			cooldownTimer.Start();
		});
		cooldownTimer = CreateTimer(1, () =>
		{
			Modulate = Color.FromHtml("FFFFFF");
			isOnCooldown = false;
		});
		damageBuffer = CreateTimer(1.5f, () =>
		{
			Modulate = Color.FromHtml("FFFFFF");
			isDamaged = false;
		});
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
		if (isDamaged)
		{
			damageTime += delta;
			if (damageTime > 0.02)
			{
                Modulate = Modulate == Color.FromHtml("777777") ? Color.FromHtml("BBBBBB") : Color.FromHtml("777777");
				damageTime = 0;
            }
		}
	}

	public void Damage(int projectileDamage)
	{
		currentHealth -= projectileDamage;
		isDamaged = true;
		damageBuffer.Start();
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

	private Timer CreateTimer(float waitTime, Action timeoutFunction)
	{
		Timer timer = new()
		{
			ProcessCallback = Timer.TimerProcessCallback.Physics,
			WaitTime = waitTime,
			OneShot = true
		};
		timer.Timeout += timeoutFunction;
		AddChild(timer);
		return timer;
	}
}
