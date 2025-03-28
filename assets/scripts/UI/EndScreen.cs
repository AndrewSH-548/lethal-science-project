using Godot;

public partial class EndScreen : CanvasLayer
{
    private Label title;
    private Label outcome;
    private GameManager gameManager;

    public override void _Ready()
    {
        MenuManager.Instance.OnGameInitialized += SetupGameEvents;
        Visible = false;
        title = GetNode<Label>("Background/Title");
        outcome = GetNode<Label>("Background/Outcome");
    }

    private void SetupGameEvents()
    {
        gameManager = GameManager.Instance;
        gameManager.GameWin += Win;
        gameManager.GameLose += Lose;
    }

    private void Lose()
    {
        Visible = true;
        outcome.Text = " You Got Obliterated\nGood Luck Next Time!";
    }

    private void Win()
    {
        Visible = true;
        title.Text = "You Win!";
        outcome.Text = "You beat our First Boss!";
    }
    
    public void OnResetPressed()
    {
        if(gameManager != null)
        {
            gameManager.ResetGame();
            Visible = false;
        }
    }
}
