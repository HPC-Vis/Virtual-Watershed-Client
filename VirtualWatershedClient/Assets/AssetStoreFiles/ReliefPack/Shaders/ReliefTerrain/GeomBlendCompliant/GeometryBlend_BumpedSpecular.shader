//
// Relief Terrain Geometry Blend shader
// Tomasz Stobierski 2014
//
Shader "Relief Pack/GeometryBlend_BumpedSpecular" {
Properties {
		//
		// keep in mind that not all of properties are used, depending on shader configuration in defines section below
		//
		_Color ("Color (RGBA)", Color) = (1,1,1,1)
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0, 100)) = 80
		_MainTex ("Main texture (RGBA)", 2D) = "black" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
}


SubShader {
	Tags {
		"Queue"="Geometry+12"
		"RenderType" = "Opaque"
	}
	LOD 700

Offset -1,-1
ZTest LEqual
CGPROGRAM
#pragma surface surf CustomBlinnPhong fullforwardshadows decal:blend
#pragma target 3.0

// remove it (comment if you don't use complemenatry lighting in your scene)
#define RTP_COMPLEMENTARY_LIGHTS

#include "UnityCG.cginc"

sampler2D _MainTex;
sampler2D _BumpMap;
float4 _Color;
float _Shininess;


struct Input {
	float2 uv_MainTex;
	float4 color:COLOR;
};

// Custom&reflexLighting
#include "Assets/ReliefPack/Shaders/CustomLightingLegacy.cginc"
	
void surf (Input IN, inout SurfaceOutput o) {
	float4 tex = tex2D(_MainTex, IN.uv_MainTex.xy);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex.xy));
	
	o.Alpha=1-IN.color.a;
	#if defined(UNITY_PASS_PREPASSFINAL)
		o.Gloss*=o.Alpha;
	#endif	
}

ENDCG
}

	FallBack "Diffuse"
}
