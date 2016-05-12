//
// file copied from U5.3.1f1 shaders
// we add only tweaks for additional RTP surface output from structure
//

#ifndef UNITY_PBS_LIGHTING_INCLUDED
#define UNITY_PBS_LIGHTING_INCLUDED

//
// explicitly set mid-level PBS lighting function, because for terrain hi-end GGX can give unpleasant results for rough surfaces (overbrighten specular visibility term)
//
#define UNITY_BRDF_PBS BRDF2_Unity_PBS

#include "UnityShaderVariables.cginc"
#include "UnityStandardConfig.cginc"
#include "UnityLightingCommon.cginc"
#include "UnityGlobalIllumination.cginc"

///////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// custom RTP dielectric constants (users complain about too high reflectivity which gives washed out colors for rough surfaces)
//
half3 RTP_ColorSpaceDielectricSpecTint;

inline half3 DiffuseAndSpecularFromMetallicMod(half3 albedo, half metallic, out half3 specColor, out half oneMinusReflectivity)
{
	specColor = lerp(unity_ColorSpaceDielectricSpec.rgb * RTP_ColorSpaceDielectricSpecTint.rgb, albedo, metallic);
	oneMinusReflectivity = 1 - specColor.r;// SpecularStrength(specColor);
	return albedo * oneMinusReflectivity;
}
//////////////////////////////////////////////////////////////////////////////////////////////////////////


//-------------------------------------------------------------------------------------
// Default BRDF to use:
#if !defined (UNITY_BRDF_PBS) // allow to explicitly override BRDF in custom shader
// still add safe net for low shader models, otherwise we might end up with shaders failing to compile
// the only exception is WebGL in 5.3 - it will be built with shader target 2.0 but we want it to get rid of constraints, as it is effectively desktop
#if SHADER_TARGET < 30 && !UNITY_53_SPECIFIC_TARGET_WEBGL
#define UNITY_BRDF_PBS BRDF3_Unity_PBS
#elif UNITY_PBS_USE_BRDF3
#define UNITY_BRDF_PBS BRDF3_Unity_PBS
#elif UNITY_PBS_USE_BRDF2
#define UNITY_BRDF_PBS BRDF2_Unity_PBS
#elif UNITY_PBS_USE_BRDF1
#define UNITY_BRDF_PBS BRDF1_Unity_PBS
#elif defined(SHADER_TARGET_SURFACE_ANALYSIS)
// we do preprocess pass during shader analysis and we dont actually care about brdf as we need only inputs/outputs
#define UNITY_BRDF_PBS BRDF1_Unity_PBS
#else
#error something broke in auto-choosing BRDF
#endif
#endif


//-------------------------------------------------------------------------------------
// BRDF for lights extracted from *indirect* directional lightmaps (baked and realtime).
// Baked directional lightmap with *direct* light uses UNITY_BRDF_PBS.
// For better quality change to BRDF1_Unity_PBS.
// No directional lightmaps in SM2.0.

#if !defined(UNITY_BRDF_PBS_LIGHTMAP_INDIRECT)
#define UNITY_BRDF_PBS_LIGHTMAP_INDIRECT BRDF2_Unity_PBS
#endif
#if !defined (UNITY_BRDF_GI)
#define UNITY_BRDF_GI BRDF_Unity_Indirect
#endif

//-------------------------------------------------------------------------------------


inline half3 BRDF_Unity_Indirect(half3 baseColor, half3 specColor, half oneMinusReflectivity, half oneMinusRoughness, half3 normal, half3 viewDir, half occlusion, UnityGI gi)
{
	half3 c = 0;
#if defined(DIRLIGHTMAP_SEPARATE)
	gi.indirect.diffuse = 0;
	gi.indirect.specular = 0;

#ifdef LIGHTMAP_ON
	c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT(baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light2, gi.indirect).rgb * occlusion;
#endif
#ifdef DYNAMICLIGHTMAP_ON
	c += UNITY_BRDF_PBS_LIGHTMAP_INDIRECT(baseColor, specColor, oneMinusReflectivity, oneMinusRoughness, normal, viewDir, gi.light3, gi.indirect).rgb * occlusion;
#endif
#endif
	return c;
}

//-------------------------------------------------------------------------------------

// little helpers for GI calculation

#define UNITY_GLOSSY_ENV_FROM_SURFACE(x, s, data)				\
	Unity_GlossyEnvironmentData g;								\
	g.roughness		= 1 - s.Smoothness;							\
	g.reflUVW		= reflect(-data.worldViewDir, s.Normal);	\


#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
#define UNITY_GI(x, s, data) x = UnityGlobalIllumination (data, s.Occlusion, s.Normal);
#else
#define UNITY_GI(x, s, data) 									\
		UNITY_GLOSSY_ENV_FROM_SURFACE(g, s, data);				\
		x = UnityGlobalIllumination (data, s.Occlusion, s.Normal, g);
#endif


//-------------------------------------------------------------------------------------


// Surface shader output structure to be used with physically
// based shading model.

//-------------------------------------------------------------------------------------
// Metallic workflow

// RTP snow transl. index
float rtp_snow_TranslucencyDeferredLightIndex;

struct SurfaceOutputStandard {
	fixed3 Albedo;		// base (diffuse or specular) color
	fixed3 Normal;		// tangent space normal, if written
	half3 Emission;
	half Metallic;		// 0=non-metal, 1=metal
	half Smoothness;	// 0=rough, 1=smooth
	half Occlusion;		// occlusion (default 1)
	fixed Alpha;		// alpha for transparencies

	// RTP additions
		float3 additionalSpecColor;
		float atten; // from POM self-shadowing
		float distance; // for fog handling
		float snow_val;
		float Wetness;
		float4 lightDir; // used on planet shader
	///////
};


inline half4 LightingStandard(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
{
	s.Normal = normalize(s.Normal);

	half oneMinusReflectivity;
	half3 specColor;
	s.Albedo = DiffuseAndSpecularFromMetallicMod(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	// shader relies on pre-multiply alpha-blend (_SrcBlend = One, _DstBlend = OneMinusSrcAlpha)
	// this is necessary to handle transparency in physically correct way - only diffuse component gets affected by alpha
	half outputAlpha;
	s.Albedo = PreMultiplyAlpha(s.Albedo, s.Alpha, oneMinusReflectivity, /*out*/ outputAlpha);

	// RTP 
		#if !defined(LIGHTMAP_ON)// && (defined(UNITY_PASS_FORWARDBASE) || defined(RTP_SOFTSHADOWS_FORWARDADD))
			half3 directLightCol = gi.light.color.rgb;
			gi.light.color.rgb *= s.atten; // attenuate
		#endif
		specColor += s.additionalSpecColor;
	//////

	half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);

	// RTP 
		#if !defined(LIGHTMAP_ON)// && (defined(UNITY_PASS_FORWARDBASE) || defined(RTP_SOFTSHADOWS_FORWARDADD))
			gi.light.color.rgb = directLightCol; // bring back light state for GI
		#endif
	//////

	c.rgb += UNITY_BRDF_GI(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);
	c.a = outputAlpha;
	return c;
}

inline half4 LightingStandard_Deferred(SurfaceOutputStandard s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
{
	half oneMinusReflectivity;
	half3 specColor;
	s.Albedo = DiffuseAndSpecularFromMetallicMod(s.Albedo, s.Metallic, /*out*/ specColor, /*out*/ oneMinusReflectivity);

	// RTP
		// glitter
		s.Emission += s.additionalSpecColor * ShadeSH9(float4(s.Normal, 1))*s.Occlusion; // in deferred gbuffer for spec is LDR, we need to add it here directly to HDR light/emission gbuffer
		specColor += s.additionalSpecColor;

		// encoded values
		// HDR only (we store 0..2047 2^11 integer value in half precision significand)
		float encoded = 0;
		encoded = floor(saturate(s.snow_val)*15); // 4 bits - 0..15 translucency levels
		encoded *= 4; // shift left 2 bits to make room for translucency index
		encoded += rtp_snow_TranslucencyDeferredLightIndex; // + 0..3 light color index

		encoded *= 4; // shift left 2 bits to make room for self shadowing value
		encoded += floor((1 - s.atten) * 3); // 0..3 integer self-shadowing range

		encoded *= 8; // shift left 3 bits to make room for wetness value
		encoded += floor(s.Wetness * 7); // 0..7 integer wetness range
		//encoded = 15;
		//encoded *= 4 * 4 * 8;
		encoded = -encoded; // negative number means we encoded values, positive value is supposed to be 1 only
	//////

	half4 c = UNITY_BRDF_PBS(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, gi.light, gi.indirect);
	c.rgb += UNITY_BRDF_GI(s.Albedo, specColor, oneMinusReflectivity, s.Smoothness, s.Normal, viewDir, s.Occlusion, gi);

	outDiffuseOcclusion = half4(s.Albedo, s.Occlusion);
	outSpecSmoothness = half4(specColor, s.Smoothness);
	outNormal = half4(s.Normal * 0.5 + 0.5, 1);
	half4 emission = half4(s.Emission + c.rgb, encoded); // RTP encoded value on emission A channel
	return emission;
}

inline void LightingStandard_GI(
	SurfaceOutputStandard s,
	UnityGIInput data,
	inout UnityGI gi)
{
	UNITY_GI(gi, s, data);
}

#endif // UNITY_PBS_LIGHTING_INCLUDED