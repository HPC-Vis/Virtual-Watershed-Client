//
// Relief Terrain  - 2 Parallax mapped materials with water (flowMap RG - first layer + GB - 2nd)
// Tomasz Stobierski 2013
//
// distances have the same meaning like in RTP inspector
//
// we're using position (x,z), size (x,z) and tiling of underlying terrain to get right coords of global perlin map and global colormap
//
// To speed-up shader we're using combined textures.
// _BumpMapCombined is texture made like RTP terrain shader do. You can make it in RTP inspector and save or:
// 1. open Window/4 to 1 texture channel mixer
// 2. target texture channels are:
//     - R = A from 1st bumpmap
//     - G = G from 1st bumpmap
//     - B = A from 2st bumpmap
//     - A = G from 2st bumpmap
// 3. combined heightmap uses RG channels - you can make it using tool described above
//
//  material blend vertex color R
//  water mask taken from synced terrain texture - perlin combined (channel B) or vertex color (B by default) - see #define VERTEX_COLOR_TO_WATER_COVERAGE below
//
Shader "Relief Pack/GeometryBlend_WaterShader_FlowMap_2VertexPaint" {
    Properties {
		TERRAIN_LayerReflection ("Layer reflection (water independent)", Range(0,2)) = 1
		TERRAIN_FresnelPow ("Fresnel Pow", Range(0.5,32)) = 2
		TERRAIN_FresnelOffset ("Fresnel Offset", Range(0.0,0.9)) = 0.2
		
		TERRAIN_LayerWetStrength ("Layer wetness", Range(0,1)) = 1
		
		TERRAIN_WaterLevelSlopeDamp ("Water level slope damp", Range(0.25,8)) = 4
		TERRAIN_ExtrudeHeight ("Extrude Height", Range(0.001,0.08)) = 0.02 
		
		_MainColor ("Main Color (RGB)", Color) = (0.5, 0.5, 0.5, 1)			
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.5
		TERRAIN_WaterSpecularity ("Water Shininess (Specularity)", Range (0.01, 1)) = 0.5
		
		_MainTex ("Texture A", 2D) = "white" {}
		_MainTex2 ("Texture B", 2D) = "white" {}
		_BumpMapCombined ("Bumpmap A+B (RGBA)", 2D) = "bump" {}
		_HeightMapCombined ("Heightmap A+B (RG)", 2D) = "black" {}		
		
 		_FlowMap ("FlowMap (RG+BA)", 2D) = "grey" {}
 		     
		TERRAIN_RippleMap ("Ripplemap", 2D) = "gray" {}
		TERRAIN_RippleScale ("Ripple scale", Float) = 1
		
		TERRAIN_WaterLevel ("Water Level", Range(0,2)) = 0.5
		TERRAIN_WaterColor ("Water Color", Color) = (1,1,1,1)
		TERRAIN_WaterEdge ("Water Edge", Range(1, 4)) = 1
		TERRAIN_WaterOpacity ("Water Opacity", Range(0,1)) =0.5
		TERRAIN_WaterGloss ("Water Gloss", Range(0,1)) = 1
		TERRAIN_DropletsSpeed ("Droplets speed", Float) = 15
		TERRAIN_WetDropletsStrength ("Rain on wet", Range(0,1)) = 0
		TERRAIN_Refraction("Refraction", Range(0,0.04)) = 0.02
		TERRAIN_WetRefraction ("Wet refraction", Range(0,1)) = 1
		TERRAIN_Flow ("Flow", Range(0, 1)) = 0.1
		TERRAIN_FlowScale ("Flow Scale", Float) = 1
		TERRAIN_FlowSpeed ("Flow Speed", Range(0, 3)) = 0.25
		TERRAIN_FlowSpeed ("Flow Speed", Range(0, 3)) = 0.25
		TERRAIN_WetSpecularity ("Wet gloss", Range(0, 1)) = 0.1
		TERRAIN_WetReflection ("Wet reflection", Range(0, 2)) = 0.5
		
		TERRAIN_ReflColorA ("Reflection Color Hi", Color) = (1,1,1,1)
		TERRAIN_ReflColorB ("Reflection Color Low", Color) = (1,1,1,1)
		TERRAIN_ReflDistortion ("Reflection distortion", Range(0,0.1)) = 0.05
		TERRAIN_ReflectionRotSpeed ("Reflection Rot Speed", Range(0,2)) = 0.3
		
		_BumpMapPerlin ("Perlin global (special combined RG)", 2D) = "black" {}
		_BumpMapPerlinBlendA ("Perlin normal A", Range(0,2)) = 0.3
		_BumpMapPerlinBlendB ("Perlin normal B", Range(0,2)) = 0.3
		_BumpMapGlobalScale ("Tiling scale", Range( 0.01,0.25) ) = 0.1
		_TERRAIN_distance_start ("Near distance start", Float) = 2
		_TERRAIN_distance_transition ("Near fade length", Float) = 20
		_TERRAIN_distance_start_bumpglobal ("Far distance start", Float) = 22
		_TERRAIN_distance_transition_bumpglobal ("Far fade length", Float) = 40
		rtp_perlin_start_val ("Perlin start val", Range(0,1)) = 0.3	
		
		_ColorMapGlobal ("Global colormap", 2D) = "black" {}
		// uncomment when used w/o ReliefTerrain script component (for example with VoxelTerrains)
		//_GlobalColorMapBlendValues ("Global colormap blending (XYZ)", Vector) = (0.3,0.6,0.8,0)
		//_GlobalColorMapSaturation ("Global colormap saturation", Range(0,1)) = 0.8
		
		_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)
		
		//_TERRAIN_HeightMap ("Terrain HeightMap (combined)", 2D) = "white" {}
		//_TERRAIN_Control ("Terrain splat controlMap", 2D) = "white" {}		
    }
    SubShader {
	Tags { "RenderType" = "Opaque" "Queue"="Geometry+12" }
	
	Offset -1,-1
	ZTest LEqual		
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert decal:blend
	#pragma only_renderers d3d9 opengl d3d11
	#pragma glsl
	#pragma target 3.0
	
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	
	// remove it (comment if you don't use complemenatry lighting in your scene)
	#define RTP_COMPLEMENTARY_LIGHTS
	
	#define RTP_REFLECTION
	#define RTP_ROTATE_REFLECTION
	
	#define RTP_WETNESS
	// enabled below if you don't want to use water flow
	//#define SIMPLE_WATER
	#define RTP_WET_RIPPLE_TEXTURE	  
	
	// edges can be heightblended
	//#define BLENDING_HEIGHT
	
	// we'll use global colormap
	#define COLOR_MAP
	#define COLOR_MAP_BLEND_MULTIPLY
	// we'll use global perlin
	#define GLOBAL_PERLIN
	// we'll show only global colormap at far distance (no detail)
	//#define SIMPLE_FAR	
	
	// if defined we don't use terrain wetmask, but B channel of vertex color
	//#define VERTEX_COLOR_TO_WATER_COVERAGE IN.color.b
	
	struct Input {
		float4 custUV;
		float4 _auxDir;
		
		float3 worldPos;
		float3 viewDir;
		float4 color:COLOR;
	};
	
	float TERRAIN_GlobalWetness;
	
	float TERRAIN_LayerWetStrength;
	float TERRAIN_WaterLevelSlopeDamp;
	float TERRAIN_ExtrudeHeight;
	
	half3 _MainColor;	
	float _Shininess;
	
	sampler2D _MainTex;
	float4 _MainTex_ST;	 	
	sampler2D _MainTex2;
	sampler2D _BumpMapCombined;
	sampler2D _HeightMapCombined;
	
	sampler2D _FlowMap;	
	
	half4 TERRAIN_ReflColorA;
	half4 TERRAIN_ReflColorB;
	float TERRAIN_ReflDistortion;
	float TERRAIN_ReflectionRotSpeed;
	
	sampler2D TERRAIN_RippleMap;
	float TERRAIN_DropletsSpeed;
	float TERRAIN_RainIntensity;
	float TERRAIN_WetDropletsStrength;
	float TERRAIN_Refraction;
	float TERRAIN_WetRefraction;
	float TERRAIN_Flow;
	float TERRAIN_FlowScale;
	float TERRAIN_RippleScale;
	float TERRAIN_FlowSpeed;
	float TERRAIN_FlowSpeedMap;	
	float TERRAIN_WetSpecularity;
	float TERRAIN_WetReflection;
	float TERRAIN_LayerReflection;
	float TERRAIN_WaterLevel;
	half4 TERRAIN_WaterColor;
	float TERRAIN_WaterEdge;
	float TERRAIN_WaterSpecularity;
	float TERRAIN_WaterGloss;
	float TERRAIN_WaterOpacity;
	float TERRAIN_FresnelPow;
	float TERRAIN_FresnelOffset;
      
	sampler2D _ColorMapGlobal;
	sampler2D _BumpMapPerlin;
	float _BumpMapGlobalScale;
	float _BumpMapPerlinBlendA;
	float _BumpMapPerlinBlendB;
	
	float rtp_perlin_start_val;			
	float _TERRAIN_distance_start;
	float _TERRAIN_distance_transition;
	float _TERRAIN_distance_start_bumpglobal;
	float _TERRAIN_distance_transition_bumpglobal;
	
	float4 _TERRAIN_PosSize;
	float4 _TERRAIN_Tiling;
	
	// set globaly by ReliefTerrain script
	float4 _GlobalColorMapBlendValues;
	float _GlobalColorMapSaturation;
	
	sampler2D _TERRAIN_HeightMap;
	sampler2D _TERRAIN_Control;
	
	// Custom&reflexLighting
	#include "Assets/ReliefPack/Shaders/CustomLightingLegacy.cginc"
	      
	inline float2 GetRipple(float2 UV, float Intensity)
	{
	    float4 Ripple = tex2D(TERRAIN_RippleMap, UV);
	    Ripple.xy = Ripple.xy * 2 - 1;
	
	    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
	    float TimeFrac = DropFrac - 1.0f + Ripple.z;
	    float DropFactor = saturate(0.2f + Intensity * 0.8f - DropFrac);
	    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
	    
	    return Ripple.xy * FinalFactor * 0.35f;
	}
	
	void vert (inout appdata_full v, out Input o) {
	    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
			UNITY_INITIALIZE_OUTPUT(Input, o);
		#endif
		o.custUV.xy=v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;		
		#if defined(RTP_REFLECTION) || defined(RTP_WETNESS)
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );		
			
			float3 viewDir = -ObjSpaceViewDir(v.vertex);
			float3 viewRefl = reflect (viewDir, v.normal);
			float2 refl_vec = normalize(mul((float3x3)_Object2World, viewRefl)).xz;
			#ifdef RTP_ROTATE_REFLECTION
				float3 refl_rot;
				refl_rot.x=sin(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.y=cos(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.z=-refl_rot.x;
				o._auxDir.x=dot(refl_vec, refl_rot.yz);
				o._auxDir.y=dot(refl_vec, refl_rot.xy);
			#else
				o._auxDir.xy=refl_vec;
			#endif
			o._auxDir.xy=o._auxDir.xy*0.5+0.5;
		#endif
		#if defined(RTP_WETNESS)
		o._auxDir.zw = ( mul (rotation, mul(_World2Object, float4(0,1,0,0)).xyz) ).xy;		
		#endif
		
		float3 Wpos=mul(_Object2World, v.vertex);
		o.custUV.zw=Wpos.xz-_TERRAIN_PosSize.xy;
		o.custUV.zw/=_TERRAIN_PosSize.zw;
		
		//float far=saturate((o._uv_Relief.w - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		//v.normal.xyz=lerp(v.normal.xyz, float3(0,1,0), far*_FarNormalDamp);		
	}
	
	void surf (Input IN, inout SurfaceOutput o) {
		float _distance=length(_WorldSpaceCameraPos.xyz-IN.worldPos);
		float2 terrainTiling=IN.worldPos.xz-_TERRAIN_PosSize.xy+_TERRAIN_Tiling.zw;
			
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)
		float _uv_Relief_z=saturate((_distance - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
		_uv_Relief_z=1-_uv_Relief_z;
		float _uv_Relief_w=saturate((_distance - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		#endif
		
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN)
		float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
		float4 global_color_value=tex2D(_ColorMapGlobal, IN.custUV.zw);
		#ifdef SIMPLE_FAR	
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, lerp(_GlobalColorMapSaturation,saturate(_GlobalColorMapSaturation*1.3+0.2),_uv_Relief_w));
		#else
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
		#endif
		#endif
			
      	float2 tH=tex2D(_HeightMapCombined, IN.custUV.xy).rg;
      	float2 uv=IN.custUV.xy + ParallaxOffset(tH.x, TERRAIN_ExtrudeHeight, IN.viewDir.xyz);
      	float2 uv2=IN.custUV.xy + ParallaxOffset(tH.y, TERRAIN_ExtrudeHeight, IN.viewDir.xyz);
      	float2 control=float2(IN.color.r, 1-IN.color.r);
      	control*=(tH+0.01);
      	float2 control_orig=control;
      	control*=control;
      	control*=control;
      	control*=control;
      	control/=dot(control, 1);
      		
      	float3 rayPos;
      	rayPos.z=dot(control, tH);
      	#if defined(RTP_SIMPLE_SHADING)
      	rayPos.xy=IN.custUV.xy;
      	#else
      	rayPos.xy=lerp(uv, uv2, control.y);
      	#endif
      	
		float3 flat_dir;
		flat_dir.xy=IN._auxDir.zw;
		flat_dir.z=sqrt(1 - saturate(dot(flat_dir.xy, flat_dir.xy)));
		float wetSlope=1-flat_dir.z;

		float perlinmask=tex2D(_BumpMapPerlin, terrainTiling/8).r;
		
		#if defined(VERTEX_COLOR_TO_WATER_COVERAGE)
		TERRAIN_LayerWetStrength*=saturate(2 - VERTEX_COLOR_TO_WATER_COVERAGE*2 - perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
		#else
		TERRAIN_LayerWetStrength*=saturate(2 - tex2D(_BumpMapPerlin, IN.custUV.zw).b*2 - perlinmask*(1-TERRAIN_LayerWetStrength*TERRAIN_GlobalWetness)*2)*TERRAIN_GlobalWetness;
		#endif
		float2 roff=0;
		wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
		float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4);
		TERRAIN_RainIntensity*=_RippleDamp;
		TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
		TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
		TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*rayPos.z);
		TERRAIN_Flow*=TERRAIN_LayerWetStrength*TERRAIN_LayerWetStrength;
		
		float p = saturate((TERRAIN_WaterLevel-rayPos.z)*TERRAIN_WaterEdge);
		p*=p;
		#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
			float2 flowUV=lerp(IN.custUV.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
			float _Tim=frac(_Time.x*4)*2;
			float ft=abs(frac(_Tim)*2 - 1);
			float2 flowSpeed=clamp((IN._auxDir.zw)*4,-1,1)/4;
			float4 vec=tex2D(_FlowMap, flowUV)*2-1;
			flowSpeed=lerp(vec.zw, vec.xy, IN.color.r)*float2(-1,1)*TERRAIN_FlowSpeedMap+flowSpeed;
			flowUV*=TERRAIN_FlowScale;
			flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
			float2 flowOffset=tex2D(_BumpMapPerlin, flowUV+frac(_Tim.xx)*flowSpeed).rg*2-1;
			flowOffset=lerp(flowOffset, tex2D(_BumpMapPerlin, flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.1).rg*2-1, ft);
			flowOffset*=TERRAIN_Flow*max(p, TERRAIN_WetSpecularity)*TERRAIN_LayerWetStrength;
		#else
			float2 flowOffset=0;
			float2 flowSpeed=0;
		#endif
		
		#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
			float2 rippleUV = IN.custUV.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
		  	roff = GetRipple( rippleUV, TERRAIN_RainIntensity);
			roff += GetRipple( rippleUV+float2(0.25,0.25), TERRAIN_RainIntensity);
		  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
		  	roff+=flowOffset;
		#else
			roff = flowOffset;
		#endif
		
		#if !defined(RTP_SIMPLE_SHADING)
			rayPos.xy+=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
		#endif
		
      	fixed4 col = control.x*tex2D(_MainTex, rayPos.xy) + control.y*tex2D(_MainTex2, rayPos.xy);
        o.Gloss = col.a;
        float GlossDry=o.Gloss;
        
        o.Gloss = lerp(max(o.Gloss, TERRAIN_WetSpecularity*saturate(TERRAIN_LayerWetStrength*2-0.25)), TERRAIN_WaterGloss, p);
        o.Specular = lerp(_Shininess, 0.03+TERRAIN_WaterSpecularity, TERRAIN_LayerWetStrength);
        
        #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
	        half rim = saturate(1.0+TERRAIN_FresnelOffset - dot (normalize(IN.viewDir), float3(roff,1) ));
	        rim*=saturate(pow (rim, TERRAIN_FresnelPow));
        #endif

        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p*lerp(rim,1,TERRAIN_WaterColor.a));
        float _WaterOpacity=TERRAIN_WaterOpacity*p;

        col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
        
		float3 n;
		float4 normals_combined = tex2D(_BumpMapCombined, rayPos.xy).rgba*control.xxyy;
		n.xy=(normals_combined.rg+normals_combined.ba)*2-1;
		n.z = sqrt(1 - saturate(dot(n.xy, n.xy)));
        o.Normal = lerp(n, float3(0,0,1), max(p*0.7, TERRAIN_WaterOpacity));
        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
        o.Normal.xy+=roff;
        //o.Normal=normalize(o.Normal);
  		
		col.rgb*=1-saturate(TERRAIN_LayerWetStrength*2)*0.3;
                
        o.Albedo = col.rgb;
		#ifdef SIMPLE_FAR      	
		o.Albedo=lerp(o.Albedo, global_color_value.rgb, _uv_Relief_w);
		#endif
		
      	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			o.Albedo=lerp(o.Albedo, o.Albedo*global_color_value.rgb*2, global_color_blend);
		#else
			o.Albedo=lerp(o.Albedo, global_color_value.rgb, global_color_blend);
		#endif      	
		#endif
		
      	#ifdef GLOBAL_PERLIN
	      	//float perlinmask=tex2D(_BumpMapGlobal, terrainTiling/8).r;
			//float4 global_bump_val=tex2D(_BumpMapPerlin, terrainTiling*_BumpMapGlobalScale);
			float4 global_bump_val=tex2D(_BumpMapPerlin, IN.custUV.xy*_BumpMapGlobalScale);
	
			float3 norm_far;
			norm_far.xy = global_bump_val.rg*2-1;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));      	
			
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*(_BumpMapPerlinBlendA*control.x + _BumpMapPerlinBlendB*control.y);	
		#endif
		o.Normal=normalize(o.Normal);
      			  		
	    #if defined(RTP_REFLECTION)
			float t=tex2D(_BumpMapPerlin, IN._auxDir.xy + o.Normal.xy*TERRAIN_ReflDistortion).a;
			#ifdef RTP_WETNESS
				float ReflectionStrength=max(TERRAIN_LayerReflection, TERRAIN_WetReflection*TERRAIN_LayerWetStrength);
			#else
				float ReflectionStrength=TERRAIN_LayerReflection;
			#endif
			rim*=max(p*saturate(TERRAIN_WaterGloss+0.3), lerp(GlossDry*saturate(ReflectionStrength), saturate(GlossDry+ReflectionStrength-1), saturate(ReflectionStrength-1)) );
			
			o.Emission = TERRAIN_ReflColorA.rgb*rim*t; // *TERRAIN_ReflColorA.a
			o.Albedo = lerp(o.Albedo, TERRAIN_ReflColorB.rgb, TERRAIN_ReflColorB.a*rim*(1-t));
		#endif   
		
		#if defined(BLENDING_HEIGHT)
			flaot4 terrain_coverage=tex2D(_TERRAIN_Control, IN.custUV.zw);
			float4 splat_control1=terrain_coverage * tex2D(_TERRAIN_HeightMap, IN.aux.xy) * IN.color.a;
			float4 splat_control2=float4(control_orig, 0, 0, 0) * (1-IN.color.a);
			
			float blend_coverage=dot(terrain_coverage, 1);
			if (blend_coverage>0.1) {
			
				splat_control1*=splat_control1;
				splat_control1*=splat_control1;
				splat_control2*=splat_control2;
				splat_control2*=splat_control2;
				
				float normalize_sum=dot(splat_control1, 1)+dot(splat_control2, 1);
				splat_control1 /= normalize_sum;
				splat_control2 /= normalize_sum;		
				
				o.Alpha=dot(splat_control2,1);
				o.Alpha=lerp(1-IN.color.a, o.Alpha, saturate((blend_coverage-0.1)*4) );
			} else {
				o.Alpha=1-IN.color.a;
			}
		#else
			o.Alpha=1-IN.color.a;
		#endif
		#if defined(UNITY_PASS_PREPASSFINAL)
			o.Gloss*=o.Alpha;
		#endif	
		o.Albedo*=_MainColor*2;		
	}
	ENDCG
      
    } 
	FallBack "Diffuse"
}
