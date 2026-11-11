using Godot;
using System;
using System.Linq;

public partial class TitleScreen : Control
{
    [Export] public Label TitleLabel { get; set; }

    public override void _Ready()
    {
        CreateLanguageSwitcher();
    }

    private void CreateLanguageSwitcher()
    {
        var switcher = new HBoxContainer
        {
            Name = "LanguageSwitcher",
            Alignment = BoxContainer.AlignmentMode.End
        };
        switcher.AddThemeConstantOverride("separation", 12);
        AddChild(switcher);

        switcher.AnchorLeft = 1f;
        switcher.AnchorRight = 1f;
        switcher.OffsetLeft = -240f;
        switcher.OffsetRight = -32f;
        switcher.OffsetTop = 32f;
        switcher.OffsetBottom = 96f;
        switcher.GrowHorizontal = GrowDirection.Begin;

        foreach (var language in LocalizationManager.SupportedLanguages)
        {
            var button = new Button
            {
                Text = language.ToUpperInvariant(),
                ToggleMode = true,
                ButtonPressed = language == LocalizationManager.Instance.CurrentLanguage,
                CustomMinimumSize = new Vector2(88, 56)
            };
            button.SetMeta("language", language);
            button.Pressed += () => OnLanguageSelected(language);
            switcher.AddChild(button);
        }

        UpdateLanguageSwitcher();
    }

    private void OnLanguageSelected(string language)
    {
        LocalizationManager.Instance.SetLanguage(language);
        UpdateLanguageSwitcher();
    }

    private void UpdateLanguageSwitcher()
    {
        var switcher = GetNodeOrNull<HBoxContainer>("LanguageSwitcher");
        if (switcher == null)
        {
            return;
        }

        var currentLanguage = LocalizationManager.Instance.CurrentLanguage;
        foreach (var button in switcher.GetChildren().OfType<Button>())
        {
            button.ButtonPressed = button.GetMeta("language").AsString() == currentLanguage;
        }
    }
}
