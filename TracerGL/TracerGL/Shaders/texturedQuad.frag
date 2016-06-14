#version 330
out vec4 color;
in vec2 texCoords;

uniform sampler2D quadTexture;

void main()
{
    color = texture(quadTexture,texCoords);
}