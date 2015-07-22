Shader "Custom/ColorWindow" {
	
Properties {
	_MainTex ("32bit Float Map", RECT) = "white" {}
	_MainTex2 ("32bit Float Map", RECT) = "white" {}

	
	_FloatMin ("Min Value", float) = 16
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

	_Blend("Blend", Range(0,1)) = 1


   
	
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
uniform sampler2D _MainTex2;


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

bool equalColor(float4 x, float4 y)
{
	return (x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a);
}

int modulo(int num, int den)
{
	return num - ((num / den)*den);
}
 
float Color2Float(float4 c)
{
	float f;
	uint fi;
					
	fi = c.r * 256;
	uint i32 = (fi & 128) / 128;
	uint i31 = (fi & 64) / 64;
	uint i30 = (fi & 32) / 32;
	uint i29 = (fi & 16) / 16;
	uint i28 = (fi & 8) / 8;
	uint i27 = (fi & 4) / 4;
	uint i26 = (fi & 2) / 2;
	uint i25 = (fi & 1) / 1;
					
	fi = c.g * 256;
	uint i24 = (fi & 128) / 128;
	uint i23 = (fi & 64) / 64;
	uint i22 = (fi & 32) / 32;
	uint i21 = (fi & 16) / 16;
	uint i20 = (fi & 8) / 8;
	uint i19 = (fi & 4) / 4;
	uint i18 = (fi & 2) / 2;
	uint i17 = (fi & 1) / 1;
					
	fi = c.b * 256;
	uint i16 = (fi & 128) / 128;
	uint i15 = (fi & 64) / 64;
	uint i14 = (fi & 32) / 32;
	uint i13 = (fi & 16) / 16;
	uint i12 = (fi & 8) / 8;
	uint i11 = (fi & 4) / 4;
	uint i10 = (fi & 2) / 2;
	uint i09 = (fi & 1) / 1;
					
	fi = c.a * 256;
	uint i08 = (fi & 128) / 128;
	uint i07 = (fi & 64) / 64;
	uint i06 = (fi & 32) / 32;
	uint i05 = (fi & 16) / 16;
	uint i04 = (fi & 8) / 8;
	uint i03 = (fi & 4) / 4;
	uint i02 = (fi & 2) / 2;
	uint i01 = (fi & 1) / 1;
					
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
	uint fi;

	fi = c.a * 256;
	uint i01 = modulo(fi, 2);
	fi = fi / 2;
	uint i02 = modulo(fi, 2);
	fi = fi / 2;
	uint i03 = modulo(fi, 2);
	fi = fi / 2;
	uint i04 = modulo(fi, 2);
	fi = fi / 2;
	uint i05 = modulo(fi, 2);
	fi = fi / 2;
	uint i06 = modulo(fi, 2);
	fi = fi / 2;
	uint i07 = modulo(fi, 2);
	fi = fi / 2;
	uint i08 = modulo(fi, 2);

	fi = c.b * 256;
	uint i09 = modulo(fi, 2);
	fi = fi / 2;
	uint i10 = modulo(fi, 2);
	fi = fi / 2;
	uint i11 = modulo(fi, 2);
	fi = fi / 2;
	uint i12 = modulo(fi, 2);
	fi = fi / 2;
	uint i13 = modulo(fi, 2);
	fi = fi / 2;
	uint i14 = modulo(fi, 2);
	fi = fi / 2;
	uint i15 = modulo(fi, 2);
	fi = fi / 2;
	uint i16 = modulo(fi, 2);

	fi = c.g * 256;
	uint i17 = modulo(fi, 2);
	fi = fi / 2;
	uint i18 = modulo(fi, 2);
	fi = fi / 2;
	uint i19 = modulo(fi, 2);
	fi = fi / 2;
	uint i20 = modulo(fi, 2);
	fi = fi / 2;
	uint i21 = modulo(fi, 2);
	fi = fi / 2;
	uint i22 = modulo(fi, 2);
	fi = fi / 2;
	uint i23 = modulo(fi, 2);
	fi = fi / 2;
	uint i24 = modulo(fi, 2);

	fi = c.r * 256;
	uint i25 = modulo(fi, 2);
	fi = fi / 2;
	uint i26 = modulo(fi, 2);
	fi = fi / 2;
	uint i27 = modulo(fi, 2);
	fi = fi / 2;
	uint i28 = modulo(fi, 2);
	fi = fi / 2;
	uint i29 = modulo(fi, 2);
	fi = fi / 2;
	uint i30 = modulo(fi, 2);
	fi = fi / 2;
	uint i31 = modulo(fi, 2);
	fi = fi / 2;
	uint i32 = modulo(fi, 2);

	float _sign = 1.0;
	if (i32 == 1)
	{
		_sign = -1.0;
	}
	float _bias = 127.0;
	float _exponent = i24 + i25*2.0 + i26*4.0 + i27*8.0 + i28*16.0 + i29*32.0 + i30*64.0 + i31*128.0;
	float _mantisa = 1.0 + (i23 / 2.0) + (i22 / 4.0) + (i21 / 8.0) + (i20 / 16.0) + (i19 / 32.0) + (i18 / 64.0) + (i17 / 128.0) + (i16 / 256.0) + (i15 / 512.0) + (i14 / 1024.0) + (i13 / 2048.0) + (i12 / 4096.0) + (i11 / 8192.0) + (i10 / 16384.0) + (i09 / 32768.0) + (i08 / 65536.0) + (i07 / 131072.0) + (i06 / 262144.0) + (i05 / 524288.0) + (i04 / 1048576.0) + (i03 / 2097152.0) + (i02 / 4194304.0) + (i01 / 8388608.0);

	if (((_exponent == 255.0) || (_exponent == 0.0)) && (_mantisa == 0.0))
	{
		f = 0.0;
	}
	else
	{
		_exponent = _exponent - _bias;
		f = _sign * _mantisa * pow(2.0, _exponent);
	}

	return f;
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




float4 frag (v2f_img i) : COLOR
{
	float4 col = tex2D(_MainTex, i.uv);
	float4 col2 = tex2D(_MainTex2, i.uv);

	if(i.uv.x < 0.1 || i.uv.x > 0.9 || i.uv.y < 0.1 || i.uv.y > 0.9)
	{
		return float4(0,0,0,1);
	}

	

#if SHADER_API_D3D11
	float Y = Color2Float(col);
	float Y2 = Color2Float(col2);

#else
	float Y = Color2FloatMod(col);
	float Y2 = Color2FloatMod(col2);

#endif

	Y = Normalize(Y);
	Y2 = Normalize(Y2);
	_x1 = Normalize(_x1);
	_x2 = Normalize(_x2);
	_x3 = Normalize(_x3);
	_x4 = Normalize(_x4);
	_x5 = Normalize(_x5);
	float _Blend;

	_Blend = 0.5;

	if(Y <= 0.00)
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
