using Godot;
using System;

public partial class CategoryButton : Button
{
    public Category Category { get; set; }
    public override void _Ready()
    {
        Pressed += OnPressed;
        UpdateButton();
    }
    public void UpdateButton()
    {
        if (Category != null)
        {
            Text = Category.CategoryName;
        }
    }
    public void OnPressed()
    {
        SignalManager.Instance.EmitSignal(nameof(SignalManager.CategorySelected), Category);
    }
}
