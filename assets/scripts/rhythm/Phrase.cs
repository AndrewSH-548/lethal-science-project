using Godot;
using System;

[GlobalClass]
public partial class Phrase : Resource
{
    [Export] public AudioStreamMP3 loop;
    [Export] public Pitch Key {get; set; }
    [Export] public int Repetitions {get; set; } = 0;
    [Export] public int Beats {get; set;} = 4;
}