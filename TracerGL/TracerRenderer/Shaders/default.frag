#version 330

out vec4 color_output;
in vec3 pos;

void main( )
{
	color_output = vec4(pos, 1.0);
}