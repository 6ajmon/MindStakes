using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CreatorScreen : Control
{
    private const string SelectButtonText = "SELECT";
    private const string ClearButtonText = "CLEAR";

    [Export] public LineEdit QuestionLineEdit { get; set; }
    [Export] public OptionButton CategoryOptionButton { get; set; }
    [Export] public Button ImageFileSelectButton { get; set; }
    [Export] public PanelContainer ImageFilePreviewContainer { get; set; }
    [Export] public Button AudioFileSelectButton { get; set; }
    [Export] public PanelContainer AudioFilePreviewContainer { get; set; }
    [Export] public LineEdit CorrectAnswerLineEdit { get; set; }
    [Export] public LineEdit Answer2LineEdit { get; set; }
    [Export] public LineEdit Answer3LineEdit { get; set; }
    [Export] public LineEdit Answer4LineEdit { get; set; }
    [Export] public LineEdit FunFactLineEdit { get; set; }
    [Export] public SpinBox PoolSpinBox { get; set; }
    [Export] public Button SaveButton { get; set; }
    [Export] public Button NewButton { get; set; }
    [Export] public PackedScene MusicPlayerScene { get; set; }
    [Export] public Tree QuestionsExplorerTree { get; set; }

    private readonly List<Category> _categories = new();
    private FileDialog _imageFileDialog;
    private FileDialog _audioFileDialog;
    private Texture2D _selectedImage;
    private AudioStream _selectedAudio;
    private string _editingQuestionPath;
    private HSplitContainer _questionsExplorerSplit;
    private bool _isQuestionsExplorerSplitResizeConnected;
    private ItemList _questionsFolderItemList;
    private readonly List<string> _currentFolderQuestionPaths = new();
    private string _openedFolderPath;
    private string _selectedQuestionPath;

    public override void _Ready()
    {
        LoadExistingCategories();
        ConfigurePoolSpinBox();
        SetupFileDialogs();
        BindUiEvents();
        SetupQuestionsExplorer();
        RefreshQuestionsExplorer();
        ResetForm();
    }

    private void LoadExistingCategories()
    {
        CategoryOptionButton.Clear();
        _categories.Clear();

        var categories = CategoriesManager.Instance.GetAllCategories();
        foreach (var category in categories)
        {
            _categories.Add(category);
            CategoryOptionButton.AddItem(category.CategoryName);
        }

        if (CategoryOptionButton.ItemCount > 0)
        {
            CategoryOptionButton.Select(0);
        }
    }

    private void ConfigurePoolSpinBox()
    {
        PoolSpinBox.MinValue = 1;
        PoolSpinBox.Step = 1;
        if (PoolSpinBox.Value < 1)
        {
            PoolSpinBox.Value = 1;
        }
    }

    private void SetupFileDialogs()
    {
        _imageFileDialog = new FileDialog
        {
            Name = "ImageFileDialog",
            Access = FileDialog.AccessEnum.Resources,
            FileMode = FileDialog.FileModeEnum.OpenFile,
            Title = "Select image",
            Filters = new string[]
            {
                "*.png ; PNG",
                "*.jpg, *.jpeg ; JPG/JPEG",
                "*.webp ; WEBP"
            }
        };
        _imageFileDialog.FileSelected += OnImageFileSelected;
        AddChild(_imageFileDialog);

        _audioFileDialog = new FileDialog
        {
            Name = "AudioFileDialog",
            Access = FileDialog.AccessEnum.Resources,
            FileMode = FileDialog.FileModeEnum.OpenFile,
            Title = "Select audio",
            Filters = new string[]
            {
                "*.mp3 ; MP3",
                "*.wav ; WAV",
                "*.ogg ; OGG"
            }
        };
        _audioFileDialog.FileSelected += OnAudioFileSelected;
        AddChild(_audioFileDialog);
    }

    private void BindUiEvents()
    {
        ImageFileSelectButton.Pressed += OnImageSelectPressed;
        AudioFileSelectButton.Pressed += OnAudioSelectPressed;
        SaveButton.Pressed += OnSavePressed;
        NewButton.Pressed += OnNewPressed;

        if (QuestionsExplorerTree != null)
        {
            QuestionsExplorerTree.ItemActivated += OnQuestionTreeItemActivated;
            QuestionsExplorerTree.ItemSelected += OnQuestionTreeItemSelected;
        }
    }

    private void SetupQuestionsExplorer()
    {
        if (QuestionsExplorerTree == null)
        {
            GD.PushWarning("QuestionsExplorerTree is not assigned.");
            return;
        }

        EnsureTwoColumnExplorerLayout();
        QuestionsExplorerTree.Columns = 1;
        QuestionsExplorerTree.HideRoot = true;

        if (_questionsExplorerSplit != null)
        {
            if (!_isQuestionsExplorerSplitResizeConnected)
            {
                _questionsExplorerSplit.Resized += OnQuestionsExplorerSplitResized;
                _isQuestionsExplorerSplitResizeConnected = true;
            }

            CallDeferred(MethodName.UpdateQuestionsExplorerSplitToHalf);
        }
    }

    private void EnsureTwoColumnExplorerLayout()
    {
        var currentParent = QuestionsExplorerTree.GetParent();
        if (currentParent == null)
        {
            return;
        }

        HSplitContainer splitContainer;
        if (currentParent is HSplitContainer parentSplit)
        {
            splitContainer = parentSplit;
        }
        else
        {
            splitContainer = currentParent.GetNodeOrNull<HSplitContainer>("QuestionsExplorerSplit");
            if (splitContainer == null)
            {
                splitContainer = new HSplitContainer
                {
                    Name = "QuestionsExplorerSplit",
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                    SizeFlagsVertical = Control.SizeFlags.ExpandFill
                };
                currentParent.AddChild(splitContainer);
            }

            QuestionsExplorerTree.Reparent(splitContainer);
        }

        _questionsExplorerSplit = splitContainer;

        QuestionsExplorerTree.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        QuestionsExplorerTree.SizeFlagsVertical = Control.SizeFlags.ExpandFill;
        QuestionsExplorerTree.CustomMinimumSize = new Vector2(180, 0);

        _questionsFolderItemList = splitContainer.GetNodeOrNull<ItemList>("QuestionsFolderItemList");
        if (_questionsFolderItemList == null)
        {
            _questionsFolderItemList = new ItemList
            {
                Name = "QuestionsFolderItemList",
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                SizeFlagsVertical = Control.SizeFlags.ExpandFill,
                SelectMode = ItemList.SelectModeEnum.Single,
                AllowRmbSelect = true
            };
            splitContainer.AddChild(_questionsFolderItemList);
            _questionsFolderItemList.ItemActivated += OnFolderListItemActivated;
            _questionsFolderItemList.ItemSelected += OnFolderListItemSelected;
        }
    }

    private void OnQuestionsExplorerSplitResized()
    {
        UpdateQuestionsExplorerSplitToHalf();
    }

    private void UpdateQuestionsExplorerSplitToHalf()
    {
        if (_questionsExplorerSplit == null)
        {
            return;
        }

        _questionsExplorerSplit.SplitOffsets[0] = (int)(_questionsExplorerSplit.Size.X * 0.5f);
    }

    private void RefreshQuestionsExplorer()
    {
        if (QuestionsExplorerTree == null)
        {
            return;
        }

        var preservedFolderPath = _openedFolderPath;
        var preservedQuestionPath = _selectedQuestionPath;

        QuestionsExplorerTree.Clear();
        var root = QuestionsExplorerTree.CreateItem();

        const string questionsRootPath = "res://Resources/Questions";
        var rootDir = DirAccess.Open(questionsRootPath);
        if (rootDir == null)
        {
            GD.PushWarning($"Failed to open '{questionsRootPath}' directory.");
            return;
        }

        var poolDirectories = new List<string>();
        rootDir.ListDirBegin();
        var entry = rootDir.GetNext();
        while (!string.IsNullOrEmpty(entry))
        {
            if (rootDir.CurrentIsDir() && entry.StartsWith("Pool", StringComparison.OrdinalIgnoreCase))
            {
                poolDirectories.Add(entry);
            }
            entry = rootDir.GetNext();
        }

        foreach (var poolDirectory in poolDirectories.OrderBy(GetPoolNumberFromName))
        {
            var poolItem = QuestionsExplorerTree.CreateItem(root);
            poolItem.SetText(0, poolDirectory);

            var poolPath = $"{questionsRootPath}/{poolDirectory}";
            poolItem.SetMetadata(0, poolPath);
            poolItem.Collapsed = poolPath != preservedFolderPath;

            var poolDir = DirAccess.Open(poolPath);
            if (poolDir == null)
            {
                continue;
            }

            var questionFiles = new List<string>();
            poolDir.ListDirBegin();
            var poolEntry = poolDir.GetNext();
            while (!string.IsNullOrEmpty(poolEntry))
            {
                if (!poolDir.CurrentIsDir() && poolEntry.EndsWith(".tres") && !poolEntry.EndsWith(".import"))
                {
                    questionFiles.Add(poolEntry);
                }
                poolEntry = poolDir.GetNext();
            }

            foreach (var questionFile in questionFiles.OrderBy(GetQuestionFileNumber))
            {
                var questionItem = QuestionsExplorerTree.CreateItem(poolItem);
                questionItem.SetText(0, questionFile);
                questionItem.SetMetadata(0, $"{poolPath}/{questionFile}");
            }
        }

        if (!string.IsNullOrEmpty(preservedFolderPath))
        {
            ShowFolderContents(preservedFolderPath);
            var pathToSelect = !string.IsNullOrEmpty(preservedQuestionPath) ? preservedQuestionPath : preservedFolderPath;
            var itemToSelect = FindTreeItemByPath(root, pathToSelect);
            itemToSelect?.Select(0);
        }
        else
        {
            ClearFolderList();
        }
    }

    private TreeItem FindTreeItemByPath(TreeItem parent, string path)
    {
        if (parent == null)
        {
            return null;
        }

        for (var item = parent.GetFirstChild(); item != null; item = item.GetNext())
        {
            var metadata = item.GetMetadata(0);
            if (metadata.VariantType != Variant.Type.Nil && metadata.AsString() == path)
            {
                return item;
            }

            var foundInChildren = FindTreeItemByPath(item, path);
            if (foundInChildren != null)
            {
                return foundInChildren;
            }
        }

        return null;
    }

    private int GetPoolNumberFromName(string poolName)
    {
        var numericPart = new string(poolName.Where(char.IsDigit).ToArray());
        return int.TryParse(numericPart, out var number) ? number : int.MaxValue;
    }

    private int GetQuestionFileNumber(string fileName)
    {
        var baseName = fileName.Replace(".tres", string.Empty);
        return int.TryParse(baseName, out var number) ? number : int.MaxValue;
    }

    private void OnQuestionTreeItemActivated()
    {
        var selectedItem = QuestionsExplorerTree?.GetSelected();
        if (selectedItem == null)
        {
            return;
        }

        if (!TryGetItemPath(selectedItem, out var itemPath))
        {
            return;
        }

        if (IsQuestionFilePath(itemPath))
        {
            LoadQuestionForEditing(itemPath);
            return;
        }

        selectedItem.Collapsed = !selectedItem.Collapsed;
        ShowFolderContents(itemPath);
    }

    private void OnQuestionTreeItemSelected()
    {
        var selectedItem = QuestionsExplorerTree?.GetSelected();
        if (selectedItem == null)
        {
            return;
        }

        if (!TryGetItemPath(selectedItem, out var itemPath))
        {
            return;
        }

        if (IsQuestionFilePath(itemPath))
        {
            var folderPath = GetParentFolderPath(itemPath);
            ShowFolderContents(folderPath);
            LoadQuestionForEditing(itemPath);
            return;
        }

        _selectedQuestionPath = null;
        ShowFolderContents(itemPath);
    }

    private bool TryGetItemPath(TreeItem item, out string path)
    {
        path = null;
        var metadata = item.GetMetadata(0);
        if (metadata.VariantType == Variant.Type.Nil)
        {
            return false;
        }

        path = metadata.AsString();
        return !string.IsNullOrEmpty(path);
    }

    private bool IsQuestionFilePath(string path)
    {
        return path.EndsWith(".tres", StringComparison.OrdinalIgnoreCase);
    }

    private string GetParentFolderPath(string path)
    {
        var slashIndex = path.LastIndexOf('/');
        return slashIndex > 0 ? path[..slashIndex] : path;
    }

    private void ShowFolderContents(string folderPath)
    {
        if (_questionsFolderItemList == null || string.IsNullOrEmpty(folderPath))
        {
            return;
        }

        _openedFolderPath = folderPath;

        _questionsFolderItemList.Clear();
        _currentFolderQuestionPaths.Clear();

        var dir = DirAccess.Open(folderPath);
        if (dir == null)
        {
            return;
        }

        var questionFiles = new List<string>();
        dir.ListDirBegin();
        var entry = dir.GetNext();
        while (!string.IsNullOrEmpty(entry))
        {
            if (!dir.CurrentIsDir() && entry.EndsWith(".tres") && !entry.EndsWith(".import"))
            {
                questionFiles.Add(entry);
            }
            entry = dir.GetNext();
        }

        foreach (var questionFile in questionFiles.OrderBy(GetQuestionFileNumber))
        {
            _questionsFolderItemList.AddItem(questionFile);
            _currentFolderQuestionPaths.Add($"{folderPath}/{questionFile}");
        }
    }

    private void OnFolderListItemActivated(long index)
    {
        if (index < 0 || index >= _currentFolderQuestionPaths.Count)
        {
            return;
        }

        LoadQuestionForEditing(_currentFolderQuestionPaths[(int)index]);
    }

    private void OnFolderListItemSelected(long index)
    {
        if (index < 0 || index >= _currentFolderQuestionPaths.Count)
        {
            return;
        }

        LoadQuestionForEditing(_currentFolderQuestionPaths[(int)index]);
    }

    private void ClearFolderList()
    {
        _questionsFolderItemList?.Clear();
        _currentFolderQuestionPaths.Clear();
        _openedFolderPath = null;
        _selectedQuestionPath = null;
    }

    private void LoadQuestionForEditing(string questionPath)
    {
        var question = ResourceLoader.Load(questionPath, string.Empty, ResourceLoader.CacheMode.Ignore) as Question;
        if (question == null)
        {
            GD.PushWarning($"Failed to load question from '{questionPath}'.");
            return;
        }

        _selectedQuestionPath = questionPath;
        _editingQuestionPath = questionPath;
        SaveButton.Text = "UPDATE";

        QuestionLineEdit.Text = question.QuestionText ?? string.Empty;

        var answers = question.AnswersText ?? new Godot.Collections.Array<string>();
        CorrectAnswerLineEdit.Text = answers.Count > 0 ? answers[0] : string.Empty;
        Answer2LineEdit.Text = answers.Count > 1 ? answers[1] : string.Empty;
        Answer3LineEdit.Text = answers.Count > 2 ? answers[2] : string.Empty;
        Answer4LineEdit.Text = answers.Count > 3 ? answers[3] : string.Empty;
        FunFactLineEdit.Text = question.FunFact ?? string.Empty;

        SetCategorySelection(question.Category);
        SetPoolFromQuestionPath(questionPath);

        _selectedImage = question.Photo;
        SetImagePreviewTexture(_selectedImage);

        _selectedAudio = question.Audio;
        ShowAudioPreview(_selectedAudio);

        UpdateFileButtonsState();
    }

    private void SetCategorySelection(Category category)
    {
        if (category == null)
        {
            return;
        }

        for (var i = 0; i < _categories.Count; i++)
        {
            if (_categories[i] == category || _categories[i].CategoryName == category.CategoryName)
            {
                CategoryOptionButton.Select(i);
                return;
            }
        }
    }

    private void SetPoolFromQuestionPath(string questionPath)
    {
        var marker = "/Pool";
        var markerStart = questionPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (markerStart < 0)
        {
            return;
        }

        var digitsStart = markerStart + marker.Length;
        var digits = string.Empty;
        while (digitsStart < questionPath.Length && char.IsDigit(questionPath[digitsStart]))
        {
            digits += questionPath[digitsStart];
            digitsStart++;
        }

        if (int.TryParse(digits, out var poolNumber))
        {
            PoolSpinBox.Value = poolNumber;
        }
    }

    private void OnImageSelectPressed()
    {
        if (_selectedImage != null)
        {
            ClearSelectedImage();
            return;
        }

        _imageFileDialog.PopupCenteredRatio(0.65f);
    }

    private void OnAudioSelectPressed()
    {
        if (_selectedAudio != null)
        {
            ClearSelectedAudio();
            return;
        }

        _audioFileDialog.PopupCenteredRatio(0.65f);
    }

    private void OnImageFileSelected(string path)
    {
        _selectedImage = GD.Load<Texture2D>(path);
        if (_selectedImage == null)
        {
            GD.PushWarning($"Failed to load image from '{path}'.");
            return;
        }

        SetImagePreviewTexture(_selectedImage);
        UpdateFileButtonsState();
    }

    private void OnAudioFileSelected(string path)
    {
        _selectedAudio = GD.Load<AudioStream>(path);
        if (_selectedAudio == null)
        {
            GD.PushWarning($"Failed to load audio from '{path}'.");
            return;
        }

        ShowAudioPreview(_selectedAudio);
        UpdateFileButtonsState();
    }

    private void ClearSelectedImage()
    {
        _selectedImage = null;
        SetImagePreviewTexture(null);
        UpdateFileButtonsState();
    }

    private void ClearSelectedAudio()
    {
        _selectedAudio = null;
        ShowAudioPreview(null);
        UpdateFileButtonsState();
    }

    private void UpdateFileButtonsState()
    {
        if (ImageFileSelectButton != null)
        {
            ImageFileSelectButton.Text = _selectedImage == null ? SelectButtonText : ClearButtonText;
        }

        if (AudioFileSelectButton != null)
        {
            AudioFileSelectButton.Text = _selectedAudio == null ? SelectButtonText : ClearButtonText;
        }
    }

    private void OnSavePressed()
    {
        if (!ValidateForm())
        {
            return;
        }

        var selectedCategory = GetSelectedCategory();
        if (selectedCategory == null)
        {
            GD.PushWarning("No category selected.");
            return;
        }

        var poolNumber = (int)Math.Round(PoolSpinBox.Value);
        var poolPath = $"res://Resources/Questions/Pool{poolNumber}";

        EnsurePoolDirectoryExists(poolPath);

        var question = new Question
        {
            QuestionText = QuestionLineEdit.Text.Trim(),
            AnswersText = BuildAnswers(),
            IsClosedQuestion = true,
            IsBettingGameQuestion = true,
            IsSabotageGameQuestion = true,
            IsFraudGameQuestion = true,
            Category = selectedCategory,
            FunFact = FunFactLineEdit.Text.Trim(),
            Photo = _selectedImage,
            Audio = _selectedAudio
        };

        var savePath = _editingQuestionPath;
        if (string.IsNullOrEmpty(savePath))
        {
            var nextQuestionFileName = GetNextQuestionFileName(poolPath);
            savePath = $"{poolPath}/{nextQuestionFileName}.tres";
        }

        var error = ResourceSaver.Save(question, savePath);
        if (error != Error.Ok)
        {
            GD.PushWarning($"Failed to save question at '{savePath}'. Error: {error}");
            return;
        }

        _openedFolderPath = poolPath;
        _selectedQuestionPath = savePath;

        GD.Print($"Question saved to {savePath}");
        RefreshQuestionsExplorer();
    }

    private void OnNewPressed()
    {
        ResetForm();
    }

    private void SetImagePreviewTexture(Texture2D texture)
    {
        var imagePreview = ImageFilePreviewContainer.GetNodeOrNull<TextureRect>("TextureRect");
        if (imagePreview == null)
        {
            return;
        }

        imagePreview.Texture = texture;
        imagePreview.ExpandMode = TextureRect.ExpandModeEnum.FitWidthProportional;
        imagePreview.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
    }

    private void ShowAudioPreview(AudioStream audio)
    {
        ClearAudioPreview();
        if (audio == null)
        {
            return;
        }

        if (MusicPlayerScene == null)
        {
            GD.PushWarning("MusicPlayerScene is not assigned.");
            return;
        }

        var musicPlayer = MusicPlayerScene.Instantiate<MusicPlayer>();
        AudioFilePreviewContainer.AddChild(musicPlayer);
        musicPlayer.SetAudio(audio);
    }

    private void ClearAudioPreview()
    {
        foreach (var child in AudioFilePreviewContainer.GetChildren())
        {
            if (child is MusicPlayer)
            {
                child.QueueFree();
            }
        }

        var audioTexturePlaceholder = AudioFilePreviewContainer.GetNodeOrNull<TextureRect>("TextureRect");
        if (audioTexturePlaceholder != null)
        {
            audioTexturePlaceholder.Texture = null;
        }
    }

    private void EnsurePoolDirectoryExists(string poolPath)
    {
        var questionsRootPath = "res://Resources/Questions";
        var poolFolderName = $"Pool{(int)Math.Round(PoolSpinBox.Value)}";

        var questionsRootDir = DirAccess.Open(questionsRootPath);
        if (questionsRootDir == null)
        {
            GD.PushWarning($"Failed to open '{questionsRootPath}' directory.");
            return;
        }

        var makeDirError = questionsRootDir.MakeDirRecursive(poolFolderName);
        if (makeDirError != Error.Ok)
        {
            GD.PushWarning($"Failed to create folder '{poolPath}'. Error: {makeDirError}");
        }
    }

    private int GetNextQuestionFileName(string poolPath)
    {
        var highestNumber = 0;
        var dir = DirAccess.Open(poolPath);
        if (dir == null)
        {
            return 1;
        }

        dir.ListDirBegin();
        var fileName = dir.GetNext();
        while (!string.IsNullOrEmpty(fileName))
        {
            if (!dir.CurrentIsDir() && fileName.EndsWith(".tres") && !fileName.EndsWith(".import"))
            {
                var baseName = fileName.Replace(".tres", string.Empty);
                if (int.TryParse(baseName, out var parsedNumber) && parsedNumber > highestNumber)
                {
                    highestNumber = parsedNumber;
                }
            }
            fileName = dir.GetNext();
        }

        return highestNumber + 1;
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(QuestionLineEdit.Text))
        {
            GD.PushWarning("Question field cannot be empty.");
            return false;
        }

        if (CategoryOptionButton == null || CategoryOptionButton.ItemCount == 0 || CategoryOptionButton.Selected < 0)
        {
            GD.PushWarning("Category must be selected.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(CorrectAnswerLineEdit.Text)
            || string.IsNullOrWhiteSpace(Answer2LineEdit.Text)
            || string.IsNullOrWhiteSpace(Answer3LineEdit.Text)
            || string.IsNullOrWhiteSpace(Answer4LineEdit.Text))
        {
            GD.PushWarning("All answer fields are required.");
            return false;
        }

        return true;
    }

    private Category GetSelectedCategory()
    {
        var selectedIndex = CategoryOptionButton.Selected;
        if (selectedIndex < 0 || selectedIndex >= _categories.Count)
        {
            return null;
        }

        return _categories[selectedIndex];
    }

    private Godot.Collections.Array<string> BuildAnswers()
    {
        var answers = new Godot.Collections.Array<string>();
        AddIfNotEmpty(answers, CorrectAnswerLineEdit.Text);
        AddIfNotEmpty(answers, Answer2LineEdit.Text);
        AddIfNotEmpty(answers, Answer3LineEdit.Text);
        AddIfNotEmpty(answers, Answer4LineEdit.Text);
        return answers;
    }

    private void AddIfNotEmpty(Godot.Collections.Array<string> answers, string value)
    {
        var trimmed = value?.Trim();
        if (!string.IsNullOrEmpty(trimmed))
        {
            answers.Add(trimmed);
        }
    }

    private void ResetForm()
    {
        QuestionLineEdit.Text = string.Empty;
        CorrectAnswerLineEdit.Text = string.Empty;
        Answer2LineEdit.Text = string.Empty;
        Answer3LineEdit.Text = string.Empty;
        Answer4LineEdit.Text = string.Empty;
        FunFactLineEdit.Text = string.Empty;
        _editingQuestionPath = null;
        SaveButton.Text = "SAVE";

        _selectedImage = null;
        _selectedAudio = null;

        SetImagePreviewTexture(null);
        ClearAudioPreview();
        UpdateFileButtonsState();
    }
}
