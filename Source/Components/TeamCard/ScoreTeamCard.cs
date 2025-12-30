using Godot;
using System;

public partial class ScoreTeamCard : TeamCard
{
    [Export] public Button RemovePointsButton { get; set; }
    [Export] public SpinBox PointsSpinBox { get; set; }
    [Export] public Button AddPointsButton { get; set; }
    [Export] public Label TeamTypeLabel { get; set; }
    private int betPoints = 0;
    public override void _Ready()
    {
        base._Ready();
        RemovePointsButton.Pressed += OnRemovePointsButtonPressed;
        AddPointsButton.Pressed += OnAddPointsButtonPressed;
        PointsSpinBox.ValueChanged += OnPointsSpinBoxValueChanged;

        betPoints = TeamsManager.Instance.Teams[TeamData.Id].CurrentBet;
        UpdateButtonsAndText();
    }

    public void UpdateButtonsAndText()
    {
        bool isFraudMode = GameManager.Instance.CurrentGameModeIndex == (int)GameManager.GameModeEnum.Fraud;
        RemovePointsButton.Text = $"- {betPoints}"; 
        AddPointsButton.Text = $"+ {betPoints}";
        PointsSpinBox.Value = TeamData.Score;
        if (GameManager.Instance.CurrentGameModeIndex != (int)GameManager.GameModeEnum.Fraud)
        {
            TeamTypeLabel.Visible = false;
            return;
        }
        if (TeamData.Id == TeamsManager.Instance.CurrentTeam.Id && 
            TeamData.Id == TeamsManager.Instance.InitialTeam.Id)
        {
            TeamTypeLabel.Text = isFraudMode ? "Odpowiadający" : "Odpowiadający";
        }
        else if (TeamData.Id == TeamsManager.Instance.InitialTeam.Id)
        {
            TeamTypeLabel.Text = isFraudMode ? "Odpowiadający" : "Wyznaczający";
        }
        else if (TeamData.Id == TeamsManager.Instance.CurrentTeam.Id)
        {
            TeamTypeLabel.Text = isFraudMode ? "Sprawdzający" : "Odpowiadający";
        }
        else
        {
            TeamTypeLabel.Visible = false;
        }
    }

    private void OnRemovePointsButtonPressed()
    {
        TeamData.Score -= betPoints;
        UpdateButtonsAndText();
    }
    private void OnAddPointsButtonPressed()
    {
        TeamData.Score += betPoints;
        UpdateButtonsAndText();
    }

    private void OnPointsSpinBoxValueChanged(double value)
    {
        TeamData.Score = (int)value;
    }
}
