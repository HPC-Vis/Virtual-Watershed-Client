Shader "Custom/ColorProjector" {
	
Properties {
	_MainTex ("32bit Float Map", RECT) = "white" {}
	//_MainTex2 ("32bit Float Map", RECT) = "white" {}
	_Blend ( "Blend", Range(0,1)) = 1
	_NumLines( "Grid Lines", int) = 10
	//_Opacity ("Opacity", Range(0,1)) = 1

	
	_FloatMin ("Min Value", float) = 15
	_FloatMax ("Max Value", float) =  100
    
    _SegmentData000 ("Segment Color 000", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData001 ("Segment Color 001", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData002 ("Segment Color 002", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData003 ("Segment Color 003", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData004 ("Segment Color 004", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData005 ("Segment Color 005", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
   
	_x1 ("Range Limit 1", float) = 0.00000
	_x2 ("Range Limit 2", float) = 0.00000
	_x3 ("Range Limit 3", float) = 0.00000
	_x4 ("Range Limit 4", float) = 0.00000
	_x5 ("Range Limit 5", float) = 0.00000
	_MaxX("Max X",Float) = 1
	_MaxY("Max Y",Float) = 1
}

Category {
	SubShader {
		Tags {"Queue"="Transparent"}
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


float Normalize(float Y)
{
	return (Y - _FloatMin) / (_FloatMax - _FloatMin);
}

bool equalColor(float4 x, float4 y)
{
	return (x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a);
}

int modulo(int num, int den)
{
	return num - ((num/den)*den);
}
 
float Color2Float(float4 c)
{
	float f;
	int fi;
					
	fi = c.r * 256;
	int i32 = (fi & 128) / 128;
	int i31 = (fi & 64) / 64;
	int i30 = (fi & 32) / 32;
	int i29 = (fi & 16) / 16;
	int i28 = (fi & 8) / 8;
	int i27 = (fi & 4) / 4;
	int i26 = (fi & 2) / 2;
	int i25 = (fi & 1) / 1;
					
	fi = c.g * 256;
	int i24 = (fi & 128) / 128;
	int i23 = (fi & 64) / 64;
	int i22 = (fi & 32) / 32;
	int i21 = (fi & 16) / 16;
	int i20 = (fi & 8) / 8;
	int i19 = (fi & 4) / 4;
	int i18 = (fi & 2) / 2;
	int i17 = (fi & 1) / 1;
					
	fi = c.b * 256;
	int i16 = (fi & 128) / 128;
	int i15 = (fi & 64) / 64;
	int i14 = (fi & 32) / 32;
	int i13 = (fi & 16) / 16;
	int i12 = (fi & 8) / 8;
	int i11 = (fi & 4) / 4;
	int i10 = (fi & 2) / 2;
	int i09 = (fi & 1) / 1;
					
	fi = c.a * 256;
	int i08 = (fi & 128) / 128;
	int i07 = (fi & 64) / 64;
	int i06 = (fi & 32) / 32;
	int i05 = (fi & 16) / 16;
	int i04 = (fi & 8) / 8;
	int i03 = (fi & 4) / 4;
	int i02 = (fi & 2) / 2;
	int i01 = (fi & 1) / 1;
					
	float _sign = 1.0;
	if (i32==1)
	{
		_sign = -1.0;
	}
	float _bias = 127.0;
	float _exponent = i24 + i25*2.0 + i26*4.0 + i27*8.0 + i28*16.0 + i29*32.0 + i30*64.0 + i31*128.0;
	float _mantisa = 1.0 + (i23/2.0) + (i22/4.0) + (i21/8.0) + (i20/16.0) + (i19/32.0) + (i18/64.0) + (i17/128.0) + (i16/256.0) + (i15/512.0) + (i14/1024.0) + (i13/2048.0) + (i12/4096.0) + (i11/8192.0) + (i10/16384.0) + (i09/32768.0) + (i08/65536.0) + (i07/131072.0) + (i06/262144.0) + (i05/524288.0) + (i04/1048576.0) + (i03/2097152.0) + (i02/4194304.0) + (i01/8388608.0);
					
	if (((_exponent==255.0) || (_exponent==0.0)) && (_mantisa==0.0))
	{
		f = 0.0;
	} else
	{
		_exponent = _exponent - _bias;
		f = _sign * _mantisa * pow(2.0, _exponent);
	} 
			
	return f;
}

float Color2FloatMod(float4 c)
{
	float f;
	int fi;

	fi = c.a * 256;
	int i01 = modulo(fi, 2);
	fi = fi / 2;
	int i02 = modulo(fi, 2);
	fi = fi / 2;
	int i03 = modulo(fi, 2);
	fi = fi / 2;
	int i04 = modulo(fi, 2);
	fi = fi / 2;
	int i05 = modulo(fi, 2);
	fi = fi / 2;
	int i06 = modulo(fi, 2);
	fi = fi / 2;
	int i07 = modulo(fi, 2);
	fi = fi / 2;
	int i08 = modulo(fi, 2);

	fi = c.b * 256;
	int i09 = modulo(fi, 2);
	fi = fi / 2;
	int i10 = modulo(fi, 2);
	fi = fi / 2;
	int i11 = modulo(fi, 2);
	fi = fi / 2;
	int i12 = modulo(fi, 2);
	fi = fi / 2;
	int i13 = modulo(fi, 2);
	fi = fi / 2;
	int i14 = modulo(fi, 2);
	fi = fi / 2;
	int i15 = modulo(fi, 2);
	fi = fi / 2;
	int i16 = modulo(fi, 2);

	fi = c.g * 256;
	int i17 = modulo(fi, 2);
	fi = fi / 2;
	int i18 = modulo(fi, 2);
	fi = fi / 2;
	int i19 = modulo(fi, 2);
	fi = fi / 2;
	int i20 = modulo(fi, 2);
	fi = fi / 2;
	int i21 = modulo(fi, 2);
	fi = fi / 2;
	int i22 = modulo(fi, 2);
	fi = fi / 2;
	int i23 = modulo(fi, 2);
	fi = fi / 2;
	int i24 = modulo(fi, 2);
					
	fi = c.r * 256;
	int i25 = modulo(fi, 2);
	fi = fi / 2;
	int i26 = modulo(fi, 2);
	fi = fi / 2;
	int i27 = modulo(fi, 2);
	fi = fi / 2;
	int i28 = modulo(fi, 2);
	fi = fi / 2;
	int i29 = modulo(fi, 2);
	fi = fi / 2;
	int i30 = modulo(fi, 2);
	fi = fi / 2;
	int i31 = modulo(fi, 2);
	fi = fi / 2;
	int i32 = modulo(fi, 2);
										
	float _sign = 1.0;
	if (i32==1)
	{
		_sign = -1.0;
	}
	float _bias = 127.0;
	float _exponent = i24 + i25*2.0 + i26*4.0 + i27*8.0 + i28*16.0 + i29*32.0 + i30*64.0 + i31*128.0;
	float _mantisa = 1.0 + (i23/2.0) + (i22/4.0) + (i21/8.0) + (i20/16.0) + (i19/32.0) + (i18/64.0) + (i17/128.0) + (i16/256.0) + (i15/512.0) + (i14/1024.0) + (i13/2048.0) + (i12/4096.0) + (i11/8192.0) + (i10/16384.0) + (i09/32768.0) + (i08/65536.0) + (i07/131072.0) + (i06/262144.0) + (i05/524288.0) + (i04/1048576.0) + (i03/2097152.0) + (i02/4194304.0) + (i01/8388608.0);
					
	if (((_exponent==255.0) || (_exponent==0.0)) && (_mantisa==0.0))
	{
		f = 0.0;
	} else
	{
		_exponent = _exponent - _bias;
		f = _sign * _mantisa * pow(2.0, _exponent);
	} 
			
	return f;
}

float4x4 _Projector;
float4x4 _ProjectorClip;

struct vertexInput {
	float4 uv : TEXCOORD0;
	float4 pos : SV_POSITION;
};


vertexInput vert (float4 vertex : POSITION)
{
		vertexInput o;
		o.pos = mul (UNITY_MATRIX_MVP, vertex);
		o.uv = mul(_Projector, vertex);
		return o;
}

	uniform sampler2D _ShadowTex;
	uniform sampler2D _ShadowTex2;
	uniform float _ShadowTex_ST;
	uniform float _Blend;
	uniform int _NumLines;
	//uniform float _Opacity;

float getDecimal(float x){
	int intPart = (int)(x*_NumLines);
	return ((x*_NumLines) - intPart);
}

float4 determineColor(float Y)
{
	float x0 = 0.0000;

	if(equalColor(_SegmentData000, float4(0,0,0,1)) || equalColor(_SegmentData001, float4(0,0,0,1)))
	{
		return float4(0,0,0,1);
	}
	if(Y <= _x1)
	{
		float4 colour = lerp(_SegmentData000, _SegmentData001, (Y - x0) / (_x1 - x0));
		colour.a = 0;
		return colour;
	}
	if(Y <= _x2)
	{
		float4 colour = lerp(_SegmentData001, _SegmentData002, (Y - _x1) / (_x2 - _x1));
		colour.a = 0;
		return colour;
	}
	if(Y <= _x3)
	{
		float4 colour = lerp(_SegmentData002, _SegmentData003, (Y - _x2) / (_x3 - _x2));
		colour.a = 0;
		return colour;
	}
	if(Y <= _x4)
	{
		float4 colour = lerp(_SegmentData003, _SegmentData004, (Y - _x3) / (_x4 - _x3));
		colour.a = 0;
		return colour;
	}
	if(Y <= _x5)
	{
		float4 colour = lerp(_SegmentData004, _SegmentData005, (Y - _x4) / (_x5 - _x4));
		colour.a = 0;
		return colour;
	}
	return float4(0,0,0,1);
}

bool gridClamp(float x)
{
	float thresh = 0.005;
	int whole = (int)(x*_NumLines);
	float trunc = (float)((float)whole/(float)_NumLines);
	return (trunc < x && x < (trunc + thresh));
}

float4 frag (vertexInput i) : SV_Target
{
	i.uv.x = i.uv.x / _MaxX;
    i.uv.y = i.uv.y / _MaxY;
	// return float4(1,1,1,0);
	float4 col = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uv));
	float4 col2 = tex2Dproj(_ShadowTex2, UNITY_PROJ_COORD(i.uv));

	

#if SHADER_API_D3D11
	float Y =  Color2Float(col);
	float Y2 = Color2Float(col);
#else
	float Y =  Color2FloatMod(col);
	float Y2 = Color2FloatMod(col);

#endif


	if(i.uv.x < 0.01 || i.uv.x > 0.99 || i.uv.y < 0.01 || i.uv.y > 0.99)
	{
		return float4(0,0,0,1);
	}

	if(gridClamp(i.uv.x))
	{
	return float4(0,0,0,0);
	} 
	if(gridClamp(i.uv.y))
	{
	return float4(0,0,0,0);
	} 
	

	Y = Normalize(Y);
	Y2 = Normalize(Y2);
	_x1 = Normalize(_x1);
	_x2 = Normalize(_x2);
	_x3 = Normalize(_x3);
	_x4 = Normalize(_x4);
	_x5 = Normalize(_x5);

	_Blend = 0.5;


	if(Y <= 0.00 )
	{
	return float4(0,0,0,1);
	}
		
	float4 colour, colour2;

	colour = determineColor(Y);
	colour2 = determineColor(Y2);
	return lerp(colour, colour2, _Blend);

}
ENDCG

		}
	}
}

Fallback off

}
