using Godot;
using System;

public partial class Settings : CanvasLayer
{
  
    public static Settings Instance { get; private set; }

    [Signal]
    public delegate void ColorChangedEventHandler(Color color);

    [Export]
    public Color projectileColor = new Color(1, 1, 0, 1);
    Projectile test;
    public override void _EnterTree()
    {
        Instance = this;
        test = GetChild<Projectile>(1);
        test.ChangeGlowColor(projectileColor);
    }
    void OnVolumeSliderChanged(float value)
    {
        int masterBusIndex = AudioServer.GetBusIndex("Master");
        if (value <= -30) AudioServer.SetBusMute(masterBusIndex, true);
        if (value > -30 && AudioServer.IsBusMute(masterBusIndex)) AudioServer.SetBusMute(masterBusIndex, false);
        AudioServer.SetBusVolumeDb(masterBusIndex, value);
        GD.Print(AudioServer.GetBusVolumeDb(masterBusIndex));
    }

    void OnDifficultySelected(Difficulty index)
    {
        MenuManager.Instance.Difficulty = index;
        GD.Print(MenuManager.Instance.Difficulty);
    }

    void RedSliderValueChanged(float value)
    {
        projectileColor.R= value;
        test.ChangeGlowColor(projectileColor);
        EmitSignal(SignalName.ColorChanged, projectileColor);
    }
    void BlueSliderValueChanged(float value)
    {
        projectileColor.B= value;
        test.ChangeGlowColor(projectileColor);
        EmitSignal(SignalName.ColorChanged, projectileColor);
    }
    void GreenSliderValueChanged(float value)
    {
        projectileColor.G = value;
        test.ChangeGlowColor(projectileColor);
        EmitSignal(SignalName.ColorChanged, projectileColor);
    }
}
