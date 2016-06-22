#version 330

out vec4 color_output;
in vec2 uv;
in vec4 vDiffuse;

void main( )
{
	color_output = vDiffuse;
}