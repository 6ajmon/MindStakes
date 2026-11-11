using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public static class QuestionStorage
{
    public const string DefaultRootPath = "res://Resources/Questions";
    public const string DirectoryArgumentPrefix = "--questions-dir=";
    public const string EnvironmentVariableName = "MINDSTAKES_QUESTIONS_DIR";

    private const string DirectoryProjectSetting = "mindstakes/questions_dir";

    private static string _rootPath;

    public static string RootPath => _rootPath ??= ResolveRootPath();

    public static bool IsExternal => !RootPath.StartsWith("res://", StringComparison.Ordinal);

    public static string GetPoolPath(int poolNumber) => $"{RootPath}/Pool{poolNumber}";

    public static List<(string FilePath, Question Question)> LoadPoolFiles(string poolPath)
    {
        var dir = DirAccess.Open(poolPath);
        if (dir == null)
        {
            return null;
        }

        var files = new List<(string FilePath, Question Question)>();
        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir()
                && (fileName.EndsWith(".tres") || fileName.EndsWith(".res"))
                && !fileName.EndsWith(".import"))
            {
                var filePath = $"{poolPath}/{fileName}";
                var question = GD.Load<Question>(filePath);
                if (question != null)
                {
                    files.Add((filePath, question));
                }
            }

            fileName = dir.GetNext();
        }

        return files;
    }

    private static string ResolveRootPath()
    {
        var explicitPath = Normalize(GetArgumentValue())
            ?? Normalize(System.Environment.GetEnvironmentVariable(EnvironmentVariableName))
            ?? Normalize(ProjectSettings.GetSetting(DirectoryProjectSetting).AsString());

        if (!string.IsNullOrEmpty(explicitPath))
        {
            return explicitPath;
        }

        if (!OS.HasFeature("editor"))
        {
            var executableDirectory = Path.GetDirectoryName(OS.GetExecutablePath());
            var portablePath = Normalize($"{executableDirectory}/questions");
            if (DirAccess.DirExistsAbsolute(portablePath))
            {
                return portablePath;
            }
        }

        const string userPath = "user://questions";
        if (DirAccess.DirExistsAbsolute(userPath))
        {
            return userPath;
        }

        return DefaultRootPath;
    }

    private static string GetArgumentValue()
    {
        foreach (var argument in OS.GetCmdlineUserArgs())
        {
            if (argument.StartsWith(DirectoryArgumentPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return argument[DirectoryArgumentPrefix.Length..];
            }
        }

        return null;
    }

    private static string Normalize(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var trimmed = path.Trim().Trim('"').Replace('\\', '/').TrimEnd('/');
        return string.IsNullOrEmpty(trimmed) ? null : trimmed;
    }
}
