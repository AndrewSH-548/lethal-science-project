using Godot;
using System;

public partial class Settings : CanvasLayer
{
    [Export]
    public Color projColor = new Color(0, 0, 0, 1);
    [Export]
    Sprite2D test;
    public static Settings Instance { get; private set; }

    [Signal]
    public delegate void ColorChangedEventHandler(Color color);
    public override void _EnterTree()
    {
       Instance = this;
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
        projColor.R= value;
        test.SelfModulate = projColor;
        GD.Print(test.SelfModulate);
        EmitSignal(SignalName.ColorChanged, projColor);
    }
    void BlueSliderValueChanged(float value)
    {
        projColor.B= value;
        test.SelfModulate= projColor;
        GD.Print(test.SelfModulate);
        EmitSignal(SignalName.ColorChanged, projColor);
    }
    void GreenSliderValueChanged(float value)
    {
        projColor.G = value;
        test.SelfModulate = projColor;
        GD.Print(test.SelfModulate);
        EmitSignal(SignalName.ColorChanged, projColor);
    }
}
