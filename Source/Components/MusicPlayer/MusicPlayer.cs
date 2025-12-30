using Godot;
using System;

public partial class MusicPlayer : HBoxContainer
{
    [Export] public Button PlayButton { get; set; }
    [Export] public ProgressBar MusicProgressBar { get; set; }
    [Export] public Label MusicTimeLabel { get; set; }
    
    private AudioStreamPlayer _audioPlayer;
    
    public void SetAudio(AudioStream audio)
    {
        if (audio == null) return;
        
        _audioPlayer = new AudioStreamPlayer();
        _audioPlayer.Stream = audio;
        AddChild(_audioPlayer);
        
        if (MusicProgressBar != null)
        {
            MusicProgressBar.MaxValue = audio.GetLength();
            MusicProgressBar.Value = 0;
        }
        
        UpdateTimeLabel();
        
        if (PlayButton != null)
        {
            PlayButton.Pressed += OnPlayButtonPressed;
        }
        
        _audioPlayer.Finished += OnAudioFinished;
    }
    
    public override void _Process(double delta)
    {
        if (_audioPlayer != null && _audioPlayer.Playing)
        {
            if (MusicProgressBar != null)
            {
                MusicProgressBar.Value = _audioPlayer.GetPlaybackPosition();
            }
            UpdateTimeLabel();
        }
    }
    
    private void UpdateTimeLabel()
    {
        if (MusicTimeLabel == null || _audioPlayer == null) return;
        
        var currentTime = _audioPlayer.Playing ? _audioPlayer.GetPlaybackPosition() : 0;
        var totalTime = _audioPlayer.Stream.GetLength();
        
        var currentMinutes = (int)(currentTime / 60);
        var currentSeconds = (int)(currentTime % 60);
        var totalMinutes = (int)(totalTime / 60);
        var totalSeconds = (int)(totalTime % 60);
        
        MusicTimeLabel.Text = $"{currentMinutes}:{currentSeconds:D2}/{totalMinutes}:{totalSeconds:D2}";
    }
    
    private void OnPlayButtonPressed()
    {
        if (_audioPlayer == null) return;
        
        if (_audioPlayer.Playing)
        {
            _audioPlayer.Stop();
            PlayButton.Text = "▶ PLAY";
            MusicProgressBar.Value = 0;
            UpdateTimeLabel();
        }
        else
        {
            _audioPlayer.Play();
            PlayButton.Text = "⏹ STOP";
        }
    }
    
    private void OnAudioFinished()
    {
        if (PlayButton != null)
        {
            PlayButton.Text = "▶ PLAY";
        }
        if (MusicProgressBar != null)
        {
            MusicProgressBar.Value = 0;
        }
        UpdateTimeLabel();
    }
}
