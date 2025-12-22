using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestionsManager : Node
{
    public static QuestionsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestionsManager>("QuestionsManager");

    public List<Question> Questions { get; private set; } = new();
    public Question RandomQuestion { get; private set; }
    private bool _questionsLoaded = false;

    public override void _Ready()
    {
        LoadQuestions();
        SetRandomQuestion();
    }

    private void LoadQuestions()
    {
        if (_questionsLoaded) return;
        
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
            _questionsLoaded = true;
        }
        else
        {
            GD.PrintErr("Failed to open Resources/Questions directory.");
        }
    }

    public bool HasQuestionsForCategory(Category category)
    {
        if (!_questionsLoaded) LoadQuestions();
        return Questions.Any(q => q.Category == category);
    }

    public void SetRandomQuestion()
    {
        var currentCategory = CategoriesManager.Instance.RandomCategory;
        if (currentCategory == null)
        {
            RandomQuestion = null;
            return;
        }

        var availableQuestions = Questions.Where(q => q.Category == currentCategory).ToList();

        if (availableQuestions.Count == 0)
        {
            RandomQuestion = null;
            return;
        }

        var randomIndex = GD.Randi() % availableQuestions.Count;
        RandomQuestion = availableQuestions[(int)randomIndex];
        Questions.Remove(RandomQuestion);
    }
}
