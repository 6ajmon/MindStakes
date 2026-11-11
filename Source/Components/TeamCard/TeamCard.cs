using Godot;
using System;

public partial class TeamCard : Panel
{
    public Team TeamData { get; set; } = new();
    [Export] public LineEdit TeamNameLineEdit;
    [Export] public ColorPickerButton ColorPicker;
    private PanelContainer _pictureFrame;
    private TextureRect _teamPicture;

    public override void _Ready()
    {
        _pictureFrame = GetNode<PanelContainer>("VBoxContainer/Panel/MarginContainer/PictureFrame");
        _teamPicture = GetNode<TextureRect>("VBoxContainer/Panel/MarginContainer/PictureFrame/TeamPicture");

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

        ApplySavedTeamPicture();
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

    public void SaveTeamPicture()
    {
        if (_teamPicture.Texture is Resource texture && !string.IsNullOrEmpty(texture.ResourcePath))
        {
            TeamData.ImagePath = texture.ResourcePath;
        }
    }

    private void ApplySavedTeamPicture()
    {
        if (string.IsNullOrEmpty(TeamData.ImagePath))
        {
            SaveTeamPicture();
            return;
        }

        Texture2D texture = ResourceLoader.Load<Texture2D>(TeamData.ImagePath);
        if (texture == null)
        {
            GD.PushWarning($"Failed to load team picture '{TeamData.ImagePath}'.");
            return;
        }

        _teamPicture.Texture = texture;
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