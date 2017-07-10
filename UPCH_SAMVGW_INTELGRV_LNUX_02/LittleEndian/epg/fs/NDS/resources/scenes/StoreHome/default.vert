uniform mat4 WORLD;
uniform mat4 PROJ_VIEW;
uniform vec3 UV_OFFSET;

attribute vec4 POSITION;
attribute vec2 TEXCOORD0;

varying vec2 texcoord0;

void main(void)
{
	texcoord0 = vec2(TEXCOORD0.x-UV_OFFSET.x, 1.0 - TEXCOORD0.y - UV_OFFSET.y);
	gl_Position = PROJ_VIEW*(WORLD * POSITION);
}
