using Godot;
using System;

public partial class RoundCountLabel : Label
{
    public override void _Ready()
    {
        Text = $"Runda {GameManager.Instance.RoundCount + 1}/{GameManager.Instance.MaxRounds}";
    }
}
