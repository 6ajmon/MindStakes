using Godot;
using System;

public partial class TeamCreateScreen : Control
{
    public override void _Ready()
    {
        SceneManager.Instance.RegisterFirstScene();
    }
}
