#ifdef GL_ES
	precision mediump float;
#endif // GL_ES


uniform sampler2D DIFFUSE_MAP;
//uniform vec4 MODULATION_COLOR;


varying vec2 texcoord0;

void main(void)
{
	gl_FragColor = texture2D(DIFFUSE_MAP, texcoord0);
}
