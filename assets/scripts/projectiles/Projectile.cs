using Godot;
using System;

public partial class Projectile : Area2D
{
    // Projectile's starting orientation in radians. 0 makes the projectile go straight down.
    [Export] public float Orientation { get; set; } = 0;
    [Export] public int Speed { get; set; } = 6;

    // If enabled, the projectile will always appear upright, and orientation will only affect its movement direction.
    [Export] bool lockOrientation;
    public Color GlowColor { get; set; } = Color.FromHtml("FFFFFF");
    protected Vector2 direction;

    // Called when the node enters the scene tree for the first time.
    // Sets the projectile's starting orientation
    public override void _Ready()
    {
        // Ensure negative values are compatible
        if (Orientation < 0) Orientation = Mathf.Pi * 2 + Orientation;

        direction = CurveMotion(Orientation);

        // Color the glow
        GetChild<Sprite2D>(1).Modulate = GlowColor;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // Simply moves the projectile in its given direction. May be altered by inherited classes.
    public override void _Process(double delta)
    {
        Position += direction;
        if (GlobalPosition.X > 600 || GlobalPosition.X < -50 || GlobalPosition.Y > 600 || GlobalPosition.Y < -50) QueueFree();
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

    protected Vector2 CurveMotion(float rotation)
    {
        Rotate(rotation);
        if (lockOrientation)
        {
            GetChild<Sprite2D>(1).GlobalRotation = 0;
            GetChild<Sprite2D>(2).GlobalRotation = 0;
        }
        return new Vector2(-Mathf.Sin(Rotation) * Speed, Mathf.Cos(Rotation) * Speed);
    }
}
