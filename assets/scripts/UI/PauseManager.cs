using Godot;
using System;

public partial class PauseManager : Node
{
    public static PauseManager Instance { get; private set; }

    [Signal]
    public delegate void GamePauseToggleEventHandler();

    private bool isPaused = false;
    private bool gamePlayedForFirstTime = true;
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
            if(inputEventKey.Keycode == Key.Escape)
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// Opens/closes the pause screen and tells the SceneTree that the game is paused/unpaused.
    /// </summary>
    private void TogglePause()
    {
        isPaused = !isPaused;
        EmitSignal(SignalName.GamePauseToggle);
        GetTree().Paused = isPaused;
    }

    public void OnResumePressed()
    {
        if(gamePlayedForFirstTime)
        {
            // change "Play" button to "Resume"
            TextureButton resumeButton = GetNode<TextureButton>("PauseMenu/resume");
            resumeButton.TextureNormal = resumeNormal;
            resumeButton.TextureHover = resumeHover;

            // instantiate the game scene
            PackedScene gameScene = GD.Load<PackedScene>("res://scenes/main_game.tscn");
            Node gameNode = gameScene.Instantiate();
            GetParent().AddChild(gameNode);

            GetNode<CanvasLayer>("PauseMenu").Hide();

            gamePlayedForFirstTime = false;
        }
        else
        {
            TogglePause();
        }
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
