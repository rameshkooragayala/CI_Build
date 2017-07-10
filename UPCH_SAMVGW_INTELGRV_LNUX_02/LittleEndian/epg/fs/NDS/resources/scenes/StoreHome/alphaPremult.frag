#ifdef GL_ES
	precision lowp float;
#endif // GL_ES


uniform sampler2D DIFFUSE_MAP;
uniform float G_ALPHA;


varying vec2 texcoord0;

void main(void)
{
	gl_FragColor = texture2D(DIFFUSE_MAP, texcoord0) * G_ALPHA;
}
