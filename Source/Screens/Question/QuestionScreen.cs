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
    [Export] public GridContainer AnswersContainer { get; set; }
    [Export] public MarginContainer TeamCardContainer { get; set; }
    [Export] public PackedScene TeamCardScene { get; set; }
    [Export] public PackedScene AnswerBoxScene { get; set; }
    [Export] public Button CheckButton { get; set; }
    [Export] public Button ResetTimerButton { get; set; }
    [Export] public float TotalTime { get; set; } = 25.0f;
    [Export] public bool AutostartTimer { get; set; } = false;
    
    private AnswerBox _correctAnswerBox;
    private float _timeLeft;
    private bool _isTimerRunning;

    public override void _Ready()
    {
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }

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

    public async void RevealAnswer()
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
            QuestionImageContainer.AddChild(textureRect);
        }
    }
    private void OnCheckButtonPressed()
    {
        RevealAnswer();
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
