using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class QuestionValidator
{
    // Keeps in sync with MaxRounds in GameManager.tscn.
    public const int RecommendedQuestionsPerSession = 28;
    public const int RequiredAnswers = 4;

    public static List<string> GetQuestionProblems(Question question)
    {
        var problems = new List<string>();

        if (string.IsNullOrWhiteSpace(question.QuestionText))
        {
            problems.Add("question text is empty");
        }

        if (question.Category == null)
        {
            problems.Add("category is not set");
        }

        var answers = question.AnswersText;
        if (answers == null || answers.Count < RequiredAnswers)
        {
            problems.Add($"needs {RequiredAnswers} answers, has {answers?.Count ?? 0}");
            return problems;
        }

        if (string.IsNullOrWhiteSpace(answers[0]))
        {
            problems.Add("the correct answer (index 0) is empty");
        }

        var filled = answers
            .Where(answer => !string.IsNullOrWhiteSpace(answer))
            .Select(answer => answer.Trim())
            .ToList();

        if (filled.Count != answers.Count)
        {
            problems.Add("an answer is empty");
        }

        var duplicates = filled
            .GroupBy(answer => answer, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicates.Count > 0)
        {
            problems.Add($"duplicate answer text: {string.Join(", ", duplicates)}");
        }

        return problems;
    }

    public static bool ValidateQuestion(Question question, string source)
    {
        var problems = GetQuestionProblems(question);
        foreach (var problem in problems)
        {
            GD.PushWarning($"{source}: {problem}.");
        }

        return problems.Count == 0;
    }

    public static bool ValidateAndReport(string label, IReadOnlyList<(string FilePath, Question Question)> files)
    {
        GD.Print($"{label}: {files.Count} question(s) loaded (a full session uses {RecommendedQuestionsPerSession}).");

        foreach (var group in files
                     .GroupBy(file => file.Question.Category?.CategoryName ?? "(no category)")
                     .OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
        {
            GD.Print($"  {group.Key}: {group.Count()}");
        }

        if (files.Count < RecommendedQuestionsPerSession)
        {
            GD.PushWarning(
                $"{label} has {files.Count} question(s); a full session uses {RecommendedQuestionsPerSession}. " +
                "Add more questions or lower the round count.");
        }

        var allValid = true;
        foreach (var file in files)
        {
            allValid &= ValidateQuestion(file.Question, file.FilePath);
        }

        return allValid;
    }

    public static bool ValidatePoolDirectory(string poolPath)
    {
        var files = QuestionStorage.LoadPoolFiles(poolPath);
        if (files == null)
        {
            GD.PushWarning($"Cannot open question folder '{poolPath}'.");
            return false;
        }

        return ValidateAndReport(GetLastPathPart(poolPath), files);
    }

    public static bool ValidateAllPools()
    {
        var rootPath = QuestionStorage.RootPath;
        var dir = DirAccess.Open(rootPath);
        if (dir == null)
        {
            GD.PrintErr($"Cannot open question folder '{rootPath}'.");
            return false;
        }

        var poolPaths = new List<string>();
        dir.ListDirBegin();
        var entry = dir.GetNext();
        while (!string.IsNullOrEmpty(entry))
        {
            if (dir.CurrentIsDir() && entry.StartsWith("Pool", StringComparison.OrdinalIgnoreCase))
            {
                poolPaths.Add($"{rootPath}/{entry}");
            }

            entry = dir.GetNext();
        }

        if (poolPaths.Count == 0)
        {
            GD.PrintErr($"No Pool folders found in '{rootPath}'.");
            return false;
        }

        var allValid = true;
        foreach (var poolPath in poolPaths.OrderBy(path => path, StringComparer.OrdinalIgnoreCase))
        {
            allValid &= ValidatePoolDirectory(poolPath);
        }

        return allValid;
    }

    private static string GetLastPathPart(string path)
    {
        var trimmed = path.TrimEnd('/');
        var slashIndex = trimmed.LastIndexOf('/');
        return slashIndex >= 0 ? trimmed[(slashIndex + 1)..] : trimmed;
    }
}
