using Godot;
using System;

public partial class CategoryButton : Button
{
    public Category Category { get; set; }
    [Export(PropertyHint.File, "*.tscn")]
    public string QuestionScenePath { get; set; }
    [Export(PropertyHint.File, "*.tscn")]
    public string BettingScenePath { get; set; }
    public override void _Ready()
    {
        Pressed += OnPressed;
        UpdateButton();
    }
    public void UpdateButton()
    {
        if (Category != null)
        {
            Text = Category.CategoryName;
        }
    }
    public void OnPressed()
    {
        SignalManager.Instance.EmitSignal(nameof(SignalManager.CategorySelected), Category);
        ChangeScene();
    }

    public void ChangeScene()
    {
        if (QuestionScenePath is null || BettingScenePath is null)
        {
            return;
        }
        switch (GameManager.Instance.CurrentGameModeIndex)
        {
            case (int)GameManager.GameModes.Betting:
                SceneManager.Instance.ReplaceWithNewScene(BettingScenePath);
                break;
            case (int)GameManager.GameModes.Sabotage:
                SceneManager.Instance.ReplaceWithNewScene(QuestionScenePath);
                break;
            case (int)GameManager.GameModes.Fraud:
                SceneManager.Instance.ReplaceWithNewScene(QuestionScenePath);
                break;
            default:
                GD.PrintErr("Unknown Game Mode Index");
                break;
        }
    }
}
