using Godot;
using System;

public partial class PauseManager : Node
{
    public static PauseManager Instance { get; private set; }

    [Signal]
    public delegate void GamePauseToggleEventHandler(bool isPaused,bool playHit);

    private bool isPaused = false;
    private bool playHit = false;
    private bool inOptions = false;
    private Texture2D resumeNormal = GD.Load<Texture2D>("res://assets/UI/resume.png");
    private Texture2D resumeHover= GD.Load<Texture2D>("res://assets/UI/resume_Hover.png");
    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        Instance = this;
       

    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey && inputEventKey.Pressed && !inOptions)
        {
            if (inputEventKey.Keycode != Key.Escape) return;
            if(playHit)
            {
                isPaused = !isPaused;
                EmitSignal(SignalName.GamePauseToggle, isPaused, playHit);
                GetTree().Paused = isPaused;
            }
           

        }
    }


public void OnResumePressed()
    {
       
        if(!playHit)
        {
            TextureButton resumeButton = GetNode<TextureButton>("PauseMenu/resume");
            resumeButton.TextureNormal = resumeNormal;
            resumeButton.TextureHover = resumeHover;
            PackedScene gameScene = GD.Load<PackedScene>("res://scenes/main_game.tscn");
            Node gameNode = gameScene.Instantiate();
            GetParent().AddChild(gameNode);
            playHit = true;  
        }
        else
        {
            isPaused = !isPaused;
            GetTree().Paused = isPaused;
        }
        EmitSignal(SignalName.GamePauseToggle, isPaused, playHit);

    }
    public void OnQuitPressed()
    {
        GetTree().Quit();
    }
   
    public void OnOptionsPressed()
    {
        CanvasLayer options = GetNode<CanvasLayer>("Options");
        inOptions = true;
        options.Show();
    }

    public void OnBackPressed()
    {
        CanvasLayer options = GetNode<CanvasLayer>("Options");
        inOptions = false;
        options.Hide();
    }
  
}
