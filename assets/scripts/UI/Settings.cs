using Godot;
using System;

public partial class Settings : CanvasLayer
{
    void OnVolumeSliderChanged(float value)
    {
        int masterBusIndex = AudioServer.GetBusIndex("Master");
        if (value <= -30) AudioServer.SetBusMute(masterBusIndex, true);
        if (value > -30 && AudioServer.IsBusMute(masterBusIndex)) AudioServer.SetBusMute(masterBusIndex, false);
        AudioServer.SetBusVolumeDb(masterBusIndex, value);
        GD.Print(AudioServer.GetBusVolumeDb(masterBusIndex));
    }
}
