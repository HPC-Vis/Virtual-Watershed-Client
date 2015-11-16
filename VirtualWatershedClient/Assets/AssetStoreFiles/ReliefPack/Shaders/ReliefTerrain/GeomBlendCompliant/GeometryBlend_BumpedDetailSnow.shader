//
// Relief Terrain Geometry Blend shader
// Tomasz Stobierski 2014
//
//  Relief Terrain Pack here consits of  simple geometry blending shaders - only bumped specular
//	If you'd like to replace them with better Parallax Occlusion Mapped with self-shadow - take them from twin product - Relief Shaders Pack
//  - they're specialy tailored for such usage (just copy/paste here bonus file contents)
//
Shader "Relief Pack/GeometryBlend_BumpedDetailSnow" {
Properties {
		//
		// keep in mind that not all of properties are used, depending on shader configuration in defines section below
		//
		_Color ("Color (RGBA)", Color) = (1,1,1,1)
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0, 100)) = 80
		_MainTex ("Main texture (RGBA)", 2D) = "black" {}
		_BumpMap ("Normal map", 2D) = "bump" {}
		//_DetailColor ("Detail Color (RGBA)", Color) = (1, 1, 1, 1)
		_DetailBumpTex ("Detail Normalmap", 2D) = "bump" {}
		_DetailScale ("Detail Normal Scale", Float) = 1

		rtp_snow_mult("Snow multiplicator", Range(0,2)) = 1
		_ColorSnow ("Snow texture (RGBA)", 2D) = "white" {}
		_BumpMapSnow ("Snow Normalmap", 2D) = "bump" {}
		_distance_start("Snow near distance", Float) = 10
		_distance_transition("Snow distance transition length", Range(0,100)) = 20
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
#pragma surface surf CustomBlinnPhong vertex:vert fullforwardshadows decal:blend
#pragma target 3.0
#pragma glsl
#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING

// remove it (comment if you don't use complemenatry lighting in your scene)
#define RTP_COMPLEMENTARY_LIGHTS

#include "UnityCG.cginc"

#define detail_map_enabled

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	//#define RTP_SNOW
	//#define RTP_SNW_CHOOSEN_LAYER_NORM
	//#define RTP_SNW_CHOOSEN_LAYER_COLOR
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed3 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_mult;
float rtp_snow_deep_factor;

sampler2D _ColorSnow;
sampler2D _BumpMapSnow;
float4 _MainTex_TexelSize;
float4 _BumpMap_TexelSize;
#endif
////////////////////////////////////////////////////////////////////

sampler2D _MainTex;
sampler2D _BumpMap;
float4 _Color;
float _Shininess;

half _distance_start;
half _distance_transition;

fixed4 _DetailColor;
float _DetailScale;
sampler2D _DetailBumpTex;

float4 _MainTex_ST;

struct Input {
	float2 _uv_MainTex;
	float2 uv_ColorSnow;
	float4 snowDir;
	
	float3 worldPos;
	float4 color:COLOR;
};

// Custom&reflexLighting
#include "Assets/ReliefPack/Shaders/CustomLightingLegacy.cginc"
	
void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_MainTex.xy=v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		TANGENT_SPACE_ROTATION;
		o.snowDir.xyz = mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz);
		o.snowDir.w = mul(_Object2World, v.vertex).y;
	#endif	
/////////////////////////////////////////////////////////////////////	
}

void surf (Input IN, inout SurfaceOutput o) {
//	o.Emission.rg=frac(IN._uv_MainTex.xy);
//	o.Alpha=1;
//	return;
	
	float4 tex = tex2D(_MainTex, IN._uv_MainTex.xy);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	o.Normal = UnpackNormal(tex2D(_BumpMap, IN._uv_MainTex.xy));
	
	#ifndef RTP_SIMPLE_SHADING
	#ifdef detail_map_enabled
		float3 norm_det=UnpackNormal(tex2D(_DetailBumpTex, IN._uv_MainTex.xy*_DetailScale));
		o.Normal+=2*norm_det;//*_DetailColor.a;
		o.Normal=normalize(o.Normal);
	#endif
	#endif
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2*rtp_snow_mult;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow;
		float3 norm_for_snow=float3(0,0,1);
		snow_val -= rtp_snow_slope_factor*( 1 - dot(norm_for_snow, IN.snowDir.xyz) );

		float snow_depth=snow_val-1;
		snow_depth=snow_depth<0 ? 0:snow_depth*6;		
		
		snow_val -= rtp_snow_slope_factor*( 1 - dot(o.Normal, IN.snowDir.xyz));
		snow_val=saturate(snow_val);
		snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
		
		float snow_depth_lerp=saturate(snow_depth-rtp_snow_deep_factor);
		
		#ifdef RTP_SNW_CHOOSEN_LAYER_COLOR
		#ifndef RTP_SIMPLE_SHADING
			half4 c=tex2D(_ColorSnow, IN.uv_ColorSnow);
			float3 rtp_snow_color_tex=c.rgb;
			rtp_snow_gloss=c.a;
			
			float _dist=saturate((distance(_WorldSpaceCameraPos, IN.worldPos) - _distance_start) / _distance_transition);
			
			rtp_snow_color=lerp(rtp_snow_color_tex, rtp_snow_color, _dist);
		#endif
		#endif
		
		o.Albedo=lerp( o.Albedo, rtp_snow_color, snow_val );
		
		float2 dx = ddx( IN._uv_MainTex.xy * _MainTex_TexelSize.z );
		float2 dy = ddy( IN._uv_MainTex.xy * _MainTex_TexelSize.w );
		float d = max( dot( dx, dx ), dot( dy, dy ) );
		float mip_selector=min(0.5*log2(d), 8);
		float mip_selector_bumpMap=	max(0,mip_selector-log2(_BumpMap_TexelSize.x/_MainTex_TexelSize.x));
		float3 snow_normal=UnpackNormal(tex2Dlod(_BumpMap, float4(IN._uv_MainTex.xy, mip_selector_bumpMap.xx+snow_depth.xx)));
		
		#ifdef RTP_SNW_CHOOSEN_LAYER_NORM
			float3 n=UnpackNormal(tex2D(_BumpMapSnow, IN.uv_ColorSnow));
			snow_normal=lerp(snow_normal, n, snow_depth_lerp );
			snow_normal=normalize(snow_normal);
		#endif
		
		o.Normal=lerp(o.Normal, snow_normal, snow_val);		
		//o.Normal=normalize(o.Normal);
		
		o.Gloss=lerp(o.Gloss, rtp_snow_gloss, snow_val);
		o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);	
	#endif
/////////////////////////////////////////////////////////////////////
	
	o.Alpha=1-IN.color.a;
	#if defined(UNITY_PASS_PREPASSFINAL)
		o.Gloss*=o.Alpha;
	#endif
}

ENDCG
}


SubShader {
	Tags {
		"Queue" = "Geometry+10"
		"RenderType" = "Opaque"
	}
	LOD 100

Offset -1,-1
ZTest LEqual
CGPROGRAM
#pragma surface surf Lambert vertex:vert decal:blend
#pragma target 3.0
#pragma glsl
#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
#pragma debug
#include "UnityCG.cginc"

/////////////////////////////////////////////////////////////////////
// RTP specific
//
	//#define RTP_SNOW
/////////////////////////////////////////////////////////////////////

sampler2D _MainTex;
float4 _Color;
float _Shininess;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef RTP_SNOW
float rtp_snow_strength;
float rtp_global_color_brightness_to_snow;
float rtp_snow_slope_factor;
float rtp_snow_edge_definition;
float4 rtp_snow_strength_per_layer0123;
float4 rtp_snow_strength_per_layer4567;
float rtp_snow_height_treshold;
float rtp_snow_height_transition;
fixed3 rtp_snow_color;
float rtp_snow_gloss;
float rtp_snow_specular;
float rtp_snow_mult;
float rtp_snow_deep_factor;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float2 uv_MainTex;
	float4 snowDir;
	float4 color:COLOR;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		o.snowDir.xyz = normalize( mul(_Object2World, float4(v.normal,0)).xyz );
		o.snowDir.w = mul(_Object2World, v.vertex).y;
	#endif	
/////////////////////////////////////////////////////////////////////	
}

void surf (Input IN, inout SurfaceOutput o) {
	float4 tex = tex2D(_MainTex, IN.uv_MainTex);
	o.Albedo = tex.rgb * _Color.rgb;
	o.Gloss = tex.a * _Color.a;
	o.Specular = _Shininess;
	
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#ifdef RTP_SNOW
		float snow_val=rtp_snow_strength*2*rtp_snow_mult;
		float snow_height_fct=saturate((rtp_snow_height_treshold - IN.snowDir.w)/rtp_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow;
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= rtp_snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		o.Albedo=lerp( o.Albedo, rtp_snow_color, snow_val );
		
		#if !defined(SHADER_API_FLASH)
			o.Gloss=lerp(o.Gloss, rtp_snow_gloss, snow_val);
			o.Specular=lerp(o.Specular, rtp_snow_specular, snow_val);			
		#endif
	#endif
/////////////////////////////////////////////////////////////////////

	o.Alpha=1-IN.color.a;
	#if defined(UNITY_PASS_PREPASSFINAL)
		o.Gloss*=o.Alpha;
	#endif
}
ENDCG

}

	FallBack "Diffuse"
}
