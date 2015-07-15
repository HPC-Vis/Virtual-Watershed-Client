Shader "Custom/ColorMap" {
	
Properties {
	_MainTex ("32bit Float Map", 2D) = "white" {}
	
	_FloatMin ("Min Value", float) = 10
	_FloatMax ("Max Value", float) =  100
    
    _SegmentData000 ("Segment Color 000", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData001 ("Segment Color 001", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData002 ("Segment Color 002", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData003 ("Segment Color 003", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData004 ("Segment Color 004", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
    _SegmentData005 ("Segment Color 005", Color) = (0.00000, 0.00000, 0.00000, 1.00000)
   

}

Category {
	SubShader {
		Tags {"Queue" = "Geometry"}
//		Zwrite Off
		Pass {
			Fog { Mode off }
				
CGPROGRAM
// Upgrade NOTE: excluded shader from Xbox360, OpenGL ES 2.0 because it uses unsized arrays
//#pragma target 4.0
//#pragma exclude_renderers xbox360 gles
#pragma vertex vert_img
#pragma fragment frag
#pragma fragmentoption ARB_precision_huint_fastest 
#include "UnityCG.cginc"

uniform sampler2D _MainTex;

uniform float _FloatMin;
uniform float _FloatMax;

uniform half4 _SegmentData000;
uniform float4 _SegmentData001;
uniform float4 _SegmentData002;
uniform float4 _SegmentData003;
uniform float4 _SegmentData004;
uniform float4 _SegmentData005;



float Normalize(float Y)
{
	return (Y - _FloatMin) / (_FloatMax - _FloatMin);
}

 
uint modulos2(uint num,uint den)
{
	return num - (num/den*den);
} 

int modulo(int num,int den)
{
	return num - (num/den*den);
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


float Color2Float(float4 c)
{
	float f;
	uint fi;
					
	fi = c.r * 256;
	uint i25 = modulos2(fi,2);
	fi = fi / 2;
	uint i26 = modulos2(fi,2);
	fi = fi / 2;
	uint i27 = modulos2(fi,2);
	fi = fi / 2;
	uint i28 =modulos2(fi,2);
	fi = fi / 2;
	uint i29 = modulos2(fi,2);
	fi = fi / 2;
	uint i30 = modulos2(fi,2);
	fi = fi / 2;
	uint i31 = modulos2(fi,2);
	fi = fi / 2;
	uint i32 = modulos2(fi,2);
	
					
	fi = c.g * 256;
	uint i17 = modulos2(fi,2);
	fi = fi / 2;
	uint i18 = modulos2(fi,2);
	fi = fi / 2;
	uint i19 = modulos2(fi,2);
	fi = fi / 2;
	uint i20 = modulos2(fi,2);
	fi = fi / 2;
	uint i21 = modulos2(fi,2);
	fi = fi / 2;
	uint i22 = modulos2(fi,2);
	fi = fi / 2;
	uint i23 = modulos2(fi,2);
	fi = fi / 2;
	uint i24 = modulos2(fi,2);
					
	fi = c.b * 256;
	uint i09 =modulos2(fi,2);
	fi = fi / 2;
	uint i10 =modulos2(fi,2);
	fi = fi / 2;
	uint i11 =modulos2(fi,2);
	fi = fi / 2;
	uint i12 =modulos2(fi,2);
	fi = fi / 2;
	uint i13 =modulos2(fi,2);
	fi = fi / 2;
	uint i14 =modulos2(fi,2);
	fi = fi / 2;
	uint i15 =modulos2(fi,2);
	fi = fi / 2;
	uint i16 =modulos2(fi,2);
					
	fi = c.a * 256;
	uint i01 =modulos2(fi,2);
	fi = fi / 2;
	uint i02 =modulos2(fi,2);
	fi = fi / 2;
	uint i03 =modulos2(fi,2);
	fi = fi / 2;
	uint i04 =modulos2(fi,2);
	fi = fi / 2;
	uint i05 =modulos2(fi,2);
	fi = fi / 2;
	uint i06 =modulos2(fi,2);
	fi = fi / 2;
	uint i07 =modulos2(fi,2);
	fi = fi / 2;
	uint i08 =modulos2(fi,2);
					
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

float4 frag (v2f_img i) : COLOR
{
	return float4(0,1,0,1);
	float4 col = tex2D(_MainTex, i.uv);
	float Y =  Color2FloatMod(col);
	Y = Normalize(Y);

	float x0, x1;


        x0 = 0;
        x1 = 0.200000000000;
        if (Y <= x1)
            return lerp(_SegmentData000, _SegmentData001, (Y - x0) / (x1 - x0));

    
        x0 = 0.200000000000;
        x1 = 0.400000000000;
        if (Y <= x1)
            return lerp(_SegmentData001, _SegmentData002, (Y - x0) / (x1 - x0));
    
        x0 = 0.400000000000;
        x1 = 0.600000000000;
        if (Y <= x1)
            return lerp(_SegmentData002, _SegmentData003, (Y - x0) / (x1 - x0));
    
        x0 = 0.600000000000;
        x1 = 0.800000000000;
        if (Y <= x1)
            return lerp(_SegmentData003, _SegmentData004, (Y - x0) / (x1 - x0));
    
        x0 = 0.800000000000;
        x1 = 1.000000000000;
        if (Y <= x1)
            return lerp(_SegmentData004, _SegmentData005, (Y - x0) / (x1 - x0));
    


	return float4(0, 0, 0, 0);
}
ENDCG

		}
		
	}
	
}

Fallback off

}
