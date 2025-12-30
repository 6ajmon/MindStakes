using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class QuestionScreen : Control
{
    [Export] public ProgressBar TimeProgressBar { get; set; }
    [Export] public Label TimeLabel { get; set; }
    [Export] public Label CategoryLabel { get; set; }
    [Export] public Label QuestionTextLabel { get; set; }
    [Export] public MarginContainer QuestionImageContainer { get; set; }
    [Export] public MarginContainer QuestionPlayerContainer { get; set; }
    [Export] public GridContainer AnswersContainer { get; set; }
    [Export] public PackedScene MusicPlayerScene { get; set; }
    [Export] public PackedScene AnswerBoxScene { get; set; }
    [Export] public Button CheckButton { get; set; }
    [Export] public Button ResetTimerButton { get; set; }
    [Export] public float TotalTime { get; set; } = 25.0f;
    [Export] public bool AutostartTimer { get; set; } = false;
    
    private AnswerBox _correctAnswerBox;
    private float _timeLeft;
    private bool _isTimerRunning;
    private ColorRect _funFactOverlay;
    private Label _funFactLabel;

    public override void _Ready()
    {
        TotalTime = GameManager.Instance.QuestionTime;
        _timeLeft = TotalTime;
        _isTimerRunning = AutostartTimer;
        if (TimeProgressBar != null)
        {
            TimeProgressBar.MaxValue = TotalTime;
            TimeProgressBar.Value = _timeLeft;
            TimeProgressBar.ShowPercentage = false;
            
            UpdateTimerLabel();
        }
        UpdateCategoryLabel();

        UpdateQuestionData();
        LoadQuestionAudio();
        
        CheckButton.Pressed += OnCheckButtonPressed;
        ResetTimerButton.Pressed += OnResetTimerButtonPressed;
    }

    public override void _Process(double delta)
    {
        if (_isTimerRunning && _timeLeft > 0)
        {
            _timeLeft -= (float)delta;
            if (_timeLeft < 0) _timeLeft = 0;
            
            if (TimeProgressBar != null)
            {
                TimeProgressBar.Value = _timeLeft;
                UpdateTimerLabel();
            }
        }
    }

    private void UpdateCategoryLabel()
    {
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null && CategoryLabel != null)
        {
            CategoryLabel.Text = "KATEGORIA: " + question.Category.CategoryName;
        }
    }

    private void UpdateTimerLabel()
    {
        if (TimeLabel != null)
        {
            TimeLabel.Text = $"{Mathf.Ceil(_timeLeft)}s";
        }
    }

    private void UpdateQuestionData()
    {
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null)
        {
            QuestionTextLabel.Text = question.QuestionText;
            LoadQuestionImage();

            // Create a list of answers to shuffle
            var answers = new List<string>(question.AnswersText);
            
            // Shuffle the answers
            var rng = new Random();
            int n = answers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = answers[k];
                answers[k] = answers[n];
                answers[n] = value;
            }

            int index = 0;
            foreach (var answer in answers)
            {
                var answerBox = AnswerBoxScene.Instantiate<AnswerBox>();
                answerBox.AnswerText = answer;
                switch (index)
                {
                    case 0:
                        answerBox.LetterText = "A";
                        break;
                    case 1:
                        answerBox.LetterText = "B";
                        break;
                    case 2:
                        answerBox.LetterText = "C";
                        break;
                    case 3:
                        answerBox.LetterText = "D";
                        break;
                    default:
                        answerBox.LetterText = ((char)('A' + index)).ToString();
                        break;
                }
                AnswersContainer.AddChild(answerBox);
                
                if (answer == question.AnswersText[0])
                {
                    _correctAnswerBox = answerBox;
                }

                index++;
            }
        }
    }

    public void RevealAnswer()
    {
        if (_correctAnswerBox != null)
        {
            _correctAnswerBox.MarkAsCorrect();
        }
    }

    private void LoadQuestionImage()
    {
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null && question.Photo != null)
        {
            TextureRect textureRect = new TextureRect();
            textureRect.Texture = question.Photo;
            textureRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
            textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
            QuestionImageContainer.AddChild(textureRect);

            if (QuestionTextLabel != null)
            {
                QuestionTextLabel.AddThemeFontSizeOverride("font_size", 48);
            }
        }
        else
        {
            QuestionImageContainer.SizeFlagsStretchRatio = 0;
        }
    }

    private void LoadQuestionAudio()
    {
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null && question.Audio != null && MusicPlayerScene != null)
        {
            var musicPlayer = MusicPlayerScene.Instantiate<MusicPlayer>();
            musicPlayer.SetAudio(question.Audio);
            QuestionPlayerContainer.AddChild(musicPlayer);
        }
        else
        {
            QuestionPlayerContainer.SizeFlagsStretchRatio = 0;
        }
    }
    private void OnCheckButtonPressed()
    {
        RevealAnswer();

        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null && !string.IsNullOrWhiteSpace(question.FunFact))
        {
            CheckButton.Text = "CIEKAWOSTKA";
            CheckButton.Pressed -= OnCheckButtonPressed;
            CheckButton.Pressed += OnFunFactButtonPressed;
        }
    }

    private void OnFunFactButtonPressed()
    {
        if (_funFactOverlay == null) SetupFunFactUI();
        
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null)
        {
            _funFactLabel.Text = question.FunFact;
            _funFactOverlay.Visible = true;
        }
    }

    private void SetupFunFactUI()
    {
        _funFactOverlay = new ColorRect();
        _funFactOverlay.Name = "FunFactOverlay";
        _funFactOverlay.Color = new Color(0, 0, 0, 0.8f);
        _funFactOverlay.SetAnchorsPreset(LayoutPreset.FullRect);
        _funFactOverlay.Visible = false;
        _funFactOverlay.ZIndex = 100;
        AddChild(_funFactOverlay);

        var panel = new PanelContainer();
        panel.SetAnchorsPreset(LayoutPreset.Center);
        panel.AnchorLeft = 0.5f;
        panel.AnchorTop = 0.5f;
        panel.AnchorRight = 0.5f;
        panel.AnchorBottom = 0.5f;
        panel.GrowHorizontal = GrowDirection.Both;
        panel.GrowVertical = GrowDirection.Both;
        panel.CustomMinimumSize = new Vector2(800, 0);
        _funFactOverlay.AddChild(panel);

        var marginContainer = new MarginContainer();
        marginContainer.AddThemeConstantOverride("margin_top", 20);
        marginContainer.AddThemeConstantOverride("margin_bottom", 20);
        marginContainer.AddThemeConstantOverride("margin_left", 20);
        marginContainer.AddThemeConstantOverride("margin_right", 20);
        panel.AddChild(marginContainer);

        var vbox = new VBoxContainer();
        vbox.AddThemeConstantOverride("separation", 20);
        marginContainer.AddChild(vbox);

        var titleLabel = new Label();
        titleLabel.Text = "CIEKAWOSTKA";
        titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
        titleLabel.AddThemeFontSizeOverride("font_size", 42);
        vbox.AddChild(titleLabel);

        _funFactLabel = new Label();
        _funFactLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _funFactLabel.HorizontalAlignment = HorizontalAlignment.Center;
        _funFactLabel.AddThemeFontSizeOverride("font_size", 28);
        vbox.AddChild(_funFactLabel);

        var closeButton = new Button();
        closeButton.Text = "ZAMKNIJ";
        closeButton.CustomMinimumSize = new Vector2(200, 60);
        closeButton.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        closeButton.Pressed += () => _funFactOverlay.Visible = false;
        vbox.AddChild(closeButton);
    }
    private void OnResetTimerButtonPressed()
    {
        _timeLeft = TotalTime;
        _isTimerRunning = true;
        if (TimeProgressBar != null)
        {
            TimeProgressBar.Value = _timeLeft;
            UpdateTimerLabel();
        }
    }
}
