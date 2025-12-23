using Godot;
using System;

public partial class QuestionScreen : Control
{
    [Export] public Label GameModeLabel { get; set; }
    [Export] public Label QuestionTextLabel { get; set; }
    [Export] public MarginContainer QuestionImageContainer { get; set; }
    [Export] public GridContainer AnswersContainer { get; set; }
    [Export] public MarginContainer TeamCardContainer { get; set; }
    [Export] public PackedScene TeamCardScene { get; set; }
    [Export] public PackedScene AnswerBoxScene { get; set; }
    public override void _Ready()
    {
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.CurrentTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }
        UpdateGameModeLabel();
        UpdateQuestionData();
    }

    private void UpdateGameModeLabel()
    {
        var gameMode = GameManager.Instance.CurrentGameModeIndex;
        string modeText = gameMode switch
        {
            (int)GameManager.GameModeEnum.Betting => "OBSTAWIANIE",
            (int)GameManager.GameModeEnum.Sabotage => "SABOTAŻ",
            (int)GameManager.GameModeEnum.Fraud => "OSZUST",
            _ => "Unknown Mode"
        };
        GameModeLabel.Text = $"Game Mode: {modeText}";
    }
    private void UpdateQuestionData()
    {
        var question = QuestionsManager.Instance.RandomQuestion;
        if (question != null)
        {
            QuestionTextLabel.Text = question.QuestionText;
            LoadQuestionImage();
            foreach (var answer in question.AnswersText)
            {
                var answerBox = AnswerBoxScene.Instantiate<AnswerBox>();
                answerBox.AnswerText = answer;
                switch (question.AnswersText.IndexOf(answer))
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
                        answerBox.LetterText = question.AnswersText.IndexOf(answer).ToString();
                        break;
                }
                AnswersContainer.AddChild(answerBox);
            }
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
}
