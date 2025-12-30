using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<GameManager>("GameManager");

    public enum GameModeEnum
    {
        Betting = 0,
        Sabotage = 1,
        Fraud = 2
    }

    public int CurrentGameModeIndex { get; set; } = 0;

    public int RoundCount { get; set; } = 0;
    public int MaxRounds { get; set; } = 10;

    public int MaxBetAmount { get; set; } = 20;
    public float StartingScore { get; set; } = 100f;
    [Export] public int InitialRerollCount { get; set; } = 1;
    public int RerollCount { get; set; } = 1;
    [Export] public int QuestionTime { get; set; } = 6;

    public ulong GameSeed { get; private set; }

    public override void _Ready()
    {
        GameSeed = (ulong)DateTime.Now.Ticks;
        GD.Seed(GameSeed);

        RerollCount = InitialRerollCount;

        SignalManager.Instance.TeamsCreated += OnTeamsCreated;
    }
    public void OnTeamsCreated()
    {
        TeamsManager.Instance.CurrentTeam = TeamsManager.Instance.Teams[0];
        TeamsManager.Instance.InitialTeam = TeamsManager.Instance.Teams[0];
    }

    public void NextRound()
    {
        RoundCount++;
        if (RoundCount >= MaxRounds) 
        {
            FinishGame();
        }
    }

    private void FinishGame()
    {
        SceneManager.Instance.ChangeScene("res://Source/Screens/GameModePick/GameModePickScreen.tscn");
        RoundCount = 10;
    }
}
