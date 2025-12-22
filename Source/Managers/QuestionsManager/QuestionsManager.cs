using Godot;
using System;
using System.Collections.Generic;

public partial class QuestionsManager : Node
{
    public static QuestionsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestionsManager>("QuestionsManager");

    public List<Question> Questions { get; private set; } = new();
    public Question RandomQuestion { get; private set; }

    public override void _Ready()
    {
        LoadQuestions();
        SetRandomQuestion();
    }

    private void LoadQuestions()
    {
        var dir = DirAccess.Open("res://Resources/Questions");
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (!dir.CurrentIsDir())
                {
                    if ((fileName.EndsWith(".tres") || fileName.EndsWith(".res")) && !fileName.EndsWith(".import"))
                    {
                        var question = GD.Load<Question>($"res://Resources/Questions/{fileName}");
                        if (question != null)
                        {
                            Questions.Add(question);
                        }
                    }
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.PrintErr("Failed to open Resources/Questions directory.");
        }
    }

    public void SetRandomQuestion()
    {
        if (Questions.Count == 0)
        {
            RandomQuestion = null;
            return;
        }
        var randomIndex = GD.Randi() % Questions.Count;
        RandomQuestion = Questions[(int)randomIndex];
    }
}
