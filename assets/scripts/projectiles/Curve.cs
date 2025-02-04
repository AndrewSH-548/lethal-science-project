using Godot;
using System;

public partial class Curve : Projectile
{
    // Either -1 or 1.
    readonly int rotationalDirection = GD.RandRange(0, 1) * 2 - 1;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // Curves
    public override void _Process(double delta)
    {

        direction = CurveMotion((float)delta * rotationalDirection);
        Position += direction;

        base._Process(delta);
    }
}
