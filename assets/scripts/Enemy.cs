using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] PackedScene projectileScene;
	[Export] Conductor conductor;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SpawnProjectile();

		conductor.OnBeat += (int i) => SpawnProjectile();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SpawnProjectile()
	{
		Projectile projectile = projectileScene.Instantiate() as Projectile;
		projectile.Orientation = GD.Randf() * 2 - 1;
		AddChild(projectile);
	}
}
