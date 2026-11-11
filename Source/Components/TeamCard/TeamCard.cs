using Godot;
using System;

public partial class TeamCard : Panel
{
    public Team TeamData { get; set; } = new();
    [Export] public LineEdit TeamNameLineEdit;
    [Export] public ColorPickerButton ColorPicker;
    private PanelContainer _pictureFrame;

    public override void _Ready()
    {
        _pictureFrame = GetNode<PanelContainer>("VBoxContainer/Panel/MarginContainer/PictureFrame");

        if (TeamNameLineEdit != null)
        {
            TeamNameLineEdit.TextChanged += OnTeamNameLabelTextChanged;
            TeamNameLineEdit.Text = TeamData.TeamName;
        }
        if (ColorPicker != null)
        {
            ColorPicker.ColorChanged += OnColorPickerColorChanged;
            ColorPicker.Color = TeamData.Color;
        }

        ApplyTeamColorBorder();
    }

    public override void _ExitTree()
    {
        if (TeamNameLineEdit != null)
        {
            TeamNameLineEdit.TextChanged -= OnTeamNameLabelTextChanged;
        }
        if (ColorPicker != null)
        {
            ColorPicker.ColorChanged -= OnColorPickerColorChanged;
        }
    }
    
    public void OnTeamNameLabelTextChanged(string newText)
    {
        TeamData.TeamName = newText;
    }
    public void OnColorPickerColorChanged(Color newColor)
    {
        TeamData.Color = newColor;
        ApplyTeamColorBorder();
    }

    private void ApplyTeamColorBorder()
    {
        StyleBoxFlat border = _pictureFrame.GetThemeStylebox("panel").Duplicate() as StyleBoxFlat;
        if (border == null)
        {
            GD.PushError("TeamCard PictureFrame requires a StyleBoxFlat panel style.");
            return;
        }

        border.BorderColor = TeamData.Color;
        _pictureFrame.AddThemeStyleboxOverride("panel", border);
    }
}