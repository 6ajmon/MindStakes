using Godot;
using System;

public partial class CategoryButton : Button
{
    public Category Category { get; set; }
    public override void _Ready()
    {
        Pressed += OnPressed;
    }
    public void SetCategoryDisplayName(string displayName)
    {
        Category.CategoryName = displayName;
        Text = displayName;
    }
    public void OnPressed()
    {
        SignalManager.Instance.EmitSignal(nameof(SignalManager.CategorySelected), Category);
    }
}
