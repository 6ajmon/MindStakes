using Godot;
using System;

public partial class QuestionScreen : Control
{
    [Export] public Label GameModeLabel { get; set; }
    [Export] public Label QuestionTextLabel { get; set; }
    [Export] public MarginContainer QuestionImage { get; set; }
    [Export] public GridContainer AnswersContainer { get; set; }
    [Export] public MarginContainer TeamCardContainer { get; set; }
    [Export] public PackedScene TeamCardScene { get; set; }
    public override void _Ready()
    {
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }
        UpdateGameModeLabel();
    }

    private void UpdateGameModeLabel()
    {
        var gameMode = GameManager.Instance.CurrentGameModeIndex;
        string modeText = gameMode switch
        {
            (int)GameManager.GameModes.Betting => "OBSTAWIANIE",
            (int)GameManager.GameModes.Sabotage => "SABOTAŻ",
            (int)GameManager.GameModes.Fraud => "OSZUST",
            _ => "Unknown Mode"
        };
        GameModeLabel.Text = $"Game Mode: {modeText}";
    }
}
