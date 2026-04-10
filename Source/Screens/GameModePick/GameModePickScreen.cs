using Godot;
using System;

public partial class GameModePickScreen : Control
{
	[Export] public SpinBox RerollCountSpinBox { get; set; }
	[Export] public SpinBox PoolNumberSpinBox { get; set; }
	[Export] public SpinBox QuestionTimeSpinBox { get; set; }
	public override void _Ready()
	{
		RerollCountSpinBox.Value = GameManager.Instance.InitialRerollCount;
		QuestionTimeSpinBox.Value = GameManager.Instance.QuestionTime;
		
		// Setup PoolNumberSpinBox
		var maxPoolNumber = QuestionsManager.Instance.GetMaxPoolNumber();
		PoolNumberSpinBox.MinValue = 1;
		PoolNumberSpinBox.MaxValue = maxPoolNumber;
		PoolNumberSpinBox.Value = GameManager.Instance.SelectedPoolNumber;
		
		// Load questions from the selected pool
		QuestionsManager.Instance.LoadQuestionsFromPool(GameManager.Instance.SelectedPoolNumber);
		
		RerollCountSpinBox.ValueChanged += OnRerollCountSpinBoxValueChanged;
		QuestionTimeSpinBox.ValueChanged += OnQuestionTimeSpinBoxValueChanged;
		PoolNumberSpinBox.ValueChanged += OnPoolNumberSpinBoxValueChanged;
	}

	public override void _ExitTree()
	{
		GameManager.Instance.InitialRerollCount = (int)RerollCountSpinBox.Value;
		GameManager.Instance.RerollCount = (int)RerollCountSpinBox.Value;
		GameManager.Instance.QuestionTime = (int)QuestionTimeSpinBox.Value;
		GameManager.Instance.SelectedPoolNumber = (int)PoolNumberSpinBox.Value;

		RerollCountSpinBox.ValueChanged -= OnRerollCountSpinBoxValueChanged;
		QuestionTimeSpinBox.ValueChanged -= OnQuestionTimeSpinBoxValueChanged;
		PoolNumberSpinBox.ValueChanged -= OnPoolNumberSpinBoxValueChanged;
	}

	private void OnRerollCountSpinBoxValueChanged(double value)
	{
		GameManager.Instance.InitialRerollCount = (int)value;
	}
	private void OnQuestionTimeSpinBoxValueChanged(double value)
	{
		GameManager.Instance.QuestionTime = (int)value;
	}
	private void OnPoolNumberSpinBoxValueChanged(double value)
	{
		var poolNumber = (int)value;
		GameManager.Instance.SelectedPoolNumber = poolNumber;
		QuestionsManager.Instance.LoadQuestionsFromPool(poolNumber);
	}
}
