using Godot;
using System;

public partial class CategoryPickScreen : Control
{
    [Export] MarginContainer TeamCardContainer;
    [Export] PackedScene TeamCardScene;
    [Export] Panel CategoryButtonContainer;
    [Export] PackedScene CategoryButtonScene;
    [Export] Button RerollButton;
    public override void _Ready()
    {
        if (TeamCardContainer != null)
        {
            TeamCard teamCard = TeamCardScene.Instantiate<TeamCard>();
            teamCard.TeamData = TeamsManager.Instance.InitialTeam;
            TeamCardContainer.AddChild(teamCard);
            teamCard.TeamNameLineEdit.Editable = false;
        }
        if (CategoryButtonContainer != null)
        {
            CategoryButton categoryButton = CategoryButtonScene.Instantiate<CategoryButton>();
            categoryButton.Category = CategoriesManager.Instance.RandomCategory;
            CategoryButtonContainer.AddChild(categoryButton, true);
        }
        if (RerollButton != null)
        {
            RerollButton.Text = $"Reroll ({GameManager.Instance.RerollCount})";
        }
    }

    public override void _Process(double delta)
    {
        if (RerollButton != null)
        {
            RerollButton.Disabled = GameManager.Instance.RerollCount <= 0;
        }
    }

    public void OnRerollButtonPressed()
    {
        if (GameManager.Instance.RerollCount <= 0)
            return;
        CategoriesManager.Instance.SetRandomCategory();
        GameManager.Instance.RerollCount--;
        if (CategoryButtonContainer != null)
        {
            CategoryButtonContainer.GetChild(0).QueueFree();

            CategoryButton categoryButton = CategoryButtonScene.Instantiate<CategoryButton>();
            categoryButton.Category = CategoriesManager.Instance.RandomCategory;
            CategoryButtonContainer.AddChild(categoryButton, true);
            categoryButton.UpdateButton();

        }
        if (RerollButton != null)
        {
            RerollButton.Text = $"Reroll ({GameManager.Instance.RerollCount})";
        }
    }
}
