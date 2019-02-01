// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

// 
// Author: Roger Lew (rogerlew@vandals.uidaho.edu || rogerlew@gmail.com)
// Date: 1/6/2015
// License: Public Domain
//
// This is hacked to death from 
// http://forum.unity3d.com/threads/how-to-darken-a-projector-part-under-a-mesh.159475/
// http://answers.unity3d.com/questions/40357/projector-projector-shader-and-cg.html
// http://answers.unity3d.com/questions/347898/how-to-use-a-transparentcutout-shader-with-a-proje.html
// https://www.youtube.com/watch?v=2QLCKsXQsz0
//
// VERY IMPORTANT
// To avoid tiling and other problems the cookie needs to:
//   1. Have transparency along all four sides
//   2. Be imported as advanced
//   3. Have wrap mode set to clamp
//   4. Have generate mipmaps disabled
 
Shader "Projector/Slide" { 
	Properties {
		_ShadowTex ("Cookie", 2D) = "gray" {}
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
			uniform float _ShadowTex_ST;
			uniform float _Opacity;

			struct vertexInput {
				float4 uvShadow : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			vertexInput vert (float4 vertex : POSITION)
			{
				vertexInput o;
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul(unity_Projector, vertex);
				return o;
			}

			fixed4 frag (vertexInput i) : SV_Target
			{
				fixed4 texS = tex2Dproj (_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
				texS.a = 1.0 - texS.a * _Opacity;
				return texS;
			}
			ENDCG
		}
	}
}