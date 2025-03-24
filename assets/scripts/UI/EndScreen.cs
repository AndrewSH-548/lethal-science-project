using Godot;
using System;

public partial class EndScreen : Control
{
    private Label title;
    private Label outcome;
    [Export] GameManager gameManager;
    public override void _Ready()
    {
        gameManager.GameWin += Win;
        gameManager.GameLose += Lose;
        Visible = false;
        title = GetChild<Panel>(0).GetChild<Label>(1);
        outcome = GetChild<Panel>(0).GetChild<Label>(2);

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
        GetTree().ReloadCurrentScene();
    }

}
