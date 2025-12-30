using Godot;
using System;

[GlobalClass]
public partial class Question : Resource
{
    [Export(PropertyHint.MultilineText)]
    public string QuestionText { get; set; }
    [Export]
    public bool IsClosedQuestion { get; set; } = true;
    [Export]
    public Godot.Collections.Array<string> AnswersText { get; set; }
    [Export]
    public bool IsBettingGameQuestion { get; set; } = true;
    [Export]
    public bool IsSabotageGameQuestion { get; set; } = true;
    [Export]
    public bool IsFraudGameQuestion { get; set; } = true;
    [Export]
    public Category Category { get; set; }
    [Export(PropertyHint.MultilineText)]
    public string FunFact { get; set; }
    [Export]
    public Texture2D Photo { get; set; }
    [Export]
    public AudioStream Audio { get; set; }
}
