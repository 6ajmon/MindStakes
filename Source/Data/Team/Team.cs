using Godot;
using System;

[GlobalClass]
public partial class Team : Resource
{
    [Export]
    public int Id { get; set; }
    [Export]
    public string TeamName { get; set; }    
    [Export]
    public Color Color { get; set; }
    [Export]
    public float Score { get; set; }
}
