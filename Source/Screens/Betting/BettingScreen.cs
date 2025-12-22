using Godot;
using System;

public partial class BettingScreen : Control
{
    
    [Export] public PackedScene BettingTeamCardScene;
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
        BettingTeamCard teamCard = BettingTeamCardScene.Instantiate<BettingTeamCard>();
        teamCard.TeamData = newTeam;
        TeamCardsContainer.AddChild(teamCard);
    }
}
