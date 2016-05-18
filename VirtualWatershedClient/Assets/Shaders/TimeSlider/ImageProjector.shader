Shader "Custom/ImageProjector" {

	Properties{
		_MainTex("32bit Float Map", RECT) = "white" {}
		_ShadowTex("32bit Float Map", 2D) = "white" {}
	//_MainTex2 ("32bit Float Map", RECT) = "white" {}
	_Blend("Blend", Range(0,1)) = 1
		_NumLines("Grid Lines", int) = 10
		//_Opacity ("Opacity", Range(0,1)) = 1


		_FloatMin("Min Value", float) = 15
		_FloatMax("Max Value", float) = 100

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
		_MaxX("Max X",Float) = 1
		_MaxY("Max Y",Float) = 1
	}

		Category{
		SubShader{
		Tags{ "Queue" = "Transparent" }
		ZWrite Off
		Blend OneMinusSrcAlpha SrcAlpha
		Pass{

		CGPROGRAM
		// Upgrade NOTE: excluded shader from Xbox360, OpenGL ES 2.0 because it uses unsized arrays
#pragma target 3.0
#pragma exclude_renderers xbox360 gles
		//#pragma vertex vert_img
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest 
#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

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


	uniform float _MaxX;
	uniform float _MaxY;
	struct vertexInput {
		float4 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	float4x4 _Projector;
	float4x4 _ProjectorClip;

	vertexInput vert(float4 vertex : POSITION)
	{
		vertexInput o;
		o.pos = mul(UNITY_MATRIX_MVP, vertex);
		o.uv = mul(_Projector, vertex);
		return o;
	}

	uniform sampler2D _ShadowTex;
	uniform sampler2D _ShadowTex2;
	uniform float _ShadowTex_ST;
	uniform float _Blend;
	uniform int _NumLines;




	float4 frag(vertexInput i) : SV_Target
	{
		i.uv.x = i.uv.x / _MaxX;
		i.uv.y = i.uv.y / _MaxY;

	float4 col = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uv));

	if (i.uv.x < 0.01 || i.uv.x > 0.99 || i.uv.y < 0.01 || i.uv.y > 0.99)
	{
		return float4(0, 0, 0, 1);
	}

	return col;

	}
		ENDCG

	}
	}
	}

		Fallback off

}
