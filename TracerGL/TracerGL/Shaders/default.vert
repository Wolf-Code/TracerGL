#version 330

uniform mat4 MVP;
uniform vec4 diffuse;

layout(location = 0) in vec4 position;
layout(location = 1) in vec4 normal;
layout(location = 2) in vec2 texcoords;

out vec2 uv;
out vec4 vDiffuse;

void main( ) 
{
	gl_Position = MVP * position;
    uv = texcoords;
    vDiffuse = diffuse;
}