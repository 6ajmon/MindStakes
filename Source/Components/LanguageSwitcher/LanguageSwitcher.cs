using Godot;
using System;

public partial class LanguageSwitcher : HBoxContainer
{
    [Export] public Button PolishButton { get; set; }
    [Export] public Button EnglishButton { get; set; }

    public override void _Ready()
    {
        PolishButton ??= GetNodeOrNull<Button>("PLButton");
        EnglishButton ??= GetNodeOrNull<Button>("ENButton");

        if (PolishButton == null || EnglishButton == null)
        {
            GD.PushWarning("LanguageSwitcher needs PLButton and ENButton children (or the exported button fields set).");
            return;
        }

        ConfigureButton(PolishButton, LocalizationManager.PolishLanguage);
        ConfigureButton(EnglishButton, LocalizationManager.EnglishLanguage);

        PolishButton.Pressed += () => OnLanguageButtonPressed(LocalizationManager.PolishLanguage);
        EnglishButton.Pressed += () => OnLanguageButtonPressed(LocalizationManager.EnglishLanguage);

        UpdateButtons();
    }

    private static void ConfigureButton(Button button, string language)
    {
        button.Text = language.ToUpperInvariant();
        button.ToggleMode = true;
    }

    private void OnLanguageButtonPressed(string language)
    {
        LocalizationManager.Instance.SetLanguage(language);
        UpdateButtons();
    }

    private void UpdateButtons()
    {
        var currentLanguage = LocalizationManager.Instance.CurrentLanguage;
        PolishButton.ButtonPressed = currentLanguage == LocalizationManager.PolishLanguage;
        EnglishButton.ButtonPressed = currentLanguage == LocalizationManager.EnglishLanguage;
    }
}
