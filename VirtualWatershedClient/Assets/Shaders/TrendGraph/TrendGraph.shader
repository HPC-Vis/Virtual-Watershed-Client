Shader "Custom/TrendGraph" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		Time ("Time", float) = 0.0
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
			uniform float Time;

			float4 frag (v2f_img i) : COLOR
			{
				float4 col = tex2D(_MainTex, float2(i.uv.x, 1.0-i.uv.y));

				if (i.uv.x + 0.0027 > Time && i.uv.x - 0.0027 < Time)
				{
					return float4(1.0, 1.0, 0.0, 0.0);
				}
				
				if (col.r > 0.1 || col.g > 0.1 || col.b > 0.1)
				{
					return float4(col.r, col.g, col.b, 0.0);
				}
				else
				{
					return float4(1, 0, 0, 1);
				}
			}
			ENDCG

		}
	}
}

Fallback off

}
