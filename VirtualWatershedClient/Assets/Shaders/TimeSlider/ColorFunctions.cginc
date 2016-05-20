#ifndef _ColorFunctions
#define _ColorFunctions

///
///
///
int modulo(int num, int den)
{
	return num - ((num / den)*den);
}

///
///
///
bool equalColor(float4 x, float4 y)
{
	return (x.r == y.r && x.g == y.g && x.b == y.b && x.a == y.a);
}

///
///
///
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


///
///
///
float getDecimal(float x, int NumLines)
{
	int intPart = (int)(x * NumLines);
	return ((x * NumLines) - intPart);
}


///
///
///
float4 determineColor(float Y, float4 SegmentData000, float4 SegmentData001, float4 SegmentData002, float4 SegmentData003, float4 SegmentData004, float4 SegmentData005,
					  float x1, float x2, float x3, float x4, float x5)
{
	float x0 = 0.0000;

	if (equalColor(SegmentData000, float4(0, 0, 0, 1)) || equalColor(SegmentData001, float4(0, 0, 0, 1)))
	{
		return float4(0, 0, 0, 1);
	}
	if (Y <= x1)
	{
		float4 colour = lerp(SegmentData000, SegmentData001, (Y - x0) / (x1 - x0));
		colour.a = 0;
		return colour;
	}
	if (Y <= x2)
	{
		float4 colour = lerp(SegmentData001, SegmentData002, (Y - x1) / (x2 - x1));
		colour.a = 0;
		return colour;
	}
	if (Y <= x3)
	{
		float4 colour = lerp(SegmentData002, SegmentData003, (Y - x2) / (x3 - x2));
		colour.a = 0;
		return colour;
	}
	if (Y <= x4)
	{
		float4 colour = lerp(SegmentData003, SegmentData004, (Y - x3) / (x4 - x3));
		colour.a = 0;
		return colour;
	}
	if (Y <= x5)
	{
		float4 colour = lerp(SegmentData004, SegmentData005, (Y - x4) / (x5 - x4));
		colour.a = 0;
		return colour;
	}
	return float4(0, 0, 0, 1);
}


///
///
///
bool gridClamp(float x, int NumLines)
{
	float thresh = 0.005;
	int whole = (int)(x * NumLines);
	float trunc = (float)((float)whole / (float)NumLines);
	return (trunc < x && x < (trunc + thresh));
}

///
///
///
float Normalize(float Y, float FloatMin, float FloatMax)
{
	return (Y - FloatMin) / (FloatMax - FloatMin);
}


///
///
///
float Color2Float(float4 c)
{
	float f;
	uint fi;

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

#endif