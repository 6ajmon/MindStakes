using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestionsManager : Node
{
    public static QuestionsManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<QuestionsManager>("QuestionsManager");

    public List<Question> Questions { get; private set; } = new();
    public Question RandomQuestion { get; private set; }

    public override void _Ready()
    {
        if (QuestionImporter.TryRunFromCommandLine())
        {
            return;
        }

        GD.Print($"Question storage root: '{QuestionStorage.RootPath}'.");
        SetRandomQuestion();

        if (!OS.HasFeature("editor") && !QuestionStorage.IsExternal)
        {
            GD.PushWarning(
                "Questions are loaded from res:// and are bundled into the exported build. " +
                $"Set {QuestionStorage.EnvironmentVariableName} or pass --questions-dir=<path> to keep them outside the project.");
        }
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
        var dir = DirAccess.Open(QuestionStorage.RootPath);
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

        var poolPath = QuestionStorage.GetPoolPath(poolNumber);
        GD.Print($"Loading questions from '{poolPath}'.");

        var files = QuestionStorage.LoadPoolFiles(poolPath);
        if (files == null)
        {
            GD.PrintErr($"Failed to open {poolPath} directory.");
            return;
        }

        foreach (var file in files)
        {
            Questions.Add(file.Question);
        }

        GD.Print($"Loaded {Questions.Count} question(s) from '{poolPath}'.");
        QuestionValidator.ValidateAndReport($"Pool{poolNumber}", files);
    }
}
