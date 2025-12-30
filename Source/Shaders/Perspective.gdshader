shader_type canvas_item;

// Ustawienia efektu Balatro - delikatne wartości
uniform float max_skew : hint_range(0.0, 0.05) = 0.008; // Siła efektu skew/perspektywy
uniform float hover_intensity : hint_range(0.0, 2.0) = 0.6; // Intensywność efektu hover
uniform float idle_wobble_speed : hint_range(0.0, 3.0) = 0.8; // Prędkość delikatnego ruchu
uniform float idle_wobble_amount : hint_range(0.0, 0.02) = 0.004; // Siła delikatnego ruchu
uniform float scale_on_hover : hint_range(1.0, 1.1) = 1.01; // Lekkie powiększenie na hover

// Pozycja kursora (0-1 względem elementu)
uniform vec2 mouse_pos = vec2(0.5, 0.5);
// Czy kursor jest nad elementem
uniform bool is_hovered = false;
// Czas dla animacji idle
uniform float time = 0.0;

void vertex() {
    vec2 center_offset = UV - vec2(0.5);
    
    float skew_x = 0.0;
    float skew_y = 0.0;
    float current_scale = 1.0;
    
    if (is_hovered) {
        vec2 mouse_offset = (mouse_pos - vec2(0.5)) * 2.0;
        skew_x = mouse_offset.x * max_skew * hover_intensity;
        skew_y = mouse_offset.y * max_skew * hover_intensity;
        current_scale = scale_on_hover;
    } else {
        // Bardzo delikatny ruch idle
        skew_x = sin(time * idle_wobble_speed) * idle_wobble_amount;
        skew_y = cos(time * idle_wobble_speed * 0.8 + 0.5) * idle_wobble_amount * 0.7;
    }
    
    vec2 scaled_vertex = VERTEX;
    vec2 center = vec2(0.5, 0.5) / TEXTURE_PIXEL_SIZE;
    scaled_vertex = (scaled_vertex - center) * current_scale + center;
    
    scaled_vertex.x += center_offset.y * skew_x / TEXTURE_PIXEL_SIZE.x;
    scaled_vertex.y += center_offset.x * skew_y / TEXTURE_PIXEL_SIZE.y;
    
    VERTEX = scaled_vertex;
}

void fragment() {
    COLOR = texture(TEXTURE, UV);
}
