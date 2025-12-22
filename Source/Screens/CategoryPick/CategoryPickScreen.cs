using Godot;
using System;

public partial class CategoryPickScreen : Control
{
    [Export] MarginContainer TeamCardContainer;
    [Export] PackedScene TeamCardScene;

    public override void _Ready()
    {
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
        }
    }
}
