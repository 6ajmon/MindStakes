using Godot;
using System;
using System.Collections.Generic;

public partial class TeamsManager : Node
{
    public static TeamsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<TeamsManager>("TeamsManager");
    public List<Team> Teams { get; set; } = new();
    public List<Color> UniqueColors { get; set; } = new()
    {
        Colors.Red,
        Colors.Blue,
        Colors.Green,
        Colors.Yellow,
        Colors.Purple,
        Colors.Orange,
        Colors.Cyan,
        Colors.Magenta,
        Colors.Lime,
        Colors.Pink,
        Colors.Teal,
        Colors.Brown
    };
    public Team CurrentTeam { get; set; }
    public int InitialTeamsCount { get; set; } = 4;
    public override void _Ready()
    {
        CreateTeams(InitialTeamsCount);
    }
    public void CreateTeams(int numberOfTeams)
    {
        for (int i = 0; i < numberOfTeams; i++)
        {
            CreateNewTeam();
        }
    }
    public Team CreateNewTeam()
    {
        Team newTeam = new Team
        {
            Id = Teams.Count,
            TeamName = "Team " + (Teams.Count + 1),
            Color = UniqueColors[Teams.Count % UniqueColors.Count],
            Score = 0f
        };
        Teams.Add(newTeam);
        return newTeam;
    }
}
