using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestionsManager : Node
{
    public static QuestionsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestionsManager>("QuestionsManager");

    public List<Question> Questions { get; private set; } = new();
    public Question RandomQuestion { get; private set; }
    
    private const string QuestionsRootPath = "res://Resources/Questions";

    public override void _Ready()
    {
        SetRandomQuestion();
    }

    public bool HasQuestionsForCategory(Category category)
    {
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
    }

    public void RemoveCurrentQuestion()
    {
        if (RandomQuestion != null)
        {
            Questions.Remove(RandomQuestion);
            RandomQuestion = null;
        }
    }

    public int GetMaxPoolNumber()
    {
        var dir = DirAccess.Open(QuestionsRootPath);
        if (dir == null)
        {
            return 1;
        }

        var maxPoolNumber = 0;
        dir.ListDirBegin();
        var entry = dir.GetNext();
        while (!string.IsNullOrEmpty(entry))
        {
            if (dir.CurrentIsDir() && entry.StartsWith("Pool", StringComparison.OrdinalIgnoreCase))
            {
                var numberPart = new string(entry.Where(char.IsDigit).ToArray());
                if (int.TryParse(numberPart, out var poolNumber) && poolNumber > maxPoolNumber)
                {
                    maxPoolNumber = poolNumber;
                }
            }
            entry = dir.GetNext();
        }

        return maxPoolNumber > 0 ? maxPoolNumber : 1;
    }

    public void LoadQuestionsFromPool(int poolNumber)
    {
        Questions.Clear();

        var poolPath = $"{QuestionsRootPath}/Pool{poolNumber}";
        var dir = DirAccess.Open(poolPath);
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (!string.IsNullOrEmpty(fileName))
            {
                if (!dir.CurrentIsDir())
                {
                    if ((fileName.EndsWith(".tres") || fileName.EndsWith(".res")) && !fileName.EndsWith(".import"))
                    {
                        var question = GD.Load<Question>($"{poolPath}/{fileName}");
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
            GD.PrintErr($"Failed to open {poolPath} directory.");
        }
    }
}
