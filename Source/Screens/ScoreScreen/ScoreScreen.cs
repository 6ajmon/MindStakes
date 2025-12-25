 using Godot;
using System;

public partial class ScoreScreen : Control
{
    [Export] public PackedScene ScoreTeamCardScene;
    [Export] public HBoxContainer TeamCardsContainer;
    [Export] public NextButton ReadyButton;
    public override void _Ready()
    {
        LoadTeamCards();
        CheckGameEnd();
    }

    private void CheckGameEnd()
    {
        if (GameManager.Instance.RoundCount + 1 >= GameManager.Instance.MaxRounds)
        {
            if (ReadyButton != null)
            {
                ReadyButton.ScenePath = "";
            }
        }
    }

    public void LoadTeamCards()
    {
        for (int i = 0; i < TeamsManager.Instance.Teams.Count; i++)
        {
            AddTeamCard(TeamsManager.Instance.Teams[i]);
        }
    }

    public void AddTeamCard(Team newTeam)
    {
        ScoreTeamCard teamCard = ScoreTeamCardScene.Instantiate<ScoreTeamCard>();
        teamCard.TeamData = newTeam;
        TeamCardsContainer.AddChild(teamCard);
        teamCard.UpdateButtonsAndText();
    }

    private void OnReadyButtonPressed()
    {
        QuestionsManager.Instance.RemoveCurrentQuestion();
        GameManager.Instance.RerollCount = GameManager.Instance.InitialRerollCount;
        GameManager.Instance.NextRound();
        TeamsManager.Instance.NextInitialTeam();
        TeamsManager.Instance.ResetBets();
        CategoriesManager.Instance.SetRandomCategory();
        QuestionsManager.Instance.SetRandomQuestion();
    }
}
