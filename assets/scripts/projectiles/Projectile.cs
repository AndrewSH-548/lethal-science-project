using Godot;
using System;

public partial class Projectile : Area2D
{
    // Projectile's starting orientation in radians. 0 makes the projectile go straight down.
    [Export] public float Orientation { get; set; } = 0;
    [Export] protected int Speed = 6;
    protected Vector2 direction;

    // Called when the node enters the scene tree for the first time.
    // Sets the projectile's starting orientation
    public override void _Ready()
    {
        // Ensure negative values are compatible
        if (Orientation < 0) Orientation = Mathf.Pi * 2 + Orientation;

        // -sin as x and cos as y, because the origin is down.
        Rotation += Orientation;
        direction = new Vector2(-Mathf.Sin(Orientation) * Speed, Mathf.Cos(Orientation) * Speed);
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
