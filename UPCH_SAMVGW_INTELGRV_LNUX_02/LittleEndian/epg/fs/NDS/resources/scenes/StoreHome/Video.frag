uniform sampler2D DIFFUSE_MAP;
uniform sampler2D DIFFUSE_MAP2;

varying mediump vec2 texcoord0;

const mediump vec3 cc_r = vec3(1.0, -0.8604, 1.59580);
const mediump vec4 cc_g = vec4(1.0, 0.539815, -0.39173, -0.81290);
const mediump vec3 cc_b = vec3(1.0, -1.071, 2.01700);

/* DIFFUSE_MAP point to the Y video plane, DIFFUSE_MAP2 to the UV video plane */
  void main (void)
  {
      if((texcoord0.x < 0.0) || (texcoord0.x > 1.0))
    {
        gl_FragColor = vec4(0.0, 0.0, 0.0, 0.0);
    }
    else
    {
        mediump vec4  y_vec = texture2D(DIFFUSE_MAP,  texcoord0);
        mediump vec4  c_vec = texture2D(DIFFUSE_MAP2, texcoord0);

        /*  The Y component value is in y_vec.a
            The U component value is in c_vec.b
            The V component value is in c_vec.a
        */

        /*  The output fragment color is calculated by:
            - converting the YUV color from texture to RGB pixel format 
            - extending video pixel with alpha_value to get RGBA video pixel 
            - modulating RGBA video pixel color by basecolor
        */

        /*  The color conversion is equivalent to:
            |R|   |Y - 16/256|   |  0         1.59580 | | U - 128/256 |
            |G| = |Y - 16/256| + | -0.39173  -0.81290 | | V - 128/256 |
            |B|   |Y - 16/256|   |  2.01700   0       | 
        */
        mediump vec4 temp_vec = vec4(y_vec.a,1.0,c_vec.b,c_vec.a) ;

        gl_FragColor = vec4(
                                dot(cc_r,temp_vec.xyw),
                                dot(cc_g,temp_vec),
                                dot(cc_b,temp_vec.xyz),
                                1.0);
    }

  }
