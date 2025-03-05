using Godot;
using System;

public partial class Options : CanvasLayer
{
    void OnValueChanged(float value)
    {
        AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex("Master"),value);
        GD.Print(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master")));
    }
}
