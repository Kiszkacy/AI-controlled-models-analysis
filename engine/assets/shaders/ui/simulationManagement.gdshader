shader_type canvas_item;

uniform float corner_radius : hint_range(0.0, 0.2) = 0.04;
uniform float external_alpha : hint_range(0.0, 1.0) = 1.0; 

void fragment() {
    vec2 size = vec2(textureSize(TEXTURE, 0));
    vec2 pos = UV * size;

    float radius = min(size.x, size.y) * corner_radius;

    vec2 top_left = vec2(radius, radius);
    vec2 top_right = vec2(size.x - radius, radius);
    vec2 bottom_left = vec2(radius, size.y - radius);
    vec2 bottom_right = vec2(size.x - radius, size.y - radius);

    float dist = min(
        min(length(pos - top_left), length(pos - top_right)),
        min(length(pos - bottom_left), length(pos - bottom_right))
    );

    if (pos.x < radius && pos.y < radius && dist > radius ||
        pos.x > size.x - radius && pos.y < radius && dist > radius ||
        pos.x < radius && pos.y > size.y - radius && dist > radius ||
        pos.x > size.x - radius && pos.y > size.y - radius && dist > radius) {
        discard;
    }

    vec4 tex_color = texture(TEXTURE, UV);
    COLOR = vec4(tex_color.rgb, tex_color.a * external_alpha);
}
