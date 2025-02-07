using Godot;

[GlobalClass]
public partial class LoopSong : Song
{
    [Export] public Phrase Intro {get; set;} = null;
    [Export] public Phrase Outro {get; set;} = null;
    [Export] public bool LoopForever {get; set;} = true;

}