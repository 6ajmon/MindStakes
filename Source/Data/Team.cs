using Godot;
using System;

public partial class Team : Node
{
    public int Id { get; set; }
    public string TeamName { get; set; }    
    public Color Color { get; set; }
    public float Score { get; set; }
}
