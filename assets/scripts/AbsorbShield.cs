using Godot;
using System;

public partial class AbsorbShield : AnimatedSprite2D
{
    ShaderMaterial cooldownShader;
    double cooldownProgress = 1;
    public double CooldownProgress
    {
        get { return cooldownProgress; }
        set { 
            if (cooldownProgress < 0) cooldownProgress = 0;
            else if (cooldownProgress > 1) cooldownProgress = 1;
            else cooldownProgress = value; 
        }
    }

    public override void _Ready()
    {
        cooldownShader = Material as ShaderMaterial;
        AnimationFinished += () =>
        {
            Animation = "default";
        };
    }

    public override void _Process(double delta)
    {
        cooldownShader.SetShaderParameter("cooldown_progress", cooldownProgress);
    }

    public void StartReload()
    {
        cooldownProgress = 0;
    }
}
