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

    public ulong GameSeed { get; private set; }

    public override void _Ready()
    {
        GameSeed = (ulong)DateTime.Now.Ticks;
        GD.Seed(GameSeed);

        SignalManager.Instance.TeamsCreated += OnTeamsCreated;
    }
    public void OnTeamsCreated()
    {
        TeamsManager.Instance.CurrentTeam = TeamsManager.Instance.Teams[0];
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
