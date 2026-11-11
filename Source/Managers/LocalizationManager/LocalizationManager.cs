using Godot;
using System;

public partial class LocalizationManager : Node
{
    public const string DefaultLanguage = "pl";
    public static readonly string[] SupportedLanguages = { "pl", "en" };

    private const string SettingsPath = "user://settings.cfg";
    private const string SettingsSection = "locale";
    private const string LanguageSetting = "language";

    public static LocalizationManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<LocalizationManager>("LocalizationManager");

    public string CurrentLanguage { get; private set; } = DefaultLanguage;

    public override void _Ready()
    {
        ApplyLanguage(LoadSavedLanguage(), save: false);
    }

    public void SetLanguage(string language)
    {
        var normalizedLanguage = NormalizeLanguage(language);
        if (normalizedLanguage == CurrentLanguage)
        {
            return;
        }

        ApplyLanguage(normalizedLanguage, save: true);
    }

    /// <summary>
    /// Returns the translation for <paramref name="key"/> or the key itself when the
    /// catalog has no entry, so a missing translation never leaves an empty text.
    /// </summary>
    public static string Translate(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            return string.Empty;
        }

        var translated = TranslationServer.Translate(key);
        return string.IsNullOrEmpty(translated) ? key : translated;
    }

    public static string Format(string key, params object[] args)
    {
        return string.Format(Translate(key), args);
    }

    private void ApplyLanguage(string language, bool save)
    {
        CurrentLanguage = language;
        TranslationServer.SetLocale(language);

        if (save)
        {
            SaveLanguage(language);
        }
    }

    private static string NormalizeLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return DefaultLanguage;
        }

        var languageCode = language.Trim().ToLowerInvariant().Split('_', '-')[0];
        return Array.IndexOf(SupportedLanguages, languageCode) >= 0 ? languageCode : DefaultLanguage;
    }

    private static string LoadSavedLanguage()
    {
        var config = new ConfigFile();
        if (config.Load(SettingsPath) != Error.Ok)
        {
            return DefaultLanguage;
        }

        return NormalizeLanguage(config.GetValue(SettingsSection, LanguageSetting, DefaultLanguage).AsString());
    }

    private static void SaveLanguage(string language)
    {
        var config = new ConfigFile();
        config.Load(SettingsPath);
        config.SetValue(SettingsSection, LanguageSetting, language);

        var error = config.Save(SettingsPath);
        if (error != Error.Ok)
        {
            GD.PushWarning($"Failed to save the language setting to '{SettingsPath}'. Error: {error}");
        }
    }
}
