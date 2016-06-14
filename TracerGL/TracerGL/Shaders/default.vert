#version 330

uniform mat4 MVP;

layout(location = 0) in vec4 position;
layout(location = 2) in vec2 texcoords;

out vec2 uv;

void main( ) 
{
	gl_Position = MVP * position;
    uv = texcoords;
}