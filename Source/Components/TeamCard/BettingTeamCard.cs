using Godot;
using System;

public partial class BettingTeamCard : TeamCard
{
    [Export] public SpinBox BetAmountSpinBox;
    public override void _Ready()
    {
        base._Ready();
        BetAmountSpinBox.MaxValue = TeamData.Score > GameManager.Instance.MaxBetAmount ? GameManager.Instance.MaxBetAmount : TeamData.Score;
    }

    public void OnBettingSpinBoxValueChanged(double value)
    {
        TeamData.CurrentBet = (int)value;
    }
}
