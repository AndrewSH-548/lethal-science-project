using Godot;

public enum Difficulty
{
    Easy,
    Normal,
    Hard
}
public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; }

    [Export] public Player player;
    [Export] public Enemy enemy;
    [Export] public Node conductor;

    [Signal]
    public delegate void GameWinEventHandler();

    [Signal]
    public delegate void GameLoseEventHandler();

    bool deathCalled = false;
    
    public Difficulty Difficulty { get; set; }

    public override void _EnterTree()
    {
        Instance = this;
        MenuManager.Instance.InitalizeGameMenus();
    }

    public override void _Process(double delta)
    {
        if(player.CurrentHealth<=0 && !deathCalled)
        {
            conductor.Call("stop");
            player.Death(() =>
            {
                EmitSignal(SignalName.GameLose);
            });
            deathCalled = true;
        }
        if (enemy.CalmCurrent>=enemy.CalmMax)
        {
            EmitSignal(SignalName.GameWin);
        }
    }

    public void ResetGame()
    {
        PackedScene gameScene = GD.Load<PackedScene>("res://scenes/main_game.tscn");
        Node gameNode = gameScene.Instantiate();
        GetParent().AddChild(gameNode);

        QueueFree(); // remove current instance
    }
}
