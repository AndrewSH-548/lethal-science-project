shader_type canvas_item;
render_mode unshaded;

uniform bool enabled = true;
uniform float radius = 0.5;
uniform float feather = 0.05;
uniform vec2 center = vec2(0.5, 0.5);
uniform float screen_width = 1920;
uniform float screen_height = 1080;

void fragment() {
	if(enabled)
	{
		float ratio = screen_width / screen_height;
	    vec2 adjusted_uv = vec2(mix(center.x, UV.x, ratio), UV.y);
	    float dist = distance(center, adjusted_uv);
	    float edge_start = radius - feather;
	    float edge_end = radius + feather;
	    COLOR.a = smoothstep(edge_start, edge_end, dist);
	}
	else
	{
		discard; // transparent
	}
}