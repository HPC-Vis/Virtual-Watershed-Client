Shader "Custom/ColorProjector" {

Properties{
	_MainTex("32bit Float Map", RECT) = "white" {}
	_MainTex2("32bit Float Map 2", RECT) = "white" {}

	// _Blend("Blend", Range(0,1)) = 1
	_NumLines("Grid Lines", int) = 10
	// _Opacity ("Opacity", Range(0,1)) = 1

	_FloatMin("Min Value", float) = 15
	_FloatMax("Max Value", float) = 100

	// _FloatMin2("Min Value 2", float) = 15
	// _FloatMax2("Max Value 2", float) = 100

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
	_MaxX("Max X", float) = 1
	_MaxY("Max Y", float) = 1

	_Compare("Compare", int) = 0
}

Category{
	SubShader{
		Tags{ "Queue" = "Transparent" }
		ZWrite Off
		Blend OneMinusSrcAlpha SrcAlpha
		Pass {

		CGPROGRAM
		// Upgrade NOTE: excluded shader from Xbox360, OpenGL ES 2.0 because it uses unsized arrays
		#pragma target 3.0
		#pragma exclude_renderers xbox360 gles
		//#pragma vertex vert_img
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest 
		#include "UnityCG.cginc"
		#include "ColorFunctions.cginc"

		uniform sampler2D _MainTex;
		uniform sampler2D _MainTex2;

		// uniform float _Blend;
		uniform int _NumLines;

		uniform float _FloatMin;
		uniform float _FloatMax;

		// uniform float _FloatMin2;
		// uniform float _FloatMax2;

		uniform float4 _SegmentData000;
		uniform float4 _SegmentData001;
		uniform float4 _SegmentData002;
		uniform float4 _SegmentData003;
		uniform float4 _SegmentData004;
		uniform float4 _SegmentData005;

		uniform float _x1;
		uniform float _x2;
		uniform float _x3;
		uniform float _x4;
		uniform float _x5;
		uniform float _MaxX;
		uniform float _MaxY;

		uniform int _Compare;

		float4x4 _Projector;

		struct vertexInput {
			float4 uv : TEXCOORD0;
			float4 pos : SV_POSITION;
		};

		vertexInput vert(float4 vertex : POSITION)
		{
			vertexInput o;
			o.pos = mul(UNITY_MATRIX_MVP, vertex);
			o.uv = mul(_Projector, vertex);
			return o;
		}

		float4 frag (vertexInput i) : SV_Target
		{
			// Check bounding
			i.uv.x = i.uv.x / _MaxX;
			i.uv.y = i.uv.y / _MaxY;
			if (i.uv.x < 0.01 || i.uv.x > 0.99 || i.uv.y < 0.01 || i.uv.y > 0.99)
			{
				return float4(0, 0, 0, 1);
			}

			// Get the color of the texture and check for available module
			float4 col = tex2Dproj(_MainTex, UNITY_PROJ_COORD(i.uv));			
#if SHADER_API_D3D11			
			float Y =  Color2Float(col);
			if (_Compare == 1)
			{
				float4 col2 = tex2Dproj(_MainTex2, UNITY_PROJ_COORD(i.uv));
				float Y2 = Color2Float(col2);
				// float tempMin = _FloatMin - _FloatMax2;
				// float tempMax = _FloatMax - _FloatMin2;
				// _FloatMin = tempMin;
				// _FloatMax = tempMax;
				Y = Y - Y2;
			}			
#else
			float Y =  Color2FloatMod(col);
			if (_Compare == 1)
			{
				float4 col2 = tex2Dproj(_MainTex2, UNITY_PROJ_COORD(i.uv));
				float Y2 = Color2FloatMod(col2);
				// float tempMin = _FloatMin - _FloatMax2;
				// float tempMax = _FloatMax - _FloatMin2;
				// _FloatMin = tempMin;
				// _FloatMax = tempMax;
				Y = Y - Y2;
			}
#endif

			// Draw the grid lines
			if(gridClamp(i.uv.x, _NumLines))
			{
				return float4(0,0,0,0);
			} 
			if(gridClamp(i.uv.y, _NumLines))
			{
				return float4(0,0,0,0);
			}	

			Y = Normalize(Y, _FloatMin, _FloatMax);
			// Y2 = Normalize(Y2, _FloatMin, _FloatMax);
			_x1 = Normalize(_x1, _FloatMin, _FloatMax);
			_x2 = Normalize(_x2, _FloatMin, _FloatMax);
			_x3 = Normalize(_x3, _FloatMin, _FloatMax);
			_x4 = Normalize(_x4, _FloatMin, _FloatMax);
			_x5 = Normalize(_x5, _FloatMin, _FloatMax);

			// Return all 0 values
			if(Y <= 0.00 )
			{
			  return float4(0,0,0,1);
			}

			// get the color and return
			float4 colour, colour2;
			colour = determineColor(Y, _SegmentData000, _SegmentData001, _SegmentData002, _SegmentData003, _SegmentData004, _SegmentData005, _x1, _x2, _x3, _x4, _x5);
			// colour2 = determineColor(Y2, _SegmentData000, _SegmentData001, _SegmentData002, _SegmentData003, _SegmentData004, _SegmentData005, _x1, _x2, _x3, _x4, _x5);
			// return lerp(colour, colour2, _Blend);
			return colour;
		}
		ENDCG

		}
	}
}

Fallback off

}
