using Godot;
using System;

public partial class Settings : CanvasLayer
{
  
    public static Settings Instance { get; private set; }

    [Signal]
    public delegate void ColorChangedEventHandler(Color color);

    [Export]
    public Color projColor = new Color(1, 1, 0, 1);
    [Export] PackedScene prjScene;
    [Export] Node2D spawn;
    Projectile test;
    public override void _EnterTree()
    {
       Instance = this;
       CreateTestProjectile();

    }
    void OnVolumeSliderChanged(float value)
    {
        int masterBusIndex = AudioServer.GetBusIndex("Master");
        if (value <= -30) AudioServer.SetBusMute(masterBusIndex, true);
        if (value > -30 && AudioServer.IsBusMute(masterBusIndex)) AudioServer.SetBusMute(masterBusIndex, false);
        AudioServer.SetBusVolumeDb(masterBusIndex, value);
        GD.Print(AudioServer.GetBusVolumeDb(masterBusIndex));
    }

    public override void _Process(double delta)
    {
       test.GlowColor = projColor;
    }
    void OnDifficultySelected(Difficulty index)
    {
        MenuManager.Instance.Difficulty = index;
        GD.Print(MenuManager.Instance.Difficulty);
    }

    void RedSliderValueChanged(float value)
    {
        projColor.R= value;
        test.GlowColor = projColor;
        RemoveChild(test);
        CreateTestProjectile();
        GD.Print(test.GlowColor);
        EmitSignal(SignalName.ColorChanged, projColor);
    }
    void BlueSliderValueChanged(float value)
    {
        projColor.B= value;
        test.GlowColor= projColor;
        RemoveChild(test);
        CreateTestProjectile();
        EmitSignal(SignalName.ColorChanged, projColor);
    }
    void GreenSliderValueChanged(float value)
    {
        projColor.G = value;
        test.GlowColor = projColor;
        RemoveChild(test);
        CreateTestProjectile();
        EmitSignal(SignalName.ColorChanged, projColor);
    }


    void CreateTestProjectile()
    {
        test = prjScene.Instantiate() as Projectile;
        test.Position = spawn.Position;
        test.Orientation = (float)(Math.PI / 2);
        test.Speed = 0;
        test.GlowColor = projColor;
        //test.Scale = new Vector2(1.3f, 1.3f);
        AddChild(test);
    }
}
