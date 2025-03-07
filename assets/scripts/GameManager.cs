using Godot;
using System;

public partial class GameManager : Node2D
{
    [Export] public PauseManager menu;
    public void OnDelete()
    {
        menu.Reset();
    }
}
