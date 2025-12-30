using Godot;

/// <summary>
/// Komponent dodający efekt Balatro (śledzenie kursora + delikatny ruch) do Control nodes.
/// Dodaj ten skrypt do Label, Button lub innego Control z ShaderMaterial.
/// </summary>
public partial class PerspectiveShader : Label
{
    [Export] public float HoverIntensity { get; set; } = 2.0f;
    [Export] public float IdleWobbleSpeed { get; set; } = 0.6f;
    [Export] public float IdleWobbleAmount { get; set; } = 0.06f;
    [Export] public float MaxRotation { get; set; } = 6.0f;
    
    private ShaderMaterial _shaderMaterial;
    private bool _isHovered = false;
    private float _time = 0.0f;
    
    public override void _Ready()
    {
        // Pobierz ShaderMaterial z tego node'a lub jego dziecka
        _shaderMaterial = GetShaderMaterial();
        
        if (_shaderMaterial == null)
        {
            GD.PrintErr("BalatroEffect: Nie znaleziono ShaderMaterial!");
            return;
        }
        
        // Ustaw początkowe wartości
        _shaderMaterial.SetShaderParameter("hover_intensity", HoverIntensity);
        _shaderMaterial.SetShaderParameter("idle_wobble_speed", IdleWobbleSpeed);
        _shaderMaterial.SetShaderParameter("idle_wobble_amount", IdleWobbleAmount);
        _shaderMaterial.SetShaderParameter("max_rotation", MaxRotation);
        
        // Podłącz sygnały hover
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    
    public override void _Process(double delta)
    {
        if (_shaderMaterial == null) return;
        
        // Aktualizuj czas dla animacji idle
        _time += (float)delta;
        _shaderMaterial.SetShaderParameter("time", _time);
        _shaderMaterial.SetShaderParameter("is_hovered", _isHovered);
        
        if (_isHovered)
        {
            // Oblicz pozycję kursora względem tego elementu (0-1)
            Vector2 mousePos = GetLocalMousePosition();
            Vector2 size = Size;
            
            if (size.X > 0 && size.Y > 0)
            {
                Vector2 normalizedPos = new Vector2(
                    Mathf.Clamp(mousePos.X / size.X, 0, 1),
                    Mathf.Clamp(mousePos.Y / size.Y, 0, 1)
                );
                _shaderMaterial.SetShaderParameter("mouse_pos", normalizedPos);
            }
        }
    }
    
    private void OnMouseEntered()
    {
        _isHovered = true;
    }
    
    private void OnMouseExited()
    {
        _isHovered = false;
        // Reset pozycji kursora do środka
        _shaderMaterial?.SetShaderParameter("mouse_pos", new Vector2(0.5f, 0.5f));
    }
    
    private ShaderMaterial GetShaderMaterial()
    {
        // Sprawdź bezpośrednio na tym node
        if (Material is ShaderMaterial sm)
            return sm;
        
        // Sprawdź dzieci (np. Label wewnątrz Panel)
        foreach (var child in GetChildren())
        {
            if (child is CanvasItem ci && ci.Material is ShaderMaterial childSm)
                return childSm;
        }
        
        return null;
    }
}
