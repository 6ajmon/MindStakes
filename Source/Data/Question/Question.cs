using Godot;
using System;

[GlobalClass]
public partial class Question : Resource
{
    [Export]
    public string QuestionText { get; set; }
    [Export]
    public bool IsClosedQuestion { get; set; } = true;
    [Export]
    public Godot.Collections.Array<string> AnswersText { get; set; }
    [Export]
    public bool IsBettingGameQuestion { get; set; } = false;
    [Export]
    public bool IsSabotageGameQuestion { get; set; } = false;
    [Export]
    public bool IsFraudGameQuestion { get; set; } = false;
    [Export]
    public Category Category { get; set; }
    [Export]
    public Texture2D Photo { get; set; }
}
