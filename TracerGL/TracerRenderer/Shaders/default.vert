#version 330

uniform mat4 MVP;

layout(location = 0) in vec4 position;

out vec3 pos;

void main( ) 
{
	gl_Position = MVP * position;
    pos = gl_Position.xyz;
}