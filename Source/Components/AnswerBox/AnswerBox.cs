using Godot;
using System;

public partial class AnswerBox : PanelContainer
{
    [Export] public Label LetterLabel { get; set; }
    [Export] public Label AnswerLabel { get; set; }
    private string answerText = "Default Answer";
    public string AnswerText
    {
        get => answerText;
        set
        {
            answerText = value;
            if (AnswerLabel != null)
            {
                AnswerLabel.Text = answerText;
            }
        }
    }
    private string letterText = "A";
    public string LetterText
    {
        get => letterText;
        set
        {
            letterText = value;
            if (LetterLabel != null)
            {
                LetterLabel.Text = letterText;
            }
        }
    }

    public override void _Ready()
    {
        UpdateTexts();
    }

    public void UpdateTexts()
    {
        if (AnswerLabel != null)
        {
            AnswerLabel.Text = answerText;
        }
        if (LetterLabel != null)
        {
            LetterLabel.Text = letterText;
        }
    }

    public void MarkAsCorrect()
    {
        var tween = CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        
        // Blink effect
        for (int i = 0; i < 3; i++)
        {
            tween.TweenProperty(this, "self_modulate", Colors.Green, 0.15f);
            tween.TweenProperty(this, "self_modulate", Colors.White, 0.15f);
        }
        tween.TweenProperty(this, "self_modulate", Colors.Green, 0.15f);
    }
}
