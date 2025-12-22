using Godot;
using System;

public partial class GameManager : Node
{
    public static GameManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<GameManager>("GameManager");

    public enum GameModes
    {
        Betting = 0,
        Sabotage = 1,
        Fraud = 2
    }

    public int CurrentGameModeIndex { get; set; } = 0;

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
}
