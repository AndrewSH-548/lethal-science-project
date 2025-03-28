using Godot;

public partial class TutorialMenu : CanvasLayer
{
    public override void _Ready()
    {
        Visible = false;
    }

    public void Open()
    {
        Visible = true;
    }

    public void Close()
    {
        Visible = false;
    }
}
