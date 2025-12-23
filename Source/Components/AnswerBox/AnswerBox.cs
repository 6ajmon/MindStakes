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
        SelfModulate = Colors.Green;
    }
}
