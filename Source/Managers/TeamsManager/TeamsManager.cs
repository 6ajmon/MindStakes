using Godot;
using System;
using System.Collections.Generic;

public partial class TeamsManager : Node
{
    private const string TeamsSettingsPath = "user://teams.cfg";
    private const string TeamsSettingsSection = "teams";

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
    public Team InitialTeam { get; set; }
    public int InitialTeamsCount { get; set; } = 4;
    public override void _Ready()
    {
        if (!LoadSavedTeams())
        {
            CreateTeams(InitialTeamsCount);
        }
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
            TeamName = LocalizationManager.Format("TEAM_NAME", Teams.Count + 1),
            Color = UniqueColors[Teams.Count % UniqueColors.Count],
            Score = GameManager.Instance.StartingScore
        };
        Teams.Add(newTeam);
        return newTeam;
    }

    public void SaveTeams()
    {
        using var config = new ConfigFile();
        config.SetValue(TeamsSettingsSection, "count", Teams.Count);

        for (int i = 0; i < Teams.Count; i++)
        {
            Team team = Teams[i];
            string section = $"team_{i}";
            config.SetValue(section, "name", team.TeamName);
            config.SetValue(section, "color", team.Color.ToHtml(true));
            config.SetValue(section, "image_path", team.ImagePath);
        }

        Error error = config.Save(TeamsSettingsPath);
        if (error != Error.Ok)
        {
            GD.PushError($"Failed to save teams to '{TeamsSettingsPath}'. Error: {error}");
        }
    }

    private bool LoadSavedTeams()
    {
        using var config = new ConfigFile();
        Error error = config.Load(TeamsSettingsPath);
        if (error == Error.FileNotFound)
        {
            return false;
        }
        if (error != Error.Ok)
        {
            GD.PushError($"Failed to load teams from '{TeamsSettingsPath}'. Error: {error}");
            return false;
        }

        int teamCount = config.GetValue(TeamsSettingsSection, "count", 0).AsInt32();
        if (teamCount < 1)
        {
            GD.PushError($"Invalid team count in '{TeamsSettingsPath}'.");
            return false;
        }

        Teams.Clear();
        for (int i = 0; i < teamCount; i++)
        {
            string section = $"team_{i}";
            Team team = new Team
            {
                Id = i,
                TeamName = config.GetValue(section, "name", LocalizationManager.Format("TEAM_NAME", i + 1)).AsString(),
                Color = Color.FromHtml(config.GetValue(section, "color", UniqueColors[i % UniqueColors.Count].ToHtml(true)).AsString()),
                ImagePath = config.GetValue(section, "image_path", string.Empty).AsString(),
                Score = GameManager.Instance.StartingScore
            };
            Teams.Add(team);
        }

        return true;
    }
    public void NextInitialTeam()
    {
        if (Teams.Count == 0) return;
        int currentIndex = Teams.IndexOf(InitialTeam);
        int nextIndex = (currentIndex + 1) % Teams.Count;
        InitialTeam = Teams[nextIndex];
    }
    public void ResetBets()
    {
        foreach (var team in Teams)
        {
            team.CurrentBet = 0;
        }
    }
    public void CycleToNextTeam()
    {
        if (Teams.Count == 0) return;
        int currentIndex = Teams.IndexOf(CurrentTeam);
        int nextIndex = (currentIndex + 1) % Teams.Count;
        CurrentTeam = Teams[nextIndex];
    }
    public void CycleToPreviousTeam()
    {
        if (Teams.Count == 0) return;
        int currentIndex = Teams.IndexOf(CurrentTeam);
        int previousIndex = (currentIndex - 1 + Teams.Count) % Teams.Count;
        CurrentTeam = Teams[previousIndex];
    }
    public void SetCurrentTeamByIndex(int index)
    {
        if (index >= 0 && index < Teams.Count)
        {
            CurrentTeam = Teams[index];
        }
    }
}
