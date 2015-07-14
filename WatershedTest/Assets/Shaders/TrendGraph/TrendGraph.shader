Shader "Custom/TrendGraph" {
	Properties {
		_MainTex ("32bit Float Map", RECT) = "white" {}
	}
	Category {
	SubShader {
		Tags {"Queue" = "Geometry"}
//		Zwrite Off
		Blend OneMinusSrcAlpha SrcAlpha 
		Pass {
			Fog { Mode off }
				
CGPROGRAM
// Upgrade NOTE: excluded shader from Xbox360, OpenGL ES 2.0 because it uses unsized arrays
//#pragma target 4.0
#pragma exclude_renderers xbox360 gles
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;

float4 frag (v2f_img i) : COLOR
{
	return(1.0, 0.0, 1.0, 1.0);
}
ENDCG

		}
	}
}

Fallback off

}
