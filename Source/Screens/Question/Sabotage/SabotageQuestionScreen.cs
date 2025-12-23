using Godot;
using System;

public partial class SabotageQuestionScreen : QuestionScreen
{
    
    [Export] public MarginContainer TeamCardContainer { get; set; }
    [Export] public PackedScene TeamCardScene { get; set; }
    [Export] public Button CycleTeamLeftButton { get; set; }
    [Export] public Button CycleTeamRightButton { get; set; }
    [Export] public SpinBox TeamIndexSpinBox { get; set; }
    public override void _Ready()
    {
        base._Ready();
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }
        else
        {
            GD.PrintErr("TeamCardContainer is null in SabotageQuestionScreen");
        }
        CycleTeamLeftButton.Pressed += OnCycleTeamLeftButtonPressed;
        CycleTeamRightButton.Pressed += OnCycleTeamRightButtonPressed;
        TeamIndexSpinBox.ValueChanged += OnTeamIndexSpinBoxValueChanged;
        UpdateTeamIndexSpinBox();
    }
    private void OnCycleTeamLeftButtonPressed()
    {
        TeamsManager.Instance.CycleToPreviousTeam();
        UpdateTeamCard();
        UpdateTeamIndexSpinBox();
    }
    private void OnCycleTeamRightButtonPressed()
    {
        TeamsManager.Instance.CycleToNextTeam();
        UpdateTeamCard();
        UpdateTeamIndexSpinBox();
    }
    private void OnTeamIndexSpinBoxValueChanged(double value)
    {
        TeamsManager.Instance.SetCurrentTeamByIndex((int)value);
        UpdateTeamCard();
    }
    private void UpdateTeamIndexSpinBox()
    {
        if (TeamIndexSpinBox != null)
        {
            TeamIndexSpinBox.MinValue = 0;
            TeamIndexSpinBox.MaxValue = TeamsManager.Instance.Teams.Count - 1;
            TeamIndexSpinBox.Value = TeamsManager.Instance.CurrentTeam.Id;
        }
    }
    private void UpdateTeamCard()
    {
        if (TeamCardContainer != null)
        {
            TeamCardContainer.GetChild(0).QueueFree();
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }
    }
}