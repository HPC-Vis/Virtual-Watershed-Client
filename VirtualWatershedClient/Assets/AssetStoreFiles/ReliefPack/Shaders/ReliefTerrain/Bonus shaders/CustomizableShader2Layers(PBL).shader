//
// 2 Parallax mapped materials with adjustable features
// Tomasz Stobierski 2014
//
// NOTE - by default most features are disabled
// You can enable them below uncommenting property block associated with a feature and the feature itself by uncommenting its #define keyword
// So, to make any shader variation:
// 1. copy shader code into new shader file
// 2. adjust its filename and shader name below (using 20 shaders with name like "Relief Pack/Customizable Shader 2 Layers (PBL)" would be confusing)
// 3. comment/uncomment property blocks for features you'd like to use
// 4. comment/uncomment #define keywords to enable/disable features
//
// note that there are some important switches in #define section (HDR cubemaps switch, linear lighting switch and so on)
//
Shader "Relief Pack/Customizable Shader 2 Layers (PBL)" {
    Properties {
		_SpecColor ("Specular Color (RGBA)", Color) = (0.5, 0.5, 0.5, 1)		
		// takes effect only with RTP_PM_SHADING defined below
		_ExtrudeHeight ("Extrude Height", Range(0.001,0.1)) = 0.04
				
				
				
		//
		// complementary lighting
		//
		// look for RTP_COMPLEMENTARY_LIGHTS define below
		//  (using complementary lighting you probably don't need IBL diffuse)
		// (comment below properties when the feature is enabled and you want to get it synced globally by RTP scripts)
		RTP_ReflexLightDiffuseSoftness ("Complementary diffuse softness", Range(0,1)) = 0.5
		RTP_ReflexLightDiffuseColor1 ("Complementary light diffuse 1 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_ReflexLightDiffuseColor2 ("Complementary light diffuse 2 (RGB+A strength)", Color) = (1, 1, 1, 0.05)
		RTP_BackLightStrength ("Negative light power", Range(0,1)) = 0
		
		rtp_customAmbientCorrection ("Ambient correction", Color) = (0, 0, 0, 1)				



    	//
    	// main textures
    	//    
		_MainTex ("Texture A", 2D) = "white" {}
		_BumpMap ("Bumpmap A", 2D) = "bump" {}
		_HeightMap ("Heightmap A", 2D) = "black" {}
		
		_MainTex2 ("Texture B", 2D) = "white" {}
		_BumpMap2 ("Bumpmap B", 2D) = "bump" {}
		_HeightMap2 ("Heightmap B", 2D) = "black" {}
		
		
		
		// near distance values (used with global colormap, perlin normals or uv blend feature)
		// UNCOMMENT when using one of these features or you won't get control over many features)
/*
		_TERRAIN_distance_start ("Near distance start", Float) = 0
		_TERRAIN_distance_transition ("       fade length", Float) = 20
*/
		// far distance values
		_TERRAIN_distance_start_bumpglobal ("Far distance start", Float) = 0
		_TERRAIN_distance_transition_bumpglobal ("       distance transition", Float) = 50



		//
		// global colormap
		// to enable/disable feature in shader - look for COLOR_MAP define below
		//
/*		
		_ColorMapGlobal ("Global colormap", 2D) = "grey" {}
		// uncomment when shader used w/o RTP
		_GlobalColorMapBlendValues ("       blending (near,mid,far,-)", Vector) = (0.3,0.6,0.8,0)
		_GlobalColorMapSaturation ("       saturation near", Range(0,1)) = 1	
		_GlobalColorMapSaturationFar ("       saturation far", Range(0,1)) = 1
		_GlobalColorMapBrightness ("       brightness near", Range(0,2)) = 1
		_GlobalColorMapBrightnessFar ("       brightness far", Range(0,2)) = 1		
		_GlobalColorMapNearMIP ("       MIP level below far", Range(0, 8)) = 0
		_GlobalColorMapDistortByPerlin ("       Distort by perlin", Range(0, 0.02)) = 0.01
		////////
*/
	
	
	
		//
		// perlin normalmap (RG combined with wetmask in B channel and reflection map in A channel)
		// to enable/disable feature in shader - look for GLOBAL_PERLIN define below
		//
/*
		_BumpMapGlobal ("Combined texture (RG perlin, B wet, A refl)", 2D) = "black" {}
		rtp_perlin_start_val ("       Perlin start val", Range(0,1)) = 0.3
		_BumpMapGlobalScale ("       Perlin tiling scale", Range( 0.01,0.25) ) = 0.1
		_BumpMapGlobalStrength0 ("       Layer 1 perlin normal", Range(0,2)) = 0.3
		_BumpMapGlobalStrength1 ("       Layer 2 perlin normal", Range(0,2)) = 0.3
*/
	
	
	
    	//
		// per layer adjustement PBL
		//
		// Layer 1
		_LayerColor0 ("Layer 1 color", Color) = (0.5,0.5,0.5,1)
		_LayerSaturation0 ("       saturation", Range(0,2)) = 1
		_Spec0 ("       spec multiplier", Range(0,4)) = 1
		RTP_gloss_mult0 ("       gloss multiplier", Range(0,4)) = 1
		RTP_gloss2mask0 ("       spec mask from gloss", Range(0,1)) = 0
		_LayerBrightness2Spec0 ("       spec mask from albedo", Range(0,1)) = 0
		RTP_gloss_shaping0 ("       gloss shaping", Range(0,1)) = 0.5
		_FarSpecCorrection0 ("       far spec correction", Range(-1,1)) = 0
		RTP_Fresnel0 ("       fresnel", Range(0,1)) = 0
		RTP_FresnelAtten0 ("           fresnel attenuate by gloss", Range(0,1)) = 0
		RTP_DiffFresnel0 ("       diffuse scattering", Range(0,1)) = 0
		RTP_IBL_bump_smoothness0 ("       IBL / Refl bump smooth", Range(0,1)) = 0.7
		RTP_IBL_SpecStrength0 ("       IBL spec / Refl exposure", Range(0,8)) = 1
		// in Unity5 below feature makes no sense when we don't use noambient keyword
		RTP_IBL_DiffuseStrength0 ("       IBL diffuse exposure", Range(0,2)) = 1
		_LayerAlbedo2SpecColor0 ("          color from albedo (metal tint)", Range(0,1)) = 0
		// UV blend layer1
		// to enable/disable feature in shader - look for RTP_UV_BLEND define below
/*
		_MixBlend0 ("Layer1 UV blend", Range(0,1)) = 0.5
		_MixScale0 ("     tiling", Range(0.02,0.25)) = 0.125
		_MixSaturation0 ("     saturation", Range(0,2)) = 1
		_MixBrightness0 ("     brightness", Range(0.5,3.5)) = 2
		_MixReplace0 ("     replace at far", Range(0,1)) = 0.25
*/
	
		// Layer 2
		_LayerColor1 ("Layer 2 color", Color) = (0.5,0.5,0.5,1)
		_LayerSaturation1 ("       saturation", Range(0,2)) = 1
		_Spec1 ("       spec multiplier", Range(0,4)) = 1
		RTP_gloss_mult1 ("       gloss multiplier", Range(0,4)) = 1
		RTP_gloss2mask1 ("       spec mask from gloss", Range(0,1)) = 0
		_LayerBrightness2Spec1 ("       spec mask from albedo", Range(0,1)) = 0
		RTP_gloss_shaping1 ("       gloss shaping", Range(0,1)) = 0.5
		_FarSpecCorrection1 ("       far spec correction", Range(-1,1)) = 0
		RTP_Fresnel1 ("       fresnel", Range(0,1)) = 0
		RTP_FresnelAtten1 ("           fresnel attenuate by gloss", Range(0,1)) = 0
		RTP_DiffFresnel1 ("       diffuse scattering", Range(0,1)) = 0
		RTP_IBL_bump_smoothness1 ("       IBL / Refl bump smooth", Range(0,1)) = 0.7
		RTP_IBL_SpecStrength1 ("       IBL spec / Refl exposure", Range(0,8)) = 1
		// in Unity5 below feature makes no sense when we don't use noambient keyword
		RTP_IBL_DiffuseStrength1 ("       IBL diffuse exposure", Range(0,2)) = 1
		_LayerAlbedo2SpecColor1 ("          color from albedo (metal tint)", Range(0,1)) = 0
		// UV blend layer2
		// to enable/disable feature in shader - look for RTP_UV_BLEND define below		_MixBlend1 ("Layer2 UV blend", Range(0,1)) = 0.5
/*
		_MixScale1 ("     tiling", Range(0.02,0.25)) = 0.125
		_MixSaturation1 ("     saturation", Range(0,2)) = 1
		_MixBrightness1 ("     brightness", Range(0.5,3.5)) = 2
		_MixReplace1 ("     replace at far", Range(0,1)) = 0.25
*/
    	//
		// EOF per layer adjustement PBL
		//		
	
	
	
		//
		// emissive properties
		// to enable/disable feature in shader - look for RTP_EMISSION define below
		//
/*
		// Layer 1
		_LayerEmission0 ("Layer 1 emission", Range(0,1)) = 0
		_LayerEmissionColor0 ("       glow color", Color) = (0.5,0.5,0.5,0)
		// Layer 2
		_LayerEmission1 ("Layer 2 emission", Range(0,1)) = 0
		_LayerEmissionColor1 ("       glow color", Color) = (0.5,0.5,0.5,0)
*/

		//
		// water/wet
		// to enable/disable feature in shader - look for RTP_EMISSION define below
		//
/*
		TERRAIN_GlobalWetness ("Wetness", Range(0,1)) = 1
		TERRAIN_WaterLevel ("       Water level", Range(0,2)) = 0.5
		TERRAIN_WaterLevelSlopeDamp ("       Water level slope damp", Range(0.1, 2)) = 0.1
		TERRAIN_WaterEdge ("       Water level sharpness", Range(1,4)) = 1
		// when GLOBAL_PERLIN is defined below texture is ommited (flow bumps are taken from combined perlin texture)
      	TERRAIN_FlowingMap ("      Flowingmap (water bumps)", 2D) = "gray" {}
		// to enable/disable flowmap feature in shader - look for FLOWMAP define below
		//_FlowMap ("      FlowMap (RG+BA)", 2D) = "grey" {}
	  	//TERRAIN_FlowSpeedMap ("       Flow Speed (map)", Range(0, 1)) = 0.1		
		TERRAIN_FlowSpeed ("       Flow speed", Range(0,1)) = 0.5
		TERRAIN_FlowCycleScale ("       Flow cycle scale", Range(0.5,4)) = 1
		TERRAIN_FlowScale ("       Flow tex tiling", Float) = 1
		TERRAIN_FlowMipOffset ("       Flow tex filter", Range(0,4)) = 1
		TERRAIN_mipoffset_flowSpeed ("       Filter by flow speed", Range(0,4)) = 1
		TERRAIN_WetDarkening ("       Water surface darkening", Range(0.1,0.9)) = 0.5
      	
		TERRAIN_WaterColor ("       Water color (A - fresnel)", Color) = (0.5, 0.7, 1, 0.5)
		TERRAIN_WaterOpacity ("       Water opacity", Range(0,1)) = 0.2
		TERRAIN_WaterEmission ("       Water emission", Range(0,1)) = 0
		
		TERRAIN_WaterSpecularity ("       Water spec boost", Range(-1,1)) = 0.2
		TERRAIN_WaterGloss ("       Water gloss boost", Range(-1,1)) = 0.3
		
		TERRAIN_Flow ("       Flow strength", Range(0, 1)) = 0.1
		TERRAIN_Refraction ("       Water refraction", Range(0,0.04)) = 0.02
		TERRAIN_WaterIBL_SpecWaterStrength ("       IBL spec / Refl - water", Range(0,8)) = 1
		
		TERRAIN_WetSpecularity ("       Wet spec boost", Range(-1,1)) = 0.1
		TERRAIN_WetGloss ("       Wet gloss boost", Range(-1,1)) = 0.1
		
		TERRAIN_WetFlow ("       Wet flow", Range(0, 1)) = 0
		TERRAIN_WetRefraction ("       Wet refraction factor", Range(0,1)) = 0
		TERRAIN_WaterIBL_SpecWetStrength ("       IBL spec / Refl - wet", Range(0,8)) = 1
		
		TERRAIN_WaterGlossDamper ("       Hi-freq / distance gloss damper", Range(0,1)) = 0
*/
		//
		// EOF water/wet
		// 
	
							
		//
      	// rain feature
      	// to enable/disable feature in shader - look for RTP_WET_RIPPLE_TEXTURE define below (RTP_WETNESS must be defined, too)
      	//
/*
		TERRAIN_RippleMap ("Ripplemap", 2D) = "gray" {}
		TERRAIN_RainIntensity ("Rain intensity", Range(0,1)) = 1
		TERRAIN_WetDropletsStrength ("       Rain on wet", Range(0,1)) = 0.1
		TERRAIN_DropletsSpeed ("       Anim Speed", Float) = 15
		TERRAIN_RippleScale ("       Ripple tex tiling", Range(0.25,8)) = 1
*/
	
	
		//
		// static reflections (taken from A channel of _BumpMapGlobal texture)
		// to enable/disable feature in shader - look for RTP_REFLECTION define below 
		// this feature can be treated as simple (no need for cubemap texture sampler) alternative for IBL specular cubemaps
		//
/*
		TERRAIN_ReflColorA ("Reflection color A (Emissive RGB)", Color) = (0.5,0.5,0.5,0)
		TERRAIN_ReflColorB ("       color B (Diffuse RGBA)", Color) = (0.0,0.5,0.9,0)
		TERRAIN_ReflColorC ("       color C (Diffuse RGBA)", Color) = (0.3,0.6,0.9,0)
		// when GLOBAL_PERLIN is defined below texture is defined in another part of property block (reflection map is taken from A channel of combined perlin texture)
		_BumpMapGlobal ("       Refl texture (comb. texure A channel)", 2D) = "black" {}
		TERRAIN_ReflColorCenter ("       gradient center", Range(0.1, 0.9)) = 0.5
		TERRAIN_ReflGlossAttenuation ("       roughness attenuation", Range(0, 1)) = 0.5
		TERRAIN_ReflectionRotSpeed ("       Reflection rotation speed", Range(0, 2)) = 0.3
*/



		//
		// IBL
		// to enable/disable feature in shader - look for RTP_IBL_DIFFUSE and RTP_IBL_SPEC defines below  
		// (note that using IBL diffuse you probably don't need complementary lighting)
		//
/*
		// below cubemap is not used for Unity5 or when RTP_SKYSHOP_SYNC define below is used
		_DiffCubeIBL ("Custom IBL diffuse cubemap", CUBE) = "black" {}
		TERRAIN_IBL_DiffAO_Damp("Diffuse IBL AO damp", Range(0,1)) = 0
		// in case of RTP_SKYSHOP_SYNC you don't need to fill below cubemap, but you can overwrite it with custom value
		// in Unity5 it's not used either
		_SpecCubeIBL ("Custom IBL spec cubemap", CUBE) = "black" {}
		TERRAIN_IBLRefl_SpecAO_Damp("Spec IBL / Refl AO damp", Range(0,1)) = 0
*/
	
	
	
		//
		// vertical texturing
		// to enable/disable feature in shader - look for VERTICAL_TEXTURE define below  		
		//
/*
		_VerticalTexture ("Vertical texture (RGB)", 2D) = "grey" {}
		_VerticalTextureTiling ("       Texture tiling", Float) = 50
		_VerticalTextureGlobalBumpInfluence ("       Perlin distortion", Range(0,0.3)) = 0.01
		_VerticalTextureStrength0 ("       Layer 1 strength", Range(0,1)) = 0.5
		_VerticalTextureStrength1 ("       Layer 2 strength", Range(0,1)) = 0.5
*/
	
	
	
		//
		// snow (set globally by RTP scripts)
		// to enable/disable feature in shader - look for RTP_SNOW define below  		
		//
/*
		rtp_snow_strength ("Snow strength", Range(0,2)) = 1
		
		// uncomment when shader used w/o RTP
		rtp_global_color_brightness_to_snow ("       Global color influence", Range(0, 0.3)) = 0
		////////
		
		rtp_snow_slope_factor ("       Slope damp factor", Range(0,15)) = 2
		// in [m] (where snow start to appear)
		rtp_snow_height_treshold ("       Coverage height theshold", Float) = -100
		rtp_snow_height_transition ("       Coverage height length", Float) = 300
		rtp_snow_color("       Color", Color) = (0.9,0.9,1,1)
		rtp_snow_specular("       Spec (gloss mask)", Range(0, 1)) = 0.4
		rtp_snow_gloss("       Gloss", Range(0.01,1)) = 0.2
		
		rtp_snow_fresnel("       Fresnel", Range(0.01,1)) = 0.2
		rtp_snow_diff_fresnel("       Diffuse scattering", Range(0,2)) = 0.5
		rtp_snow_IBL_DiffuseStrength("       IBL diffuse exposure", Range(0,2)) = 0.25
		rtp_snow_IBL_SpecStrength("       IBL spec / Refl exposure", Range(0,8)) = 0.25
		
		rtp_snow_edge_definition ("       Edges definition", Range(0.25,20)) = 2
		//rtp_snow_deep_factor("       Deep factor", Range(0,6)) = 2 // not used in shader
*/



		//
		// caustics
		// to enable/disable feature in shader - look for RTP_CAUSTICS define below  		
		//
/*
		TERRAIN_CausticsAnimSpeed(" Caustics anim speed", Range(0, 10)) = 2
		TERRAIN_CausticsColor("       Color (RGB)", Color) = (1,1,1,0)
		TERRAIN_CausticsWaterLevel("       Water Level", Float) = 0
		TERRAIN_CausticsWaterLevelByAngle("       Water level by slope", Range(0,8)) = 4
		TERRAIN_CausticsWaterShallowFadeLength("       Shallow fade length", Range(0.1, 10)) = 1
		TERRAIN_CausticsWaterDeepFadeLength("       Deep fade length", Range(1, 100)) = 20
		TERRAIN_CausticsTilingScale("       Texture tiling", Float) = 1 //Range(0.5, 4)) = 2
		TERRAIN_CausticsTex("       Caustics texture", 2D) = "black" {}
*/
  
  
  
		//
		// global ambient emissive map
		// to enable/disable feature in shader - look for RTP_AMBIENT_EMISSIVE_MAP define below  		
		//
/*
		_AmbientEmissiveMapGlobal("Global ambient emissive map", 2D) = "black" {}
		_AmbientEmissiveMultiplier("       emission brightness", Range(0,4)) = 0.5
		_AmbientEmissiveRelief("       normal/height mod", Range(0,1)) = 0.5
		_shadow_distance_start ("       shadows distance", Float) = 20
		_shadow_distance_transition ("       shadows fade length", Range(0,30)) = 30
		_shadow_value ("       shadows blending", Range(0,1)) = 0
*/

   
         
    	// params for underlying terrain (used for global maps and in geom blend mode)
 		//_TERRAIN_PosSize ("Terrain pos (xz to XY) & size(xz to ZW)", Vector) = (0,0,1000,1000)
		//_TERRAIN_Tiling ("Terrain tiling (XY) & offset(ZW)", Vector) = (3,3,0,0)       
    }
    SubShader {
	Tags { "RenderType" = "Opaque" }
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert

	#pragma exclude_renderers flash
	#pragma glsl
	#pragma target 3.0

	// define either PM or simple shading mode. You can also compile 2 versions to be switched runtime (globally by RTP LOD manager script)
	#define RTP_PM_SHADING
	//#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING

	// complementary lights (you'll probably want to turn it off every time you use IBL diffuse)	
	#define RTP_COMPLEMENTARY_LIGHTS
	// AO can be taken from arbitrary vertex color channel
	#define VERTEX_COLOR_AO_DAMP IN.color.g
	// switch to choose we're render in linear or gamma
	//#define RTP_COLORSPACE_LINEAR
	
	// comment if you don't need global color map
	//#define COLOR_MAP
	// if not defined global color map will be blended (lerp)
	//#define COLOR_MAP_BLEND_MULTIPLY
	
	// makes usage of _BumpMapGlobal texture (RG - perlin bumpmap, B watermask, A - reflection map)
	// practical when used on larger areas of geom blend, this switch makes also perlin global texture to be used as water bumpmaps
	//#define GLOBAL_PERLIN
	
	// uv blending
	//#define RTP_UV_BLEND

	// water features	
	//#define RTP_WETNESS
	// enable below if you don't want to use water flow
	//#define SIMPLE_WATER
	// rain droplets
	//#define RTP_WET_RIPPLE_TEXTURE	  
	// if defined we don't use terrain wetmask (_BumpMapGlobal B channel), but B channel of vertex color
	// (to get it from combined texture B channel you need to define GLOBAL_PERLIN)
	#define VERTEX_COLOR_TO_WATER_COVERAGE IN.color.b	
	// you can ask shader to use flowmap to control direction of flow (can be dependent on this flowmap along uv coords defined there)
	//#define FLOWMAP

	
	// IBL lighting - diffuse cubemap used (you'll probably want to turn it off every time you use complementary lights)	
	// in Unity5 below feature makes no sense when we don't use noambient keyword
	//#define RTP_IBL_DIFFUSE
	// IBL lighting - specular / reflections cubemap used
	//#define RTP_IBL_SPEC
	// if not defined we will decode LDR cubemaps (RGB only)
	#define IBL_HDR_RGBM
	// if you'd like to integrate Skyshop IBL features (global Sky object)
	//#define RTP_SKYSHOP_SYNC	
	// use if you'd like global skyshop's cubemaps to be rotates accordingly to skyshop settings
	//#define RTP_SKYSHOP_SKY_ROTATION	
	
	// static reflection (A channel of  _BumpMapGlobal texture)
	//#define RTP_REFLECTION
	// if RTP_REFLECTION is enabled - below define control rotation "skymap" around
	#define RTP_ROTATE_REFLECTION
	
	//  layer emissiveness
	//#define RTP_EMISSION
	// when wetness is defined and fuild on surface is emissive we can mod its emisiveness by output normal (wrinkles of flowing "water")
	// below define change the way we treat output normals (works fine for "lava" like emissive fuilds)
	//#define RTP_FUILD_EMISSION_WRAP	
	
	// vertical texturing
	//#define VERTICAL_TEXTURE
	
	// dynamic snow
	//#define RTP_SNOW
	// you can optionally define source vertex color to control snow coverage (useful when you need to mask snow under any kind of shelters)
	//#define VERTEX_COLOR_TO_SNOW_COVERAGE IN.color.a

	// caustics	
	//#define RTP_CAUSTICS

	// PBL visibility function - not necessary if you'd like to configure fast shader (makes shader more physically accurate - boosts a bit spec behaviour in sereval conditions)
	#define RTP_PBL_VISIBILITY_FUNCTION
	// fresnel used for direct lighting (in forward only !)
	#define RTP_PBL_FRESNEL
	// when you don't use solution like Lux which handles PBL in deferred, leave below define uncommented
	#define RTP_DEFERRED_PBL_NORMALISATION
	
	// global ambient emissive map
	//#define RTP_AMBIENT_EMISSIVE_MAP
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	

	#define TWO_LAYERS
	//#define NOSPEC_BLEED
	
#ifdef RTP_SKYSHOP_SYNC
	// SH IBL lighting taken under permission from Skyshop MarmosetCore.cginc
	uniform float3		_SH0;
	uniform float3		_SH1;
	uniform float3		_SH2;
	uniform float3		_SH3;
	uniform float3		_SH4;
	uniform float3		_SH5;
	uniform float3		_SH6;
	uniform float3		_SH7;
	uniform float3		_SH8;	
	float3 SHLookup(float3 dir) {
		//l = 0 band (constant)
		float3 result = _SH0.xyz;

		//l = 1 band
		result += _SH1.xyz * dir.y;
		result += _SH2.xyz * dir.z;
		result += _SH3.xyz * dir.x;

		//l = 2 band
		float3 swz = dir.yyz * dir.xzx;
		result += _SH4.xyz * swz.x;
		result += _SH5.xyz * swz.y;
		result += _SH7.xyz * swz.z;
		float3 sqr = dir * dir;
		result += _SH6.xyz * ( 3.0*sqr.z - 1.0 );
		result += _SH8.xyz * ( sqr.x - sqr.y );
		
		return abs(result);
	}	
	uniform float4 _ExposureIBL;	
#endif
	
	struct Input {
		float4 texCoords_FlatRef;

		float3 worldPos;
		float3 viewDir;
		float3 worldNormal;
		float3 worldRefl;
		INTERNAL_DATA
		fixed4 color:COLOR;
	};
	
	float RTP_BackLightStrength;
	fixed3 rtp_customAmbientCorrection;
	float _ExtrudeHeight;
	
	//
	// complementary lighting (uncomment property block when used)
	//
	float RTP_ReflexLightDiffuseSoftness;
	fixed4 RTP_ReflexLightDiffuseColor1;
	fixed4 RTP_ReflexLightDiffuseColor2;

	//
	// main section
	//    
	sampler2D _MainTex;
	sampler2D _BumpMap;
	sampler2D _HeightMap;
	sampler2D _MainTex2;
	sampler2D _BumpMap2;
	sampler2D _HeightMap2;
	
	//
	// per layer adjustement PBL
	//
	// Layer 1
	fixed3 _LayerColor0;
	float _LayerSaturation0;
	float _Spec0; 
	float RTP_gloss_mult0;
	float RTP_gloss2mask0;
	float _LayerBrightness2Spec0;
	float RTP_gloss_shaping0;

	float _FarSpecCorrection0;
	float RTP_Fresnel0;
	float RTP_FresnelAtten0;
	float RTP_DiffFresnel0;
	float RTP_IBL_bump_smoothness0;
	float RTP_IBL_SpecStrength0;
	float RTP_IBL_DiffuseStrength0;
	float _LayerAlbedo2SpecColor0;
			
	// Layer 2
	fixed3 _LayerColor1;
	float _LayerSaturation1;
	float _Spec1; 
	float RTP_gloss_mult1;
	float RTP_gloss2mask1;
	float _LayerBrightness2Spec1;
	float RTP_gloss_shaping1;
			
	float _FarSpecCorrection1;
	float RTP_Fresnel1;
	float RTP_FresnelAtten1;
	float RTP_DiffFresnel1;
	float RTP_IBL_bump_smoothness1;
	float RTP_IBL_SpecStrength1;
	float RTP_IBL_DiffuseStrength1;
	float _LayerAlbedo2SpecColor1;
	//
	// EOF per layer adjustement PBL
	//		
	
	//
	// emissive properties
	//
	// Layer 1
	float _LayerEmission0;
	fixed3 _LayerEmissionColor0;
	// Layer 2
	float _LayerEmission1;
	fixed3 _LayerEmissionColor1;

	//
	// water/wet
	//
	float TERRAIN_GlobalWetness;
	// general flow direction
	sampler2D _FlowMap;
  	sampler2D TERRAIN_FlowingMap;
	float TERRAIN_FlowSpeed;
  	float TERRAIN_FlowSpeedMap;
	float TERRAIN_FlowCycleScale;
	float TERRAIN_FlowScale;
	float TERRAIN_FlowMipOffset;
	float TERRAIN_mipoffset_flowSpeed;
	float TERRAIN_WetDarkening;
  	
	fixed4 TERRAIN_WaterColor;
	float TERRAIN_WaterOpacity;
	float TERRAIN_WaterEmission;
	
	float TERRAIN_WaterLevel;
	float TERRAIN_WaterLevelSlopeDamp;
	float TERRAIN_WaterEdge;
	
	float TERRAIN_WaterSpecularity;
	float TERRAIN_WaterGloss;
	float TERRAIN_WaterGlossDamper;
	
	float TERRAIN_Flow;
	float TERRAIN_Refraction;
	float TERRAIN_WaterIBL_SpecWaterStrength;
	
	float TERRAIN_WetSpecularity;
	float TERRAIN_WetGloss;
	
	float TERRAIN_WetFlow;
	float TERRAIN_WetRefraction;
	float TERRAIN_WaterIBL_SpecWetStrength;
	//
	// EOF water/wet
	// 	
	
	//
  	// rain feature
  	//
	sampler2D TERRAIN_RippleMap;
	float TERRAIN_RainIntensity;
	float TERRAIN_WetDropletsStrength;
	float TERRAIN_DropletsSpeed;
	float TERRAIN_RippleScale;
	
	//
	// colormap global
	//
	sampler2D _ColorMapGlobal;
	// can be set globaly by ReliefTerrain script
	float4 _GlobalColorMapBlendValues;
	float _GlobalColorMapNearMIP; 
	float _GlobalColorMapDistortByPerlin; 
	float _GlobalColorMapSaturation;
	float _GlobalColorMapSaturationFar	; 
	float _GlobalColorMapBrightness;
	float _GlobalColorMapBrightnessFar;
	
	//
	// perlin ( + watermask & reflection)
	//
	sampler2D _BumpMapGlobal;
	float4 _BumpMapGlobal_TexelSize;
	float _BumpMapGlobalScale;
	float _BumpMapGlobalStrength0;
	float _BumpMapGlobalStrength1;
	float rtp_perlin_start_val;		
	
	//
	// UV blend
	//
	float _MixBlend0;
	float _MixBlend1;
	float _MixScale0;
	float _MixScale1;
	float _MixSaturation0;
	float _MixSaturation1;
	float _MixBrightness0;
	float _MixBrightness1;
	float _MixReplace0;
	float _MixReplace1;
	
	//
	// reflection
	//
	fixed4 TERRAIN_ReflColorA;
	fixed4 TERRAIN_ReflColorB;
	fixed4 TERRAIN_ReflColorC;
	float TERRAIN_ReflColorCenter;
	float TERRAIN_ReflGlossAttenuation;
	float TERRAIN_ReflectionRotSpeed;

	//
	// IBL
	//
	samplerCUBE _DiffCubeIBL;
	samplerCUBE _SpecCubeIBL;
	float TERRAIN_IBL_DiffAO_Damp;
	float TERRAIN_IBLRefl_SpecAO_Damp;
	float4x4	_SkyMatrix;// set globaly by skyshop		
	
	sampler2D _VerticalTexture;
	float _VerticalTextureTiling;
	float _VerticalTextureGlobalBumpInfluence;
	float _VerticalTextureStrength0;
	float _VerticalTextureStrength1;
	
	//
	// snow
	//
	float rtp_global_color_brightness_to_snow;
	float rtp_snow_strength;
	float rtp_snow_slope_factor;
	// in [m] (where snow start to appear)
	float rtp_snow_height_treshold;
	float rtp_snow_height_transition;
	fixed4 rtp_snow_color;
	float rtp_snow_specular;
	float rtp_snow_gloss;
	
	float rtp_snow_fresnel;
	float rtp_snow_diff_fresnel;
	float rtp_snow_IBL_DiffuseStrength;
	float rtp_snow_IBL_SpecStrength;
	
	float rtp_snow_edge_definition;
	float rtp_snow_deep_factor;

	//
	// caustics
	//
	float TERRAIN_CausticsAnimSpeed;
	fixed4 TERRAIN_CausticsColor;
	float TERRAIN_CausticsWaterLevel;
	float TERRAIN_CausticsWaterLevelByAngle;
	float TERRAIN_CausticsWaterShallowFadeLength;
	float TERRAIN_CausticsWaterDeepFadeLength;
	float TERRAIN_CausticsTilingScale;
	sampler2D TERRAIN_CausticsTex;
	
	//
	// global ambient emissive map
	//
	sampler2D _AmbientEmissiveMapGlobal;
	float _AmbientEmissiveMultiplier;
	float _AmbientEmissiveRelief;
	float _shadow_distance_start;
	float _shadow_distance_transition;
	float _shadow_value;
		
	// RTP terrain specific
	float _TERRAIN_distance_start;
	float _TERRAIN_distance_transition;
	float _TERRAIN_distance_start_bumpglobal;
	float _TERRAIN_distance_transition_bumpglobal;			

	// used for global maps and height blend	
	float4 _TERRAIN_PosSize;
	float4 _TERRAIN_Tiling;
	sampler2D _TERRAIN_HeightMap;
	sampler2D _TERRAIN_Control;		
	
struct RTPSurfaceOutput {
	fixed3 Albedo;
	fixed3 Normal;
	fixed3 Emission;
	half Specular;
	fixed Alpha;
	float2 RTP;
	half3 SpecColor;
};

	#include "Assets/ReliefPack/Shaders/CustomLighting.cginc"
	#include "Assets/ReliefPack/Shaders/ConfigurableCore.cginc"
	ENDCG
      
    } 
    Fallback "Diffuse"
}
