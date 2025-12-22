using Godot;
using System;

public partial class TeamCard : Panel
{
    public Team TeamData { get; set; } = new();
    [Export] public LineEdit TeamNameLineEdit;
    [Export] public ColorPickerButton ColorPicker;
    public override void _Ready()
    {
        TeamNameLineEdit.TextChanged += OnTeamNameLabelTextChanged;
        ColorPicker.ColorChanged += OnColorPickerColorChanged;
        if (TeamNameLineEdit != null)
        {
            TeamNameLineEdit.Text = TeamData.TeamName;
        }
        if (ColorPicker != null)
        {
            ColorPicker.Color = TeamData.Color;
        }
    }

    public override void _ExitTree()
    {
        TeamNameLineEdit.TextChanged -= OnTeamNameLabelTextChanged;
        ColorPicker.ColorChanged -= OnColorPickerColorChanged;
    }
    
    public void OnTeamNameLabelTextChanged(string newText)
    {
        TeamData.TeamName = newText;
    }
    public void OnColorPickerColorChanged(Color newColor)
    {
        TeamData.Color = newColor;
    }
}