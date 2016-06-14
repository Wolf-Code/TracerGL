#version 330

out vec4 color_output;
in vec2 uv;

void main( )
{
	color_output = vec4(uv, 0.0, 1.0);
}