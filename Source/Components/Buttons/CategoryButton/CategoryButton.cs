using Godot;
using System;

public partial class CategoryButton : Button
{
    public Category Category { get; set; }
    [Export(PropertyHint.File, "*.tscn")]
    public string QuestionScenePath { get; set; }
    [Export(PropertyHint.File, "*.tscn")]
    public string BettingScenePath { get; set; }
    [Export(PropertyHint.File, "*.tscn")]
    public string SabotageScenePath { get; set; }
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
        CategoriesManager.Instance.RandomCategory = Category;
        QuestionsManager.Instance.SetRandomQuestion();
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
            case (int)GameManager.GameModeEnum.Betting:
                SceneManager.Instance.ReplaceWithNewScene(BettingScenePath);
                break;
            case (int)GameManager.GameModeEnum.Sabotage:
                SceneManager.Instance.ReplaceWithNewScene(SabotageScenePath);
                break;
            case (int)GameManager.GameModeEnum.Fraud:
                SceneManager.Instance.ReplaceWithNewScene(QuestionScenePath);
                break;
            default:
                GD.PrintErr("Unknown Game Mode Index");
                break;
        }
    }
}
