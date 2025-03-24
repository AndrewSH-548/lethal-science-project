using Godot;
using System;

public partial class EndScreen : Control
{
    private Label outcome;
    [Export] GameManager gameManager;
    public override void _Ready()
    {
        gameManager.GameWin += Win;
        gameManager.GameLose += Lose;
        Visible = false;

    }
    private void Lose()
    {
        Visible = true;
        outcome.Text = " You Got Obliterated Good Luck Next Time!";
    }
    private void Win()
    {
       Visible = true;
        outcome.Text = "Congratulations you beat our First Boss!";
    }
    
    public void OnResetPressed()
    {
        GetTree().ReloadCurrentScene();
    }

}
