using Godot;
using System;

public partial class TeamEditCard : TeamCard
{
    private const string PictureFolderPath = "user://team_pictures";

    [Export] public ColorPickerButton ColorPicker;

    private FileDialog _pictureFileDialog;

    public override void _Ready()
    {
        base._Ready();

        if (ColorPicker != null)
        {
            ColorPicker.ColorChanged += OnColorPickerColorChanged;
            ColorPicker.Color = TeamData.Color;
        }

        TextureButton addPictureButton = GetNodeOrNull<TextureButton>("VBoxContainer/TeamCardColorPicker/HBoxContainer/MarginContainer/AddPictureButton");
        if (addPictureButton == null)
        {
            GD.PushError("TeamEditCard requires an AddPictureButton in TeamCardColorPicker.");
            return;
        }

        addPictureButton.Pressed += OnAddPictureButtonPressed;

        _pictureFileDialog = new FileDialog
        {
            Access = FileDialog.AccessEnum.Filesystem,
            FileMode = FileDialog.FileModeEnum.OpenFile,
            UseNativeDialog = true,
            Filters = new[] { "*.png, *.jpg, *.jpeg, *.webp ; Image" },
        };
        _pictureFileDialog.FileSelected += OnPictureFileSelected;
        AddChild(_pictureFileDialog);
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (ColorPicker != null)
        {
            ColorPicker.ColorChanged -= OnColorPickerColorChanged;
        }
        if (_pictureFileDialog != null)
        {
            _pictureFileDialog.FileSelected -= OnPictureFileSelected;
        }
    }

    public void OnColorPickerColorChanged(Color newColor)
    {
        TeamData.Color = newColor;
        ApplyTeamColorBorder();
    }

    private void OnAddPictureButtonPressed()
    {
        _pictureFileDialog.PopupCenteredRatio(0.65f);
    }

    private void OnPictureFileSelected(string sourcePath)
    {
        // Picked images are copied into user:// so the team keeps its picture across launches.
        string targetPath = CopyPicture(sourcePath);
        if (targetPath != null)
        {
            SetTeamPicture(targetPath);
        }
    }

    private string CopyPicture(string sourcePath)
    {
        string extension = sourcePath.GetExtension().ToLowerInvariant();
        if (DirAccess.MakeDirRecursiveAbsolute(PictureFolderPath) != Error.Ok)
        {
            GD.PushError($"Failed to create '{PictureFolderPath}'.");
            return null;
        }

        string targetPath = $"{PictureFolderPath}/team_{TeamData.Id}.{extension}";
        if (DirAccess.CopyAbsolute(sourcePath, targetPath) != Error.Ok)
        {
            GD.PushError($"Failed to copy '{sourcePath}' to '{targetPath}'.");
            return null;
        }

        return targetPath;
    }
}
