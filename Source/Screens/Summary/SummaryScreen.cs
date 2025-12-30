using Godot;
using System;

public partial class SummaryScreen : Control
{
    [Export] public PackedScene ScoreTeamCardScene;
    [Export] public HBoxContainer TeamCardsContainer;
    public override void _Ready()
    {
        LoadTeamCards();
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
        ScoreTeamCard teamCard = ScoreTeamCardScene.Instantiate<ScoreTeamCard>();
        teamCard.TeamData = newTeam;
        teamCard.shouldShowTeamTypeLabel = false;
        TeamCardsContainer.AddChild(teamCard);
        teamCard.UpdateButtonsAndText();
    }
}
