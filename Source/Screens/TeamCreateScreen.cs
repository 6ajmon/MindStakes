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

    public void OnAddButtonPressed()
    {
        AddTeamCard(TeamsManager.Instance.CreateNewTeam());
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
