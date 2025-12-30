using Godot;
using System;

public partial class ShaderedBackground : ColorRect
{
    public override void _Ready()
    {
        Visible = GameManager.Instance.IsShaderedBackgroundEnabled;
    }
}
