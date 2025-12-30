using Godot;
using System;

public partial class GameModePickScreen : Control
{
    [Export] public SpinBox RerollCountSpinBox { get; set; }
    [Export] public SpinBox QuestionTimeSpinBox { get; set; }
    public override void _Ready()
    {
        RerollCountSpinBox.Value = GameManager.Instance.InitialRerollCount;
        QuestionTimeSpinBox.Value = GameManager.Instance.QuestionTime;
        RerollCountSpinBox.ValueChanged += OnRerollCountSpinBoxValueChanged;
        QuestionTimeSpinBox.ValueChanged += OnQuestionTimeSpinBoxValueChanged;
    }

    public override void _ExitTree()
    {
        GameManager.Instance.InitialRerollCount = (int)RerollCountSpinBox.Value;
        GameManager.Instance.RerollCount = (int)RerollCountSpinBox.Value;
        GameManager.Instance.QuestionTime = (int)QuestionTimeSpinBox.Value;

        RerollCountSpinBox.ValueChanged -= OnRerollCountSpinBoxValueChanged;
        QuestionTimeSpinBox.ValueChanged -= OnQuestionTimeSpinBoxValueChanged;
    }

    private void OnRerollCountSpinBoxValueChanged(double value)
    {
        GameManager.Instance.InitialRerollCount = (int)value;
    }
    private void OnQuestionTimeSpinBoxValueChanged(double value)
    {
        GameManager.Instance.QuestionTime = (int)value;
    }
}
