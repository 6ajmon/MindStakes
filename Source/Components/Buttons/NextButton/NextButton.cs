using Godot;
using System;

[GlobalClass]
public partial class NextButton : Button
{
    [Export(PropertyHint.File, "*.tscn")]
    public string ScenePath { get; set; }

    public override void _Ready()
    {
        Pressed += OnPressed;
    }
    public async void OnPressed()
    {
        if (!string.IsNullOrEmpty(ScenePath))
        {
            SceneManager.Instance.ReplaceWithNewScene(ScenePath);
        }
    }
}