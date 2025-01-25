using Godot;
using System;

[GlobalClass]
public partial class Phrase : Resource
{
    [Export] public AudioStreamMP3 loop;
    [Export] public int BPM {get; set; }
    [Export] public int Meter {get; set; } // how many beats
    [Export] public Pitch Key {get; set; }
}