// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Colorbar" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_FloatMin("Min Value", float) = 0
		_FloatMax("Max Value", float) = 5

		_SegmentData000("Segment Color 000", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
		_SegmentData001("Segment Color 001", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
		_SegmentData002("Segment Color 002", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
		_SegmentData003("Segment Color 003", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
		_SegmentData004("Segment Color 004", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
		_SegmentData005("Segment Color 005", Color) = (0.00000, 0.00000, 0.00000, 1.00000)

		_x1("Range Limit 1", float) = 0.00000
		_x2("Range Limit 2", float) = 0.00000
		_x3("Range Limit 3", float) = 0.00000
		_x4("Range Limit 4", float) = 0.00000
		_x5("Range Limit 5", float) = 0.00000
	}
		SubShader
	{
		// No culling or depth
		//Cull Off ZWrite Off ZTest Always

		Tags{ "Queue" = "Transparent" }
		ZWrite Off
		Blend OneMinusSrcAlpha SrcAlpha

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#pragma target 3.0
#pragma exclude_renderers xbox360 gles
#pragma fragmentoption ARB_precision_hint_fastest 

#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;

	uniform float _FloatMin;
	uniform float _FloatMax;

	uniform float _x1;
	uniform float _x2;
	uniform float _x3;
	uniform float _x4;
	uniform float _x5;

	uniform float4 _SegmentData000;
	uniform float4 _SegmentData001;
	uniform float4 _SegmentData002;
	uniform float4 _SegmentData003;
	uniform float4 _SegmentData004;
	uniform float4 _SegmentData005;


	float Normalize(float Y)
	{
		return (Y - _FloatMin) / (_FloatMax - _FloatMin);
	}


	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
		// just invert the colors
		col = 1 - col;

		float _Blend = 0.5;
		if (i.uv.y < Normalize(_x1)) {
			return float4(_SegmentData000.xyz, 0.0);
		}
		else if (i.uv.y < Normalize(_x2)) {
			return float4(_SegmentData001.xyz, 0.0);
		}
		else if (i.uv.y < Normalize(_x3)) {
			return float4(_SegmentData002.xyz, 0.0);
		}
		else if (i.uv.y < Normalize(_x4)) {
			return float4(_SegmentData003.xyz, 0.0);
		}
		else if (i.uv.y < Normalize(_x5)) {
			return float4(_SegmentData004.xyz, 0.0);
		}
		else {
			return float4(0.0, 0.0, 0.0, 1.0);
		}

		
	}
		ENDCG
	}
	}
}