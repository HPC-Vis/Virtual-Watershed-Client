//
// Relief Terrain Shader
// Tomasz Stobierski 2014
//
Shader "Relief Pack/ReliefTerrain2GeometryBlendBase" {
Properties {
	_Control ("Control1 (RGBA)", 2D) = "red" {} 

	_Control1 ("Control1 (RGBA)", 2D) = "red" {}  
	_Control2 ("Control2 (RGBA)", 2D) = "red" {}
	_Control3 ("Control3 (RGBA)", 2D) = "red" {} 
	
	_SplatAtlasA  ("atlas", 2D) = "black" {}
	_SplatAtlasB  ("atlas", 2D) = "black" {}
	_SplatAtlasC  ("atlas", 2D) = "black" {}

	_SplatA0 ("Detailmap 0 (RGB+A spec)", 2D) = "black" {}
	_SplatA1 ("Detailmap 1 (RGB+A spec)", 2D) = "black" {}
	_SplatA2 ("Detailmap 2 (RGB+A spec)", 2D) = "black" {}
	_SplatA3 ("Detailmap 3 (RGB+A spec)", 2D) = "black" {}
	_SplatB0 ("Detailmap 4 (RGB+A spec)", 2D) = "black" {}
	_SplatB1 ("Detailmap 5 (RGB+A spec)", 2D) = "black" {}
	_SplatB2 ("Detailmap 6 (RGB+A spec)", 2D) = "black" {}
	_SplatB3 ("Detailmap 7 (RGB+A spec)", 2D) = "black" {}
	_SplatC0 ("Detailmap 8 (RGB+A spec)", 2D) = "black" {}
	_SplatC1 ("Detailmap 9 (RGB+A spec)", 2D) = "black" {}
	_SplatC2 ("Detailmap 10 (RGB+A spec)", 2D) = "black" {}
	_SplatC3 ("Detailmap 11 (RGB+A spec)", 2D) = "black" {}
	_BumpMap01 ("Bumpmap combined 0+1 (RG+BA)", 2D) = "grey" {}
	_BumpMap23 ("Bumpmap combined 2+3 (RG+BA)", 2D) = "grey" {}
	_BumpMap45 ("Bumpmap combined 4+5 (RG+BA)", 2D) = "grey" {}
	_BumpMap67 ("Bumpmap combined 6+7 (RG+BA)", 2D) = "grey" {}
	_BumpMap89 ("Bumpmap combined 8+9 (RG+BA)", 2D) = "grey" {}
	_BumpMapAB ("Bumpmap combined 10+11 (RG+BA)", 2D) = "grey" {}
	_TERRAIN_HeightMap ("Heightmap combined (RGBA - layers 0-3)", 2D) = "white" {}	
	_TERRAIN_HeightMap2 ("Heightmap combined (RGBA - layers 4-7)", 2D) = "white" {}	
	_TERRAIN_HeightMap3 ("Heightmap combined (RGBA - layers 8-11)", 2D) = "white" {}	
	
	_ColorMapGlobal ("Global colormap (RGBA)", 2D) = "white" {}
	_NormalMapGlobal ("Global normalmap (RGBA)", 2D) = "white" {}
	_TreesMapGlobal ("Global pixel treesmap (RGBA)", 2D) = "white" {}
	_AmbientEmissiveMapGlobal ("Global  ambient emissive map (RGBA)", 2D) = "white" {}
	_BumpMapGlobal ("Perlin normal combined w. water & reflection map (RG+B+A)", 2D) = "black" {}

	_VerticalTexture ("Vertical texture", 2D) = "grey" {}
	TERRAIN_RippleMap ("Water ripplemap", 2D) = "black" {}
	_SSColorCombinedA ("Color combined fo supersimple mode", 2D) = "black" {}
	
	terrainTileSize ("terrainTileSize", Vector) = (600,200,600,1)
	_Phong ("_Phong", Float) = 0.5
		
	_CubemapDiff ("IBL Diff cube", CUBE) = "black" {}
	_CubemapSpec ("IBL Spec cube", CUBE) = "black" {}
	_DiffCubeIBL ("IBL Diff cube", CUBE) = "black" {}
	_SpecCubeIBL ("IBL Spec cube", CUBE) = "black" {}
	
	_BumpMapGlobalScale ("", Float) = 1
	_GlobalColorMapBlendValues ("", Vector) = (1,1,1,1)
	_GlobalColorMapSaturation ("", Float) = 1
	_GlobalColorMapSaturationFar ("", Float) = 1
	_GlobalColorMapBrightness ("", Float) = 1
	_GlobalColorMapBrightnessFar ("", Float) = 1
	_GlobalColorMapNearMIP ("", Float) = 1
	_GlobalColorMapDistortByPerlin ("", Float) = 1
	EmissionRefractFiltering ("", Float) = 1
	EmissionRefractAnimSpeed ("", Float) = 1
	RTP_DeferredAddPassSpec ("", Float) = 1
	
	_TERRAIN_ReliefTransform ("", Vector) = (1,1,1,1)
	_TERRAIN_ReliefTransformTriplanarZ ("", Float) = 1
	_TERRAIN_DIST_STEPS ("", Float) = 1
	_TERRAIN_WAVELENGTH ("", Float) = 1
	
	_blend_multiplier ("", Float) = 1
	
	_TERRAIN_ExtrudeHeight ("", Float) = 1
	_TERRAIN_LightmapShading ("", Float) = 1
	
	_TERRAIN_SHADOW_STEPS ("", Float) = 1
	_TERRAIN_WAVELENGTH_SHADOWS ("", Float) = 1
	_TERRAIN_SHADOW_SMOOTH_STEPS ("", Float) = 1
	_TERRAIN_SelfShadowStrength ("", Float) = 1
	_TERRAIN_ShadowSmoothing ("", Float) = 1
	
	rtp_mipoffset_color ("", Float) = 1
	rtp_mipoffset_bump ("", Float) = 1
	rtp_mipoffset_height ("", Float) = 1
	rtp_mipoffset_superdetail ("", Float) = 1
	rtp_mipoffset_flow ("", Float) = 1
	rtp_mipoffset_ripple ("", Float) = 1
	rtp_mipoffset_globalnorm ("", Float) = 1
	rtp_mipoffset_caustics ("", Float) = 1
	
	// caustics
	TERRAIN_CausticsAnimSpeed ("", Float) = 1
	TERRAIN_CausticsColor ("", Vector) = (1,1,1,1)
	TERRAIN_CausticsTilingScale ("", Float) = 1
	
	///////////////////////////////////////////
	//
	// reflection
	//
	TERRAIN_ReflColorA ("", Vector) = (1,1,1,1)
	TERRAIN_ReflColorB ("", Vector) = (1,1,1,1)
	TERRAIN_ReflectionRotSpeed ("", Float) = 1
	TERRAIN_ReflGlossAttenuation ("", Float) = 1
	TERRAIN_ReflColorC ("", Vector) = (1,1,1,1)
	TERRAIN_ReflColorCenter ("", Float) = 1 
	
	//
	// water/wet
	//
	// global
	
	TERRAIN_RippleScale ("", Float) = 1
	TERRAIN_FlowScale ("", Float) = 1
	TERRAIN_FlowSpeed ("", Float) = 1
	TERRAIN_FlowCycleScale ("", Float) = 1
	TERRAIN_FlowMipOffset ("", Float) = 1
	
	TERRAIN_DropletsSpeed ("", Float) = 1
	TERRAIN_WetDarkening ("", Float) = 1
	TERRAIN_WetDropletsStrength ("", Float) = 1
	TERRAIN_WetHeight_Treshold ("", Float) = 1
	TERRAIN_WetHeight_Transition ("", Float) = 1
	
	TERRAIN_mipoffset_flowSpeed ("", Float) = 1
	
	_TERRAIN_distance_start ("", Float) = 1
	_TERRAIN_distance_transition ("", Float) = 1
	
	_TERRAIN_distance_start_bumpglobal ("", Float) = 1
	_TERRAIN_distance_transition_bumpglobal ("", Float) = 1
	rtp_perlin_start_val ("", Float) = 1
	_FarNormalDamp ("", Float) = 1
	
	_RTP_MIP_BIAS ("", Float) = 1
	
	_SuperDetailTiling ("", Float) = 1
	
	_VerticalTextureTiling ("", Float) = 1
	_VerticalTextureGlobalBumpInfluence ("", Float) = 1
	
	RTP_AOamp ("", Float) = 1
	
	RTP_AOsharpness ("", Float) = 1
		
		
	// per layer 0-3
	_MixScale0123  ("", Vector) = (1,1,1,1)
	_MixBlend0123 ("", Vector) = (1,1,1,1)
	_GlobalColorPerLayer0123 ("", Vector) = (1,1,1,1)
	_LayerBrightness0123  ("", Vector) = (1,1,1,1)
	_LayerSaturation0123 ("", Vector) = (1,1,1,1)
	_LayerBrightness2Spec0123 ("", Vector) = (1,1,1,1)
	_LayerAlbedo2SpecColor0123 ("", Vector) = (1,1,1,1)
	_MixSaturation0123 ("", Vector) = (1,1,1,1)
	_MixBrightness0123 ("", Vector) = (1,1,1,1)
	_MixReplace0123 ("", Vector) = (1,1,1,1)
	_LayerEmission0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractStrength0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractHBedge0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorR0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorG0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorB0123 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorA0123 ("", Vector) = (1,1,1,1)
	
	_GlobalColorBottom0123 ("", Vector) = (1,1,1,1)
	_GlobalColorTop0123 ("", Vector) = (1,1,1,1)
	_GlobalColorColormapLoSat0123 ("", Vector) = (1,1,1,1)
	_GlobalColorColormapHiSat0123 ("", Vector) = (1,1,1,1)
	_GlobalColorLayerLoSat0123 ("", Vector) = (1,1,1,1)
	_GlobalColorLayerHiSat0123 ("", Vector) = (1,1,1,1)
	_GlobalColorLoBlend0123 ("", Vector) = (1,1,1,1)
	_GlobalColorHiBlend0123 ("", Vector) = (1,1,1,1)
	
	_Spec0123 ("", Vector) = (1,1,1,1)
	_FarSpecCorrection0123 ("", Vector) = (1,1,1,1)
	_MIPmult0123 ("", Vector) = (1,1,1,1)
	
	// water per layer
	TERRAIN_LayerWetStrength0123 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterLevel0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterLevelSlopeDamp0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEdge0123 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterOpacity0123 ("", Vector) = (1,1,1,1)
	TERRAIN_Refraction0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WetRefraction0123 ("", Vector) = (1,1,1,1)
	TERRAIN_Flow0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WetSpecularity0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WetFlow0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WetGloss0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterSpecularity0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGloss0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGlossDamper0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEmission0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorR0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorG0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorB0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorA0123 ("", Vector) = (1,1,1,1)
	
	_BumpMapGlobalStrength0123 ("", Vector) = (1,1,1,1)
	
	PER_LAYER_HEIGHT_MODIFIER0123 ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultA0123 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultB0123 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthNormal0123 ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultASelfMaskNear0123 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultASelfMaskFar0123 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskNear0123 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskFar0123 ("", Vector) = (1,1,1,1)
	
	_VerticalTexture0123 ("", Vector) = (1,1,1,1)
	
	// PBL / IBL
	RTP_gloss2mask0123 ("", Vector) = (1,1,1,1)
	RTP_gloss_mult0123 ("", Vector) = (1,1,1,1)
	RTP_gloss_shaping0123 ("", Vector) = (1,1,1,1)
	RTP_Fresnel0123 ("", Vector) = (1,1,1,1)
	RTP_FresnelAtten0123 ("", Vector) = (1,1,1,1)
	RTP_DiffFresnel0123 ("", Vector) = (1,1,1,1)
	// IBL
	RTP_IBL_bump_smoothness0123 ("", Vector) = (1,1,1,1)
	RTP_IBL_DiffuseStrength0123 ("", Vector) = (1,1,1,1)
	RTP_IBL_SpecStrength0123 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterIBL_SpecWetStrength0123 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterIBL_SpecWaterStrength0123 ("", Vector) = (1,1,1,1)
	
	RTP_AO_0123 ("", Vector) = (1,1,1,1)
	
	// per layer  4-7
	_MixScale4567  ("", Vector) = (1,1,1,1)
	_MixBlend4567 ("", Vector) = (1,1,1,1)
	_GlobalColorPerLayer4567 ("", Vector) = (1,1,1,1)
	_LayerBrightness4567  ("", Vector) = (1,1,1,1)
	_LayerSaturation4567 ("", Vector) = (1,1,1,1)
	_LayerBrightness2Spec4567 ("", Vector) = (1,1,1,1)
	_LayerAlbedo2SpecColor4567 ("", Vector) = (1,1,1,1)
	_MixSaturation4567 ("", Vector) = (1,1,1,1)
	_MixBrightness4567 ("", Vector) = (1,1,1,1)
	_MixReplace4567 ("", Vector) = (1,1,1,1)
	_LayerEmission4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractStrength4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractHBedge4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorR4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorG4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorB4567 ("", Vector) = (1,1,1,1)
	_LayerEmissionColorA4567 ("", Vector) = (1,1,1,1)
	
	_GlobalColorBottom4567 ("", Vector) = (1,1,1,1)
	_GlobalColorTop4567 ("", Vector) = (1,1,1,1)
	_GlobalColorColormapLoSat4567 ("", Vector) = (1,1,1,1)
	_GlobalColorColormapHiSat4567 ("", Vector) = (1,1,1,1)
	_GlobalColorLayerLoSat4567 ("", Vector) = (1,1,1,1)
	_GlobalColorLayerHiSat4567 ("", Vector) = (1,1,1,1)
	_GlobalColorLoBlend4567 ("", Vector) = (1,1,1,1)
	_GlobalColorHiBlend4567 ("", Vector) = (1,1,1,1)
	
	_Spec4567 ("", Vector) = (1,1,1,1)
	_FarSpecCorrection4567 ("", Vector) = (1,1,1,1)
	_MIPmult4567 ("", Vector) = (1,1,1,1)
	
	// water per layer
	TERRAIN_LayerWetStrength4567 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterLevel4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterLevelSlopeDamp4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEdge4567 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterOpacity4567 ("", Vector) = (1,1,1,1)
	TERRAIN_Refraction4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WetRefraction4567 ("", Vector) = (1,1,1,1)
	TERRAIN_Flow4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WetSpecularity4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WetFlow4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WetGloss4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterSpecularity4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGloss4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGlossDamper4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEmission4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorR4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorG4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorB4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorA4567 ("", Vector) = (1,1,1,1)
	
	_BumpMapGlobalStrength4567 ("", Vector) = (1,1,1,1)
	
	PER_LAYER_HEIGHT_MODIFIER4567 ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultA4567 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultB4567 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthNormal4567 ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultASelfMaskNear4567 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultASelfMaskFar4567 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskNear4567 ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskFar4567 ("", Vector) = (1,1,1,1)
	
	_VerticalTexture4567 ("", Vector) = (1,1,1,1)
	
	// PBL / IBL
	RTP_gloss2mask4567 ("", Vector) = (1,1,1,1)
	RTP_gloss_mult4567 ("", Vector) = (1,1,1,1)
	RTP_gloss_shaping4567 ("", Vector) = (1,1,1,1)
	RTP_Fresnel4567 ("", Vector) = (1,1,1,1)
	RTP_FresnelAtten4567 ("", Vector) = (1,1,1,1)
	RTP_DiffFresnel4567 ("", Vector) = (1,1,1,1)
	// IBL
	RTP_IBL_bump_smoothness4567 ("", Vector) = (1,1,1,1)
	RTP_IBL_DiffuseStrength4567 ("", Vector) = (1,1,1,1)
	RTP_IBL_SpecStrength4567 ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterIBL_SpecWetStrength4567 ("", Vector) = (1,1,1,1)
	TERRAIN_WaterIBL_SpecWaterStrength4567 ("", Vector) = (1,1,1,1)
	
	RTP_AO_4567 ("", Vector) = (1,1,1,1)
	
	// per layer 0-3
	_MixScale89AB ("", Vector) = (1,1,1,1)
	_MixBlend89AB ("", Vector) = (1,1,1,1)
	_GlobalColorPerLayer89AB ("", Vector) = (1,1,1,1)
	_LayerBrightness89AB  ("", Vector) = (1,1,1,1)
	_LayerSaturation89AB ("", Vector) = (1,1,1,1)
	_LayerBrightness2Spec89AB ("", Vector) = (1,1,1,1)
	_LayerAlbedo2SpecColor89AB ("", Vector) = (1,1,1,1)
	_MixSaturation89AB ("", Vector) = (1,1,1,1)
	_MixBrightness89AB ("", Vector) = (1,1,1,1)
	_MixReplace89AB ("", Vector) = (1,1,1,1)
	_LayerEmission89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractStrength89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionRefractHBedge89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionColorR89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionColorG89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionColorB89AB ("", Vector) = (1,1,1,1)
	_LayerEmissionColorA89AB ("", Vector) = (1,1,1,1)
	
	_GlobalColorBottom89AB ("", Vector) = (1,1,1,1)
	_GlobalColorTop89AB ("", Vector) = (1,1,1,1)
	_GlobalColorColormapLoSat89AB ("", Vector) = (1,1,1,1)
	_GlobalColorColormapHiSat89AB ("", Vector) = (1,1,1,1)
	_GlobalColorLayerLoSat89AB ("", Vector) = (1,1,1,1)
	_GlobalColorLayerHiSat89AB ("", Vector) = (1,1,1,1)
	_GlobalColorLoBlend89AB ("", Vector) = (1,1,1,1)
	_GlobalColorHiBlend89AB ("", Vector) = (1,1,1,1)
	
	_Spec89AB ("", Vector) = (1,1,1,1)
	_FarSpecCorrection89AB ("", Vector) = (1,1,1,1)
	_MIPmult89AB ("", Vector) = (1,1,1,1)
	
	// water per layer
	TERRAIN_LayerWetStrength89AB ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterLevel89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterLevelSlopeDamp89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEdge89AB ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterOpacity89AB ("", Vector) = (1,1,1,1)
	TERRAIN_Refraction89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WetRefraction89AB ("", Vector) = (1,1,1,1)
	TERRAIN_Flow89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WetSpecularity89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WetFlow89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WetGloss89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterSpecularity89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGloss89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterGlossDamper89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterEmission89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorR89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorG89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorB89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterColorA89AB ("", Vector) = (1,1,1,1)
	
	_BumpMapGlobalStrength89AB ("", Vector) = (1,1,1,1)
	
	PER_LAYER_HEIGHT_MODIFIER89AB ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultA89AB ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultB89AB ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthNormal89AB ("", Vector) = (1,1,1,1)
	
	_SuperDetailStrengthMultASelfMaskNear89AB ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultASelfMaskFar89AB ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskNear89AB ("", Vector) = (1,1,1,1)
	_SuperDetailStrengthMultBSelfMaskFar89AB ("", Vector) = (1,1,1,1)
	
	_VerticalTexture89AB ("", Vector) = (1,1,1,1)
	
	// PBL / IBL
	RTP_gloss2mask89AB ("", Vector) = (1,1,1,1)
	RTP_gloss_mult89AB ("", Vector) = (1,1,1,1)
	RTP_gloss_shaping89AB ("", Vector) = (1,1,1,1)
	RTP_Fresnel89AB ("", Vector) = (1,1,1,1)
	RTP_FresnelAtten89AB ("", Vector) = (1,1,1,1)
	RTP_DiffFresnel89AB ("", Vector) = (1,1,1,1)
	// IBL
	RTP_IBL_bump_smoothness89AB ("", Vector) = (1,1,1,1)
	RTP_IBL_DiffuseStrength89AB ("", Vector) = (1,1,1,1)
	RTP_IBL_SpecStrength89AB ("", Vector) = (1,1,1,1)
	
	TERRAIN_WaterIBL_SpecWetStrength89AB ("", Vector) = (1,1,1,1)
	TERRAIN_WaterIBL_SpecWaterStrength89AB ("", Vector) = (1,1,1,1)
	
	RTP_AO_89AB ("", Vector) = (1,1,1,1)
	
	_Phong ("", Float) = 0	
	_TessSubdivisions ("", Float) = 1
	_TessSubdivisionsFar ("", Float) = 1
	_TessYOffset ("", Float) = 0
	
}

/* INIT
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// POM / PM / SIMPLE shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"Queue" = "Geometry+3"
		"RenderType" = "Opaque"
	}
	LOD 700
	Fog { Mode Off }
	ZTest LEqual
	CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog
	// U5 fog handling
	#pragma multi_compile_fog		
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_POM_SHADING_HI
	
	#include "UnityCG.cginc"
	
	// for geom blend (early exit from sur function)
	#define COLOR_EARLY_EXIT
	// tangents approximation
	//#define APPROX_TANGENTS
	
	#include "./../RTP_Base.cginc"

	ENDCG
	
///astar AddBlend
Fog { Mode Off }
ZWrite Off
ZTest LEqual	
CGPROGRAM
	#pragma surface surf CustomBlinnPhong vertex:vert finalcolor:customFog decal:blend
	// U5 fog handling
	#pragma multi_compile_fog	
	#pragma target 3.0
	#pragma glsl
	#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
	#pragma multi_compile RTP_PM_SHADING RTP_SIMPLE_SHADING
	//#define RTP_PM_SHADING
	
	#include "UnityCG.cginc"

	// for geom blend (early exit from sur function)
	#define COLOR_EARLY_EXIT
	// tangents approximation
	//#define APPROX_TANGENTS
	
	#define BLENDBASE
	
	#include "./../RTP_AddBase.cginc"

ENDCG  				
//astar/ // AddBlend
	
}
// EOF POM / PM / SIMPLE shading
*/ // INIT

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
//
// CLASSIC shading
//
//
//
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
SubShader {
	Tags {
		"Queue" = "Geometry+3"
		"RenderType" = "Opaque"
	}
	LOD 100
	ZTest LEqual
CGPROGRAM
	#pragma surface surf Lambert vertex:vert
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		//#define COLOR_MAP_BLEND_MULTIPLY
		
		//#define RTP_SNOW
	#endif
/////////////////////////////////////////////////////////////////////
float4 _Control_ST;	
sampler2D _Control, _Control1;
sampler2D _SplatA0,_SplatA1,_SplatA2,_SplatA3;
float4 _TERRAIN_ReliefTransform;
float RTP_DeferredAddPassSpec;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float _snow_strength;
float _global_color_brightness_to_snow;
float _snow_slope_factor;
float _snow_edge_definition;
float4 _snow_strength_per_layer0123;
float4 _snow_strength_per_layer4567;
float _snow_height_treshold;
float _snow_height_transition;
fixed3 _snow_color;
float _snow_gloss;
float _snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float4 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=mul(_Object2World, v.vertex).xz / _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	o._uv_Relief.zw=TRANSFORM_TEX(v.texcoord, _Control);
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
	float4 splat_control = tex2D(_Control1, IN._uv_Relief.zw);
	o.Specular=RTP_DeferredAddPassSpec;
		
	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float3 global_color_value=tex2D(_ColorMapGlobal, IN._uv_Relief.zw).rgb;
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	

	#ifdef RTP_SNOW
		float snow_val=_snow_strength*2;
		float snow_height_fct=saturate((_snow_height_treshold - IN.snowDir.w)/_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += _snow_strength*dot(1-global_color_value.rgb, _global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= _snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		
	 	fixed3 col;
		col = splat_control.r * lerp(tex2D(_SplatA0, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.x );
		col += splat_control.g * lerp(tex2D(_SplatA1, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.y );
		col += splat_control.b * lerp(tex2D(_SplatA2, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.z );
		col += splat_control.a * lerp(tex2D(_SplatA3, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer0123.w );
		
		global_color_value.rgb=lerp(global_color_value.rgb, _snow_color, snow_val);
	#else		
	 	fixed3 col;
		col = splat_control.r * tex2D(_SplatA0, IN._uv_Relief.xy).rgb;
		col += splat_control.g * tex2D(_SplatA1, IN._uv_Relief.xy).rgb;
		col += splat_control.b * tex2D(_SplatA2, IN._uv_Relief.xy).rgb;
		col += splat_control.a * tex2D(_SplatA3, IN._uv_Relief.xy).rgb;
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col=lerp(col, col*global_color_value.rgb*2, _GlobalColorMapBlendValues.y);
		#else
			col=lerp(col, global_color_value.rgb, _GlobalColorMapBlendValues.y);
		#endif		
	#endif
		
	o.Albedo = col;
}
ENDCG

///* AddPass
ZTest LEqual
CGPROGRAM
	#pragma surface surf Lambert vertex:vert decal:add
	#include "UnityCG.cginc"
	
	#pragma only_renderers d3d9 opengl gles gles3 xbox360 metal ps3 d3d11 xboxone ps4
	
/////////////////////////////////////////////////////////////////////
// RTP specific
//
	#define ADDITIONAL_FEATURES_IN_FALLBACKS

	#ifdef ADDITIONAL_FEATURES_IN_FALLBACKS	
		// comment if you don't need global color map
		#define COLOR_MAP
		// if not defined global color map will be blended (lerp)
		//#define COLOR_MAP_BLEND_MULTIPLY
		
		//#define RTP_SNOW
	#endif
/////////////////////////////////////////////////////////////////////
float4 _Control_ST;	
sampler2D _Control, _Control2;
sampler2D _SplatB0,_SplatB1,_SplatB2,_SplatB3;
float4 _TERRAIN_ReliefTransform;

/////////////////////////////////////////////////////////////////////
// RTP specific
//
#ifdef COLOR_MAP
float3 _GlobalColorMapBlendValues;
float _GlobalColorMapSaturation;
sampler2D _ColorMapGlobal;
#endif
#ifdef RTP_SNOW
float _snow_strength;
float _global_color_brightness_to_snow;
float _snow_slope_factor;
float _snow_edge_definition;
float4 _snow_strength_per_layer0123;
float4 _snow_strength_per_layer4567;
float _snow_height_treshold;
float _snow_height_transition;
fixed3 _snow_color;
float _snow_gloss;
float _snow_specular;
#endif
////////////////////////////////////////////////////////////////////

struct Input {
	float4 _uv_Relief;
	float4 snowDir;
};

void vert (inout appdata_full v, out Input o) {
    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
		UNITY_INITIALIZE_OUTPUT(Input, o);
	#endif
	o._uv_Relief.xy=mul(_Object2World, v.vertex).xz / _TERRAIN_ReliefTransform.xy + _TERRAIN_ReliefTransform.zw;
	o._uv_Relief.zw=TRANSFORM_TEX(v.texcoord, _Control);
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
	float4 splat_control = tex2D(_Control2, IN._uv_Relief.zw);

	#if defined(COLOR_MAP) || defined(RTP_SNOW)
		float3 global_color_value=tex2D(_ColorMapGlobal, IN._uv_Relief.zw).rgb;
		global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, _GlobalColorMapSaturation);
	#endif	

	#ifdef RTP_SNOW
		float snow_val=_snow_strength*2;
		float snow_height_fct=saturate((_snow_height_treshold - IN.snowDir.w)/_snow_height_transition)*4;
		snow_val += snow_height_fct<0 ? 0 : -snow_height_fct;
		
		snow_val += _snow_strength*dot(1-global_color_value.rgb, _global_color_brightness_to_snow);
		float3 norm_for_snow=float3(0,1,0);
		snow_val -= _snow_slope_factor*(1-dot(norm_for_snow, IN.snowDir.xyz));

		snow_val=saturate(snow_val);
		snow_val*=snow_val;
		snow_val*=snow_val;
		
	 	fixed3 col;
		col = splat_control.r * lerp(tex2D(_SplatB0, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.x );
		col += splat_control.g * lerp(tex2D(_SplatB1, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.y );
		col += splat_control.b * lerp(tex2D(_SplatB2, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.z );
		col += splat_control.a * lerp(tex2D(_SplatB3, IN._uv_Relief.xy).rgb, _snow_color, snow_val*_snow_strength_per_layer4567.w );
		
		global_color_value.rgb=lerp(global_color_value.rgb, _snow_color, snow_val);
	#else		
	 	fixed3 col;
		col = splat_control.r * tex2D(_SplatB0, IN._uv_Relief.xy).rgb;
		col += splat_control.g * tex2D(_SplatB1, IN._uv_Relief.xy).rgb;
		col += splat_control.b * tex2D(_SplatB2, IN._uv_Relief.xy).rgb;
		col += splat_control.a * tex2D(_SplatB3, IN._uv_Relief.xy).rgb;
	#endif
	
	#ifdef COLOR_MAP
		#ifdef COLOR_MAP_BLEND_MULTIPLY
			col=lerp(col, col*global_color_value.rgb*2, _GlobalColorMapBlendValues.y);
		#else
			col=lerp(col, global_color_value.rgb, _GlobalColorMapBlendValues.y);
		#endif		
	#endif	
	
	o.Albedo = col;
}
ENDCG  
//*/ // AddPass

}
// EOF CLASSIC shading

// Fallback to Diffuse
Fallback "Diffuse"
}

