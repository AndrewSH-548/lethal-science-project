using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] PackedScene projectileScene;
	[Export] Conductor conductor;
	[Export] Color projectileColor;
	[Export] int projectileSpeed;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		conductor.OnBeat += Beat;
	}

	/// <summary>
	/// Instead of _Process() let's use Beat() to run logic every beat.
	/// </summary>
	private void Beat(float beatIndex)
	{
		SpawnProjectile();
	}

	public void SpawnProjectile()
	{
		Projectile projectile = projectileScene.Instantiate() as Projectile;
		projectile.GlowColor = projectileColor;
		projectile.Speed = projectileSpeed;
		projectile.Orientation = GD.Randf() * 1.4f - 0.7f;
		AddChild(projectile);
	}
}
