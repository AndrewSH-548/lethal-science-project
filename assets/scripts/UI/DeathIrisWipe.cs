using Godot;
using System;

public partial class DeathIrisWipe : ColorRect
{
	public float Radius {get; set;}
	public Vector2 Center {get; set;}

	[Export] CharacterBody2D player;

	public override void _Ready()
	{
		ShaderMaterial shader = (Material as ShaderMaterial);
		shader.SetShaderParameter("enabled", false);
		shader.SetShaderParameter("radius", 1f);
		shader.SetShaderParameter("feather", 0.035f);
		shader.SetShaderParameter("screen_width", GetViewportRect().Size.X);
		shader.SetShaderParameter("screen_height", GetViewportRect().Size.Y);
		shader.SetShaderParameter("center", new Vector2(0.5f, 0.5f));
	}

	private void ZoomIn(Vector2 spot)
	{
		ShaderMaterial shader = (Material as ShaderMaterial);
		shader.SetShaderParameter("enabled", true);
		shader.SetShaderParameter("center", spot);

		Tween tween = GetTree().CreateTween();
		tween.TweenProperty(this, "material:shader_parameter/radius", .25f, 2); // zoom in
		tween.TweenProperty(this, "material:shader_parameter/radius", .25f, 1); // hold
		tween.TweenProperty(this, "material:shader_parameter/radius", 0f, 2f); // zoom in again
		tween.TweenProperty(this, "material:shader_parameter/feather", 0f, 0.5f); // remove the softened edge
	}

	public void TriggerIrisWipe()
	{
		var viewportRectSize = GetViewportRect().Size;
		var playerPosToScreenCoord = player.Position / viewportRectSize;
		ZoomIn(playerPosToScreenCoord);
	}
}
