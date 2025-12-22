using Godot;
using System;

public partial class TeamCreateScreen : Control
{
    [Export] HBoxContainer TeamCardsContainer;
    [Export] PackedScene TeamCardScene;
    [Export] Button ReadyButton;
    int initialTeamCount = 4;
    public override void _Ready()
    {
        SceneManager.Instance.RegisterFirstScene();
        LoadTeamCards();
    }

    public override void _Process(double delta)
    {
        ReadyButton.Disabled = TeamsManager.Instance.Teams.Count < 2;
    }

    public void LoadTeamCards()
    {
        for (int i = 0; i < TeamsManager.Instance.Teams.Count; i++)
        {
            AddTeamCard(TeamsManager.Instance.Teams[i]);
        }
    }

    public void AddTeamCard(Team newTeam)
    {
        TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
        teamCard.TeamData = newTeam;
        TeamCardsContainer.AddChild(teamCard);
    }

    public Team CreateNewTeam()
    {
        Team newTeam = new Team
        {
            Id = TeamsManager.Instance.Teams.Count,
            TeamName = "Team " + (TeamsManager.Instance.Teams.Count + 1),
            Color = TeamsManager.Instance.UniqueColors[TeamsManager.Instance.Teams.Count % TeamsManager.Instance.UniqueColors.Count],
            Score = 0f
        };
        TeamsManager.Instance.Teams.Add(newTeam);
        return newTeam;
    }

    public void OnAddButtonPressed()
    {
        AddTeamCard(CreateNewTeam());
    }

    public void OnRemoveButtonPressed()
    {
        if (TeamCardsContainer.GetChildCount() <= 1) return;
        TeamsManager.Instance.Teams.RemoveAt(TeamsManager.Instance.Teams.Count - 1);
        TeamCardsContainer.GetChild<TeamCard>(TeamCardsContainer.GetChildCount() - 1).QueueFree();
    }

    public void OnReadyButtonPressed()
    {
        SignalManager.Instance.EmitSignal(nameof(SignalManager.TeamsCreated));
    }
}
