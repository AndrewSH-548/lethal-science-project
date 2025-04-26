using System;
using Godot;

public partial class Limb : Enemy
{
    public new bool IsFullyPacified
    {
        get { return GetParent<Enemy>().IsFullyPacified; }
    }
    public override void Pacify()
    {
        GetParent<Enemy>().Pacify();
    }
}
