using Godot;

public partial class GameManager : Node2D
{
    [Export] public Player player;
    [Export] public Enemy enemy;

    [Signal]
    public delegate void GameWinEventHandler();

    [Signal]
    public delegate void GameLoseEventHandler();

    public override void _Process(double delta)
    {
        if(player.CurrentHealth<=0 )
        {
            EmitSignal(SignalName.GameLose);
        }
        if(enemy.CalmCurrent>=enemy.CalmMax)
        {
            EmitSignal(SignalName.GameWin);
        }
    }
}
