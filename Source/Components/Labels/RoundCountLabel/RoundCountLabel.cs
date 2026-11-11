using Godot;
using System;

public partial class RoundCountLabel : Label
{
    public override void _Ready()
    {
        Text = LocalizationManager.Format("ROUND_COUNT", GameManager.Instance.RoundCount + 1, GameManager.Instance.MaxRounds);
    }
}
