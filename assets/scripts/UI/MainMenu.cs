using Godot;
using System;
using System.Diagnostics;

public partial class MainMenu : CanvasLayer
{
    public static MainMenu Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;
    }
    public void ToggleVisibility()
    {
        Visible = !Visible;
    }
}
