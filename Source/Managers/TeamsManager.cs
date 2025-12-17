using Godot;
using System;

public partial class TeamsManager : Node
{
    public static TeamsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<TeamsManager>("TeamsManager");
}
