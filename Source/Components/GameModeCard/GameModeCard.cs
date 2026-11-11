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
            GameManager.GameModeEnum.Betting => Tr("GAME_MODE_BETTING"),
            GameManager.GameModeEnum.Sabotage => Tr("GAME_MODE_SABOTAGE"),
            GameManager.GameModeEnum.Fraud => Tr("GAME_MODE_FRAUD"),
            _ => Tr("GAME_MODE_UNKNOWN")
        };
        GameModeLabel.Text = modeText;
    }
    private void OnStartButtonPressed()
    {
        GameManager.Instance.CurrentGameModeIndex = (int)GameModeType;
        GameManager.Instance.RoundCount = 0;
        CategoriesManager.Instance.SetUpCategories();
    }
}
