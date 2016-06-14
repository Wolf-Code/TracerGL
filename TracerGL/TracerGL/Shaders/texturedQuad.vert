#version 330 
layout(location = 0) in vec3 position;
layout(location = 2) in vec2 texcoords;

out vec2 texCoords;


void main()
{
    gl_Position = vec4(position.x, position.y, 0.0f, 1.0f);
    texCoords = texcoords;
}