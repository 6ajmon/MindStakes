using Godot;
using System;
using System.Collections.Generic;using System.Linq;
public partial class CategoriesManager : Node
{
    public static CategoriesManager Instance => ((SceneTree)Engine.GetMainLoop()).Root.GetNode<CategoriesManager>("CategoriesManager");

    public List<Category> Categories { get; private set; } = new();
    public Category RandomCategory {get; private set; }
    public int RerollsLeft { get; set; } = 166;
    public override void _Ready()
    {
        LoadCategories();
        SetRandomCategory();
    }
    private void LoadCategories()
    {
        var dir = DirAccess.Open("res://Resources/Categories");
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (!dir.CurrentIsDir())
                {
                    // Check for resource files (usually .tres or .res in Godot)
                    // Also avoiding .import files
                    if ((fileName.EndsWith(".tres") || fileName.EndsWith(".res")) && !fileName.EndsWith(".import"))
                    {
                        var category = GD.Load<Category>($"res://Resources/Categories/{fileName}");
                        if (category != null)
                        {
                            Categories.Add(category);
                        }
                    }
                }
                fileName = dir.GetNext();
            }
        }
        else
        {
            GD.PrintErr("Failed to open Resources/Categories directory.");
        }
    }

    public void SetRandomCategory()
    {
        var validCategories = Categories.Where(c => QuestionsManager.Instance.HasQuestionsForCategory(c)).ToList();

        if (validCategories.Count == 0 || RerollsLeft <= 0)
        {
            RandomCategory = null;
            return;
        }

        if (RandomCategory != null && validCategories.Count > 1)
        {
            var available = validCategories.Where(c => c != RandomCategory).ToList();
            var idx = GD.Randi() % available.Count;
            RandomCategory = available[(int)idx];
        }
        else
        {
            var randomIndex = GD.Randi() % validCategories.Count;
            RandomCategory = validCategories[(int)randomIndex];
        }
    }
}
