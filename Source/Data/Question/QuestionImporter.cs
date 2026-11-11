using Godot;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public static class QuestionImporter
{
    private const string ImportArgumentPrefix = "--import-questions=";
    private const string PoolArgumentPrefix = "--import-pool=";
    private const string DryRunArgument = "--import-dry-run";
    private const string ValidateArgument = "--validate-questions";

    private static readonly string[] PoolAliases = { "pool", "poolnumber", "poolno", "pula" };
    private static readonly string[] CategoryAliases = { "category", "categoryname", "kategoria" };
    private static readonly string[] QuestionAliases = { "question", "questiontext", "pytanie", "text" };
    private static readonly string[] CorrectAliases = { "correct", "correctanswer", "answer1", "odpowiedz1" };
    private static readonly string[] Answer2Aliases = { "answer2", "answerb", "odpowiedz2" };
    private static readonly string[] Answer3Aliases = { "answer3", "answerc", "odpowiedz3" };
    private static readonly string[] Answer4Aliases = { "answer4", "answerd", "odpowiedz4" };
    private static readonly string[] FunFactAliases = { "funfact", "funfacttext", "ciekawostka" };
    private static readonly string[] ImageAliases = { "image", "photo", "imagepath", "obraz" };
    private static readonly string[] AudioAliases = { "audio", "audiopath", "sound", "dzwiek" };

    private sealed class ImportRow
    {
        public int Record;
        public string Source;
        public string PoolText;
        public int Pool;
        public string Category;
        public string QuestionText;
        public string Correct;
        public string Answer2;
        public string Answer3;
        public string Answer4;
        public string FunFact;
        public string ImagePath;
        public string AudioPath;
    }

    public static bool TryRunFromCommandLine()
    {
        var arguments = OS.GetCmdlineUserArgs();

        if (arguments.Any(argument => string.Equals(argument, ValidateArgument, StringComparison.OrdinalIgnoreCase)))
        {
            var valid = QuestionValidator.ValidateAllPools();
            Quit(valid ? 0 : 1);
            return true;
        }

        var importPath = GetArgumentValue(arguments, ImportArgumentPrefix);
        if (string.IsNullOrEmpty(importPath))
        {
            return false;
        }

        var defaultPool = GetArgumentValue(arguments, PoolArgumentPrefix);
        var dryRun = arguments.Any(argument => string.Equals(argument, DryRunArgument, StringComparison.OrdinalIgnoreCase));
        var exitCode = RunImport(importPath, defaultPool, dryRun);
        Quit(exitCode);
        return true;
    }

    private static int RunImport(string inputPath, string defaultPoolText, bool dryRun)
    {
        if (!File.Exists(inputPath))
        {
            GD.PrintErr($"Question import: file not found: '{inputPath}'.");
            return 1;
        }

        int? defaultPool = null;
        if (!string.IsNullOrWhiteSpace(defaultPoolText))
        {
            if (!int.TryParse(defaultPoolText, out var parsedPool) || parsedPool < 1)
            {
                GD.PrintErr($"Question import: --import-pool expects a positive number, got '{defaultPoolText}'.");
                return 1;
            }

            defaultPool = parsedPool;
        }

        List<ImportRow> rows;
        try
        {
            var text = File.ReadAllText(inputPath);
            rows = Path.GetExtension(inputPath).ToLowerInvariant() switch
            {
                ".csv" => ParseCsv(text, defaultPool),
                ".json" => ParseJson(text, defaultPool),
                _ => throw new InvalidDataException("Supported input files are .csv and .json.")
            };
        }
        catch (Exception exception) when (exception is InvalidDataException or IOException)
        {
            GD.PrintErr($"Question import: {exception.Message}");
            return 1;
        }

        if (rows.Count == 0)
        {
            GD.PrintErr("Question import: no questions found in the input file.");
            return 1;
        }

        var errors = new List<string>();
        var prepared = new List<(int Pool, string Source, Question Question)>();
        var sourceName = Path.GetFileName(inputPath);

        foreach (var row in rows)
        {
            row.Source = $"{sourceName} record {row.Record}";
            if (!TryResolvePool(row, defaultPool, errors))
            {
                continue;
            }

            var question = BuildQuestion(row, errors);
            foreach (var problem in QuestionValidator.GetQuestionProblems(question))
            {
                errors.Add($"{row.Source}: {problem}");
            }

            prepared.Add((row.Pool, row.Source, question));
        }

        if (errors.Count > 0)
        {
            foreach (var error in errors)
            {
                GD.PrintErr($"Question import: {error}.");
            }

            GD.PrintErr($"Question import: {errors.Count} problem(s) found, nothing was saved.");
            return 1;
        }

        if (dryRun)
        {
            foreach (var group in prepared.GroupBy(item => item.Pool).OrderBy(group => group.Key))
            {
                var poolPath = QuestionStorage.GetPoolPath(group.Key);
                var nextNumber = GetNextQuestionNumber(poolPath);
                foreach (var item in group)
                {
                    GD.Print($"[dry-run] {poolPath}/{nextNumber++}.tres <- {item.Source}");
                }
            }

            GD.Print($"[dry-run] {prepared.Count} question(s) would be imported. Nothing was saved.");
            return 0;
        }

        foreach (var group in prepared.GroupBy(item => item.Pool).OrderBy(group => group.Key))
        {
            var poolPath = QuestionStorage.GetPoolPath(group.Key);
            var makeDirectoryError = DirAccess.MakeDirRecursiveAbsolute(poolPath);
            if (makeDirectoryError != Error.Ok)
            {
                GD.PrintErr($"Question import: cannot create '{poolPath}' (error {makeDirectoryError}).");
                return 1;
            }

            var nextNumber = GetNextQuestionNumber(poolPath);
            foreach (var item in group)
            {
                var savePath = $"{poolPath}/{nextNumber}.tres";
                var saveError = ResourceSaver.Save(item.Question, savePath);
                if (saveError != Error.Ok)
                {
                    GD.PrintErr($"Question import: failed to save '{savePath}' (error {saveError}).");
                    return 1;
                }

                GD.Print($"Saved {savePath} <- {item.Source}");
                nextNumber++;
            }

            QuestionValidator.ValidatePoolDirectory(poolPath);
        }

        GD.Print($"Question import: imported {prepared.Count} question(s).");
        return 0;
    }

    private static Question BuildQuestion(ImportRow row, List<string> errors)
    {
        var answers = new Godot.Collections.Array<string>();
        AddAnswer(answers, row.Correct);
        AddAnswer(answers, row.Answer2);
        AddAnswer(answers, row.Answer3);
        AddAnswer(answers, row.Answer4);

        return new Question
        {
            QuestionText = row.QuestionText?.Trim(),
            AnswersText = answers,
            IsClosedQuestion = true,
            IsBettingGameQuestion = true,
            IsSabotageGameQuestion = true,
            IsFraudGameQuestion = true,
            Category = LoadCategory(row, errors),
            FunFact = string.IsNullOrWhiteSpace(row.FunFact) ? null : row.FunFact.Trim(),
            Photo = LoadAsset<Texture2D>(row.ImagePath, "image", row, errors),
            Audio = LoadAsset<AudioStream>(row.AudioPath, "audio", row, errors)
        };
    }

    private static void AddAnswer(Godot.Collections.Array<string> answers, string value)
    {
        answers.Add(string.IsNullOrWhiteSpace(value) ? null : value.Trim());
    }

    private static Category LoadCategory(ImportRow row, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(row.Category))
        {
            errors.Add($"{row.Source}: category is empty");
            return null;
        }

        var path = row.Category.StartsWith("res://", StringComparison.Ordinal)
            ? row.Category
            : $"res://Resources/Categories/{row.Category}.tres";

        if (!ResourceLoader.Exists(path))
        {
            errors.Add($"{row.Source}: category '{row.Category}' not found (looked for '{path}')");
            return null;
        }

        var category = GD.Load<Category>(path);
        if (category == null)
        {
            errors.Add($"{row.Source}: '{path}' is not a Category resource");
        }

        return category;
    }

    private static T LoadAsset<T>(string path, string label, ImportRow row, List<string> errors) where T : Resource
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        if (!path.StartsWith("res://", StringComparison.Ordinal))
        {
            errors.Add($"{row.Source}: {label} path must start with res:// (got '{path}')");
            return null;
        }

        if (!ResourceLoader.Exists(path))
        {
            errors.Add($"{row.Source}: {label} not found at '{path}'");
            return null;
        }

        var asset = GD.Load<T>(path);
        if (asset == null)
        {
            errors.Add($"{row.Source}: '{path}' is not a {typeof(T).Name}");
        }

        return asset;
    }

    private static bool TryResolvePool(ImportRow row, int? defaultPool, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(row.PoolText) && defaultPool.HasValue)
        {
            row.Pool = defaultPool.Value;
            return true;
        }

        if (!int.TryParse(row.PoolText, out var pool) || pool < 1)
        {
            errors.Add($"{row.Source}: pool must be a positive number (got '{row.PoolText ?? string.Empty}')");
            return false;
        }

        row.Pool = pool;
        return true;
    }

    private static int GetNextQuestionNumber(string poolPath)
    {
        var highestNumber = 0;
        var dir = DirAccess.Open(poolPath);
        if (dir == null)
        {
            return 1;
        }

        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".tres") && !fileName.EndsWith(".import"))
            {
                var baseName = fileName[..^".tres".Length];
                if (int.TryParse(baseName, out var parsedNumber) && parsedNumber > highestNumber)
                {
                    highestNumber = parsedNumber;
                }
            }

            fileName = dir.GetNext();
        }

        return highestNumber + 1;
    }

    private static List<ImportRow> ParseCsv(string text, int? defaultPool)
    {
        text = text.TrimStart('\uFEFF');
        var records = ParseCsvRecords(text, DetectDelimiter(text));
        if (records.Count == 0)
        {
            throw new InvalidDataException("CSV file is empty.");
        }

        var header = records[0].Select(NormalizeName).ToList();

        int FindColumn(string[] aliases) => header.FindIndex(name => aliases.Contains(name));

        var poolIndex = FindColumn(PoolAliases);
        var categoryIndex = FindColumn(CategoryAliases);
        var questionIndex = FindColumn(QuestionAliases);
        var correctIndex = FindColumn(CorrectAliases);
        var answer2Index = FindColumn(Answer2Aliases);
        var answer3Index = FindColumn(Answer3Aliases);
        var answer4Index = FindColumn(Answer4Aliases);
        var funFactIndex = FindColumn(FunFactAliases);
        var imageIndex = FindColumn(ImageAliases);
        var audioIndex = FindColumn(AudioAliases);

        if (categoryIndex < 0 || questionIndex < 0 || correctIndex < 0
            || answer2Index < 0 || answer3Index < 0 || answer4Index < 0)
        {
            throw new InvalidDataException(
                "CSV must have the columns: pool (or --import-pool), category, question, correct, answer2, answer3, answer4. " +
                $"Found: {string.Join(", ", header)}");
        }

        var rows = new List<ImportRow>();
        for (var recordIndex = 1; recordIndex < records.Count; recordIndex++)
        {
            var record = records[recordIndex];
            if (record.All(string.IsNullOrWhiteSpace))
            {
                continue;
            }

            rows.Add(new ImportRow
            {
                Record = recordIndex + 1,
                PoolText = GetCell(record, poolIndex) ?? defaultPool?.ToString(),
                Category = GetCell(record, categoryIndex),
                QuestionText = GetCell(record, questionIndex),
                Correct = GetCell(record, correctIndex),
                Answer2 = GetCell(record, answer2Index),
                Answer3 = GetCell(record, answer3Index),
                Answer4 = GetCell(record, answer4Index),
                FunFact = GetCell(record, funFactIndex),
                ImagePath = GetCell(record, imageIndex),
                AudioPath = GetCell(record, audioIndex)
            });
        }

        return rows;
    }

    private static List<ImportRow> ParseJson(string text, int? defaultPool)
    {
        var json = new Json();
        var parseError = json.Parse(text);
        if (parseError != Error.Ok)
        {
            throw new InvalidDataException($"invalid JSON at line {json.GetErrorLine()}: {json.GetErrorMessage()}");
        }

        if (json.Data.VariantType != Variant.Type.Array)
        {
            throw new InvalidDataException("JSON root must be an array of question objects.");
        }

        var rows = new List<ImportRow>();
        var record = 0;
        foreach (var element in json.Data.AsGodotArray())
        {
            record++;
            if (element.VariantType != Variant.Type.Dictionary)
            {
                throw new InvalidDataException($"JSON record {record} is not an object.");
            }

            var dictionary = element.AsGodotDictionary();
            rows.Add(new ImportRow
            {
                Record = record,
                PoolText = GetDictionaryValue(dictionary, PoolAliases) ?? defaultPool?.ToString(),
                Category = GetDictionaryValue(dictionary, CategoryAliases),
                QuestionText = GetDictionaryValue(dictionary, QuestionAliases),
                Correct = GetDictionaryValue(dictionary, CorrectAliases),
                Answer2 = GetDictionaryValue(dictionary, Answer2Aliases),
                Answer3 = GetDictionaryValue(dictionary, Answer3Aliases),
                Answer4 = GetDictionaryValue(dictionary, Answer4Aliases),
                FunFact = GetDictionaryValue(dictionary, FunFactAliases),
                ImagePath = GetDictionaryValue(dictionary, ImageAliases),
                AudioPath = GetDictionaryValue(dictionary, AudioAliases)
            });
        }

        return rows;
    }

    private static string GetDictionaryValue(Godot.Collections.Dictionary dictionary, string[] aliases)
    {
        foreach (var key in dictionary.Keys)
        {
            if (!aliases.Contains(NormalizeName(key.AsString())))
            {
                continue;
            }

            var value = VariantToText(dictionary[key]);
            return value.Length == 0 ? null : value;
        }

        return null;
    }

    private static string VariantToText(Variant value)
    {
        switch (value.VariantType)
        {
            case Variant.Type.Nil:
                return string.Empty;
            case Variant.Type.Int:
                return value.AsInt32().ToString(CultureInfo.InvariantCulture);
            case Variant.Type.Float:
                var number = value.AsDouble();
                return number == Math.Floor(number)
                    ? ((long)number).ToString(CultureInfo.InvariantCulture)
                    : number.ToString(CultureInfo.InvariantCulture);
            default:
                return value.AsString().Trim();
        }
    }

    private static string GetCell(List<string> record, int index)
    {
        if (index < 0 || index >= record.Count || string.IsNullOrWhiteSpace(record[index]))
        {
            return null;
        }

        return record[index].Trim();
    }

    private static char DetectDelimiter(string text)
    {
        var firstLineEnd = text.IndexOf('\n');
        var firstLine = firstLineEnd >= 0 ? text[..firstLineEnd] : text;

        var commas = 0;
        var semicolons = 0;
        var tabs = 0;
        var inQuotes = false;

        foreach (var character in firstLine)
        {
            if (character == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (!inQuotes)
            {
                switch (character)
                {
                    case ',':
                        commas++;
                        break;
                    case ';':
                        semicolons++;
                        break;
                    case '\t':
                        tabs++;
                        break;
                }
            }
        }

        if (tabs > commas && tabs > semicolons)
        {
            return '\t';
        }

        return semicolons > commas ? ';' : ',';
    }

    private static List<List<string>> ParseCsvRecords(string text, char delimiter)
    {
        var records = new List<List<string>>();
        var record = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;
        var atFieldStart = true;

        for (var index = 0; index < text.Length; index++)
        {
            var character = text[index];

            if (inQuotes)
            {
                if (character == '"')
                {
                    if (index + 1 < text.Length && text[index + 1] == '"')
                    {
                        field.Append('"');
                        index++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(character);
                }

                continue;
            }

            if (character == '"' && atFieldStart)
            {
                inQuotes = true;
                atFieldStart = false;
            }
            else if (character == '\n')
            {
                record.Add(field.ToString());
                field.Clear();
                records.Add(record);
                record = new List<string>();
                atFieldStart = true;
            }
            else if (character != '\r')
            {
                if (character == delimiter)
                {
                    record.Add(field.ToString());
                    field.Clear();
                    atFieldStart = true;
                }
                else
                {
                    field.Append(character);
                    atFieldStart = false;
                }
            }
        }

        if (field.Length > 0 || record.Count > 0)
        {
            record.Add(field.ToString());
            records.Add(record);
        }

        return records;
    }

    private static string NormalizeName(string value) =>
        new string((value ?? string.Empty).ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());

    private static string GetArgumentValue(string[] arguments, string prefix)
    {
        foreach (var argument in arguments)
        {
            if (argument.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return argument[prefix.Length..].Trim().Trim('"');
            }
        }

        return null;
    }

    private static void Quit(int exitCode)
    {
        GD.Print($"Question tools finished (exit code {exitCode}).");
        if (Engine.GetMainLoop() is SceneTree tree)
        {
            tree.Quit(exitCode);
        }
    }
}
