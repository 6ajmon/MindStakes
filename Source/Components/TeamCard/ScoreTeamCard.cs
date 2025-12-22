using Godot;
using System;

public partial class ScoreTeamCard : TeamCard
{
    [Export] public Button RemovePointsButton { get; set; }
    [Export] public SpinBox PointsSpinBox { get; set; }
    [Export] public Button AddPointsButton { get; set; }
    private int betPoints = 0;
    public override void _Ready()
    {
        base._Ready();
        RemovePointsButton.Pressed += OnRemovePointsButtonPressed;
        AddPointsButton.Pressed += OnAddPointsButtonPressed;
        PointsSpinBox.ValueChanged += OnPointsSpinBoxValueChanged;

        betPoints = TeamsManager.Instance.Teams[TeamData.Id].CurrentBet;
        UpdateButtons();
    }

    public void UpdateButtons()
    {
        RemovePointsButton.Text = $"- {betPoints}"; 
        AddPointsButton.Text = $"+ {betPoints}";
        PointsSpinBox.Value = TeamData.Score;
    }

    private void OnRemovePointsButtonPressed()
    {
        TeamData.Score -= betPoints;
        UpdateButtons();
    }
    private void OnAddPointsButtonPressed()
    {
        TeamData.Score += betPoints;
        UpdateButtons();
    }

    private void OnPointsSpinBoxValueChanged(double value)
    {
        TeamData.Score = (int)value;
    }
}
