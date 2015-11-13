// 
// Author: Roger Lew (rogerlew@vandals.uidaho.edu || rogerlew@gmail.com)
// Date: 1/6/2015
// License: Public Domain
//
// VERY IMPORTANT
// To avoid tiling and other problems the cookie needs to:
//   1. Have transparency along all four sides
//   2. Be imported as advanced
//   3. Have wrap mode set to clamp
//   4. Have generate mipmaps disabled
 
Shader "VTL/BlendSlideProjector" { 
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
		_ShadowTex2 ("Cookie", 2D) = "gray" {}
		_MaxX("Max X",Float)=1
		_MaxY("Max Y",Float)=1
		_Blend ("Blend", Range(0,1)) = 1
		_Opacity ("Opacity", Range(0,1)) = 1
	}
	Subshader {
		Tags {"Queue"="Transparent"}
		ZWrite Off
		Blend OneMinusSrcAlpha SrcAlpha 

		Pass {
		
			CGPROGRAM

			#pragma exclude_renderers ps3 xbox360
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			
			uniform sampler2D _ShadowTex;
			uniform sampler2D _ShadowTex2;
			uniform float _ShadowTex_ST;
			uniform float _Blend;
			uniform float _Opacity;

			struct vertexInput {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			float4x4 _Projector;
			float4x4 _ProjectorClip;
			vertexInput vert (float4 vertex : POSITION)
			{
				vertexInput o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul(_Projector, vertex);
				return o;
			}

			fixed4 frag (vertexInput i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS.a = 1.0 - texS.a* _Opacity;

				//fixed4 texS2 = tex2Dproj (_ShadowTex2, UNITY_PROJ_COORD(i.uvShadow));
				//texS2.a = 1.0 - texS2.a * _Opacity;
				return texS;//lerp(texS, texS2, _Blend);
			}
			ENDCG
		}
	}
}