using Godot;
using System;

public partial class MenuManager : Control
{
    public static MenuManager Instance { get; private set; }

    [Signal]
    public delegate void GamePauseToggleEventHandler();

    public delegate void GameInitializedEventHandler();
    public event GameInitializedEventHandler OnGameInitialized;

    private bool isPaused = false;
    public bool IsGameStarted { get; private set; } = false;
    private bool inOptions = false;

    public Difficulty Difficulty { get; set; } = Difficulty.Normal;
    
    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        Instance = this;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey inputEventKey && inputEventKey.Pressed && AllMenusClear())
        {
            if(inputEventKey.Keycode == Key.Escape)
            {
                TogglePause();
            }
        }
    }

    /// <summary>
    /// Called by GameManager when the game enters the scene tree. It will set up the data
    /// for any downstream menus that need data from the game (ex. EndScreen needs game data
    /// to know if the player won or lost).
    /// </summary>
    public void InitializeGameMenus()
    {
        OnGameInitialized?.Invoke();
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

    public void OnPlayPressed()
    {
        // instantiate the game scene
        PackedScene gameScene = GD.Load<PackedScene>("res://scenes/main_game.tscn");
        GameManager gameNode = gameScene.Instantiate() as GameManager;
        gameNode.Difficulty = Difficulty;
        GetParent().AddChild(gameNode);
        MainMenu.Instance.ToggleVisibility();
        Settings.Instance.ToggleDifficultySelect();
        IsGameStarted = true;
    }

    public void OnResumePressed()
    {
        TogglePause();
    }

    public void OnQuitPressed()
    {
        GetTree().Quit();
    }
   
    public void OnSettingsPressed()
    {
        CanvasLayer settings = GetNode<CanvasLayer>("SettingsMenu");
        inOptions = true;
        settings.Show();
    }

    public void OnBackPressed()
    {
        CanvasLayer settings = GetNode<CanvasLayer>("SettingsMenu");
        inOptions = false;
        settings.Hide();
    }

    public void ResetMenu()
    {
        Settings.Instance.ToggleDifficultySelect();
        MainMenu.Instance.ToggleVisibility();
        IsGameStarted = false;
    }

    private bool AllMenusClear()
    {
        return !GetNode<CanvasLayer>("SettingsMenu").Visible
            && !GetNode<CanvasLayer>("MainMenu").Visible
            && !GetNode<CanvasLayer>("PauseMenu").Visible
            && !GetNode<CanvasLayer>("TutorialMenu").Visible
            && !GetNode<CanvasLayer>("EndMenu").Visible;
    }
}
