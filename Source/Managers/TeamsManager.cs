using Godot;
using System;
using System.Collections.Generic;

public partial class TeamsManager : Node
{
    public static TeamsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<TeamsManager>("TeamsManager");
    public List<Team> Teams { get; set; } = new();
    
}
