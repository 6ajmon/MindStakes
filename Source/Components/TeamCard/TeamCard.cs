using Godot;
using System;

public partial class TeamCard : Panel
{
    public Team TeamData { get; set; }
    public void OnTeamNameLabelTextSubmitted(string newText)
    {
        GD.Print($"Team name changed to: {newText}");
    }
}