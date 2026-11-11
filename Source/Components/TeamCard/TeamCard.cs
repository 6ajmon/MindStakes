using Godot;
using System;

public partial class TeamCard : Panel
{
    public Team TeamData { get; set; } = new();
    [Export] public LineEdit TeamNameLineEdit;
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

        ApplySavedTeamPicture();
        ApplyTeamColorBorder();
    }

    public override void _ExitTree()
    {
        if (TeamNameLineEdit != null)
        {
            TeamNameLineEdit.TextChanged -= OnTeamNameLabelTextChanged;
        }
    }
    
    public void OnTeamNameLabelTextChanged(string newText)
    {
        TeamData.TeamName = newText;
    }

    public void SaveTeamPicture()
    {
        if (_teamPicture.Texture is Resource texture && !string.IsNullOrEmpty(texture.ResourcePath))
        {
            TeamData.ImagePath = texture.ResourcePath;
        }
    }

    public void SetTeamPicture(string path)
    {
        Texture2D texture = LoadPicture(path);
        if (texture == null)
        {
            GD.PushWarning($"Failed to load team picture '{path}'.");
            return;
        }

        TeamData.ImagePath = path;
        _teamPicture.Texture = texture;
    }

    private void ApplySavedTeamPicture()
    {
        if (string.IsNullOrEmpty(TeamData.ImagePath))
        {
            SaveTeamPicture();
            return;
        }

        SetTeamPicture(TeamData.ImagePath);
    }

    private static Texture2D LoadPicture(string path)
    {
        if (path.StartsWith("res://"))
        {
            return ResourceLoader.Load<Texture2D>(path);
        }

        Image image = new();
        return image.Load(path) == Error.Ok ? ImageTexture.CreateFromImage(image) : null;
    }

    protected void ApplyTeamColorBorder()
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