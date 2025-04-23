using Godot;
using System;

public partial class AbsorbShield : Area2D
{
    ShaderMaterial cooldownShader;
    public AnimatedSprite2D Sprite { get; private set; }
    CollisionShape2D hitbox;
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
        Sprite = GetChild<AnimatedSprite2D>(0);
        cooldownShader = Sprite.Material as ShaderMaterial;
        hitbox = GetChild<CollisionShape2D>(1);
        Toggle(false);
        Sprite.AnimationFinished += () =>
        {
            Sprite.Animation = "default";
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

    public void Toggle(bool isEnabled)
    {
        //Reversed for intuition
        //True means enabled (!disabled) and false means disabled
        hitbox.Disabled = !isEnabled;
    }
}
