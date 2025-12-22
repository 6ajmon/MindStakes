using Godot;
using System;

[GlobalClass]
public partial class Category : Resource 
{
    [Export]
    public string CategoryName { get; set; }
    [Export]
    public bool IsEnabled { get; set; } = true;
    [Export]
    public Texture2D Icon { get; set; }
}
