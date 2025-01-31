using Godot;
using System;

[GlobalClass]
public partial class ProjectileEvent : Resource
{
	[Export] public int beat;

	[Export] public PackedScene projectileScene;

	[Export] public float orientation;

	public void SpawnProjectile(Node parent, Vector2 position)
	{
		Projectile projectile = projectileScene.Instantiate() as Projectile;
		projectile.Position = position;
		projectile.Orientation = orientation;
        parent.AddChild(projectile);
	}
}
