using System;
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
    bool pacifyCalled = false;
    
    [Export] public Difficulty Difficulty { get; set; } = Difficulty.Hard;

    public override void _EnterTree()
    {
        Instance = this;
        MenuManager.Instance.InitializeGameMenus();
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
        if (enemy.CalmCurrent>=enemy.CalmMax && !pacifyCalled)
        {
            conductor.Call("stop");
            enemy.End(() =>
            {
                EmitSignal(SignalName.GameWin);
            });
            pacifyCalled = true;
        }
    }

    public void ResetGame()
    {
        PackedScene gameScene = GD.Load<PackedScene>("res://scenes/main_game.tscn");
        GameManager gameNode = gameScene.Instantiate() as GameManager;
        gameNode.Difficulty = Difficulty;
        GetParent().AddChild(gameNode);

        QueueFree(); // remove current instance
    }

    public static Timer CreateTimer(Node parent, float waitTime, Action timeoutFunction)
    {
        Timer timer = new()
        {
            ProcessCallback = Timer.TimerProcessCallback.Physics,
            WaitTime = waitTime,
            OneShot = true
        };
        timer.Timeout += timeoutFunction;
        parent.AddChild(timer);
        return timer;
    }

    public static float GetAnimationLength(SpriteFrames spriteFrames, string animationName) {
        return spriteFrames.GetFrameCount(animationName) / (float)spriteFrames.GetAnimationSpeed(animationName);
    }
}
