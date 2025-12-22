using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{
    public static SceneManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<SceneManager>("SceneManager");
    public bool IsMenuOpen { get; set; }
    public Node CurrentMenu { get; set; }
    public List<string> SceneHistory { get; set; } = new List<string>();
    public Node CurrentScene { get; set; }
    public int CurrentSceneIndex { get; set; }
    private bool isFirstSceneRegistered = false;
    public void RegisterFirstScene()
    {
        if (isFirstSceneRegistered)
        {
            return;
        }
        CurrentScene = GetTree().CurrentScene;
        if (CurrentScene != null)
        {
            SceneHistory.Add(CurrentScene.SceneFilePath);
            CurrentSceneIndex = 0;
        }
        isFirstSceneRegistered = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("GoBack"))
        {
            GoBack();
        }
        if (@event.IsActionPressed("GoForward"))
        {
            GoForward();
        }
    }

    public void ReplaceWithNewScene(string newScenePath)
    {
        CurrentSceneIndex += 1;
        if (CurrentSceneIndex < SceneHistory.Count - 1)
        {
            SceneHistory.RemoveRange(CurrentSceneIndex + 1, SceneHistory.Count - CurrentSceneIndex - 1);
        }
        SceneHistory.Add(newScenePath);
        
        ChangeScene(newScenePath);
    }

    public void ChangeScene(string newScenePath)
    {
        if (IsMenuOpen)
        {
            CurrentMenu?.QueueFree();
            CurrentMenu = null;
            IsMenuOpen = false;
        }

        GetTree().Paused = false;

        GetTree().ChangeSceneToFile(newScenePath);
    }

    public void GoBack()
    {
        if (SceneHistory.Count > 0 && CurrentSceneIndex > 0)
        {
            CurrentSceneIndex -= 1;
            string previousScenePath = SceneHistory[CurrentSceneIndex];
            ChangeScene(previousScenePath);
        }
    }

    public void GoForward()
    {
        if (SceneHistory.Count > 0 && CurrentSceneIndex < SceneHistory.Count - 1)
        {
            CurrentSceneIndex += 1;
            string nextScenePath = SceneHistory[CurrentSceneIndex];
            ChangeScene(nextScenePath);
        }
    }

    public void AddChildScene(string scenePath)
    {
        PackedScene scene = ResourceLoader.Load<PackedScene>(scenePath);
        if (scene != null)
        {
            Node instance = scene.Instantiate();
            if (instance != null)
            {
                GetTree().Root.AddChild(instance);
                CurrentMenu = instance;
                IsMenuOpen = true;
            }
            else
            {
                GD.PrintErr($"Failed to instantiate scene at {scenePath}");
            }
        }
        else
        {
            GD.PrintErr($"Failed to load scene at {scenePath}");
        }
    }
}