using Godot;
using System;

public partial class DeathIrisWipe : ColorRect
{
    public float Radius {get; set;}
    public Vector2 Center {get; set;}

    [Export] CharacterBody2D player;

    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        var mousePos = GetViewport().GetMousePosition();
        var viewportRectSize = GetViewportRect().Size;
        var playerPosToScreenCoord = player.Position / viewportRectSize;

        if(Input.IsActionJustPressed("P"))
        {
            ZoomIn(playerPosToScreenCoord, 1f);
        }

        //(Material as ShaderMaterial).SetShaderParameter("center", playerPosToScreenCoord);
    }

    public void ZoomIn(Vector2 spot, float duration)
    {
        ShaderMaterial shader = (Material as ShaderMaterial);
        shader.SetShaderParameter("enabled", true);

        Center = spot;
        Radius = .25f;
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(this, "material:shader_parameter/radius", .25f, duration);
        //tween.TweenCallback();
    }
}
