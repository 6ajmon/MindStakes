using Godot;
using System;

public partial class GameModeCard : MarginContainer
{
    [Export] public Label GameModeLabel;
    [Export] public SpinBox RoundCountSpinBox;
    [Export] public Button StartButton;
    [Export] public GameManager.GameModeEnum GameModeType;
    public override void _Ready()
    {
        RoundCountSpinBox.Value = GameManager.Instance.MaxRounds;
        RoundCountSpinBox.ValueChanged += OnRoundCountChanged;
        UpdateGameModeLabel();
        StartButton.Pressed += OnStartButtonPressed;
    }
    private void OnRoundCountChanged(double value)
    {
        GameManager.Instance.MaxRounds = (int)value;
    }
    private void UpdateGameModeLabel()
    {
        string modeText = GameModeType switch
        {
            GameManager.GameModeEnum.Betting => "OBSTAWIANIE",
            GameManager.GameModeEnum.Sabotage => "SABOTAŻ",
            GameManager.GameModeEnum.Fraud => "OSZUST",
            _ => "Unknown Mode"
        };
        GameModeLabel.Text = modeText;
    }
    private void OnStartButtonPressed()
    {
        GameManager.Instance.CurrentGameModeIndex = (int)GameModeType;
        GameManager.Instance.RoundCount = 0;
    }
}
