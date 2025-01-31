using Godot;
using System;

public partial class Straight : Projectile
{
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // Simply moves the projectile in its given direction
    public override void _Process(double delta)
	{
		Position += direction;
	}
}
