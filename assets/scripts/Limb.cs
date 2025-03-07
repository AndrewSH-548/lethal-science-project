using System;
using Godot;

public partial class Limb : Enemy
{
    public override void Pacify()
    {
        GetParent<Enemy>().Pacify();
    }
}
