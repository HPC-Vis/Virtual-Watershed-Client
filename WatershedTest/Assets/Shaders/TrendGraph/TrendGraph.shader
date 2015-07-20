Shader "Custom/TrendGraph" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	Category {
	SubShader {
		Tags {"Queue" = "Geometry"}
		//Zwrite Off
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
	float4 col = tex2D(_MainTex, i.uv);

	if (col.r < 0.9)
	{
		return float4(0, 0, 0, 0);
	}
	else if(col.g > 0.9)
	{
		return float4(1.0, 1.0, 1.0, 0.0);
	}
	
	return  float4(1.0, 0.0, 0.0, 0.0);
}
ENDCG

		}
	}
}

Fallback off

}
