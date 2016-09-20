// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

	float4 _MainTex_ST;
	void vert (inout appdata_full v, out Input o) {
	    #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X) || defined(UNITY_PI)
			UNITY_INITIALIZE_OUTPUT(Input, o);
		#endif

		o.texCoords_FlatRef.xy=TRANSFORM_TEX(v.texcoord, _MainTex);
	
		float3 Wpos=mul(unity_ObjectToWorld, v.vertex).xyz;
		
		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS) || defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
			float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal.xyz );				
		#endif

		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
			o.texCoords_FlatRef.zw = normalEncode(( mul (rotation, mul(unity_WorldToObject, float4(0,1,0,0)).xyz) ).xyz);
		#endif
	}

	void surf (Input IN, inout RTPSurfaceOutput o) {
		o.Normal=float3(0,0,1); o.Albedo=0;	o.Emission=0; o.Specular=0.01; o.Alpha=0;
		o.RTP.xy=float2(0,1); o.SpecColor=0;
		half o_Gloss=0;	
		
		float _distance=length(_WorldSpaceCameraPos.xyz-IN.worldPos);
		float _uv_Relief_w=saturate((_distance - _TERRAIN_distance_start_bumpglobal) / _TERRAIN_distance_transition_bumpglobal);
		#if defined(COLOR_MAP) || defined(GLOBAL_PERLIN) || defined(RTP_UV_BLEND)
			float _uv_Relief_z=saturate((_distance - _TERRAIN_distance_start) / _TERRAIN_distance_transition);
			float _uv_Relief_wz_no_overlap=_uv_Relief_w*_uv_Relief_z;
			_uv_Relief_z=1-_uv_Relief_z;
		#else
			float _uv_Relief_z=1-_uv_Relief_w;
		#endif

		#if defined(GLOBAL_PERLIN)		
			float4 global_bump_val=tex2D(_BumpMapGlobal, IN.texCoords_FlatRef.xy*_BumpMapGlobalScale);
			#if !defined(RTP_SIMPLE_SHADING)
				global_bump_val.rg=global_bump_val.rg*0.6 + tex2D(_BumpMapGlobal, IN.texCoords_FlatRef.xy*_BumpMapGlobalScale*8).rg*0.4;
			#endif
		#endif

		float2 globalUV=(IN.worldPos.xz-_TERRAIN_PosSize.xy)/_TERRAIN_PosSize.zw;
		#ifdef COLOR_MAP
			float global_color_blend=lerp( lerp(_GlobalColorMapBlendValues.y, _GlobalColorMapBlendValues.x, _uv_Relief_z*_uv_Relief_z), _GlobalColorMapBlendValues.z, _uv_Relief_w);
			#if defined(RTP_SIMPLE_SHADING) || !defined(GLOBAL_PERLIN)
				float4 global_color_value=tex2D(_ColorMapGlobal, globalUV);
				global_color_value=lerp(tex2Dlod(_ColorMapGlobal, float4(globalUV, _GlobalColorMapNearMIP.xx)), global_color_value, _uv_Relief_w);
			#else
				float4 global_color_value=tex2D(_ColorMapGlobal, globalUV+(global_bump_val.rg-float2(0.5f, 0.5f))*_GlobalColorMapDistortByPerlin);
				global_color_value=lerp(tex2Dlod(_ColorMapGlobal, float4(globalUV+(global_bump_val.rg-float2(0.5f, 0.5f))*_GlobalColorMapDistortByPerlin, _GlobalColorMapNearMIP.xx)), global_color_value, _uv_Relief_w);
			#endif
			
			//float perlin2global_color=abs((global_bump_val.r-0.4)*5);
			//perlin2global_color*=perlin2global_color;
			//float GlobalColorMapSaturationByPerlin = saturate( lerp(_GlobalColorMapSaturation, _GlobalColorMapSaturationFar, _uv_Relief_w) -perlin2global_color*_GlobalColorMapSaturationByPerlin);
			float GlobalColorMapSaturationByPerlin = lerp(_GlobalColorMapSaturation, _GlobalColorMapSaturationFar, _uv_Relief_w);
			global_color_value.rgb=lerp(dot(global_color_value.rgb,0.35).xxx, global_color_value.rgb, GlobalColorMapSaturationByPerlin);
			global_color_value.rgb*=lerp(_GlobalColorMapBrightness, _GlobalColorMapBrightnessFar, _uv_Relief_w);
		#endif		
		
		#if defined(GLOBAL_PERLIN)
      		float perlinmask=tex2Dbias(_BumpMapGlobal, float4(IN.texCoords_FlatRef.xy/16, _uv_Relief_w.xx*2)).r;  		
      	#else
      		#if defined(RTP_WETNESS) && !defined(SIMPLE_WATER)
      			float perlinmask=tex2D(TERRAIN_FlowingMap, IN.texCoords_FlatRef.xy/8).a;
      		#else
      			float perlinmask=0;
      		#endif
      	#endif
   		float3 norm_far=float3(0,0,1);

		#if defined(TWO_LAYERS)
	      	float2 tH;
	      	tH.x=tex2D(_HeightMap, IN.texCoords_FlatRef.xy).a;
	    	tH.y=tex2D(_HeightMap2, IN.texCoords_FlatRef.xy).a;
	      	#if !defined(RTP_SIMPLE_SHADING)
	      	#if defined(GEOM_BLEND)
	      		float eh=max(0.001, _ExtrudeHeight*(1-IN.color.a));
	      	#else
	      		float eh=_ExtrudeHeight;
	      	#endif		      	
	      	float2 uv=IN.texCoords_FlatRef.xy + ParallaxOffset(tH.x, eh, IN.viewDir.xyz);
	      	float2 uv2=IN.texCoords_FlatRef.xy + ParallaxOffset(tH.y, eh, IN.viewDir.xyz);
	      	#endif
	      	float2 control=float2(IN.color.r, 1-IN.color.r);
	      	control*=(tH+0.01);
      		float2 control_orig=control;		
	      	control*=control;
	      	control*=control;
	      	control*=control;
	      	control/=dot(control, 1);
	      	#ifdef NOSPEC_BLEED
				float2 control_nobleed=saturate(control-float2(0.5,0.5))*2;
			#else
				float2 control_nobleed=control;
			#endif
	      	float actH=dot(control, tH);
	    #else
	      	float actH=tex2D(_HeightMap, IN.texCoords_FlatRef.xy).a;
	      	#if !defined(RTP_SIMPLE_SHADING)
	      	#if defined(GEOM_BLEND)
	      		float eh=max(0.001, _ExtrudeHeight*(1-IN.color.a));
	      	#else
	      		float eh=_ExtrudeHeight;
	      	#endif		      	
	      	float2 uv=IN.texCoords_FlatRef.xy + ParallaxOffset(actH, eh, IN.viewDir.xyz);
	      	#endif
		#endif
      		
      	float2 rayPos;
      	
		// simple fresnel rim (w/o bumpmapping)
		IN.viewDir=normalize(IN.viewDir);
		IN.viewDir.z=saturate(IN.viewDir.z); // czasem wystepuja problemy na krawedziach widocznosci (viewDir.z nie powinien byc jednak ujemny)
		float diffFresnel = exp2(SchlickFresnelApproxExp2Const*IN.viewDir.z); // ca. (1-x)^5
		
		#if defined(RTP_SNOW) || defined(RTP_WETNESS) || defined(RTP_CAUSTICS)
			float3 flat_dir = normalDecode(IN.texCoords_FlatRef.zw);
			#if defined(RTP_WETNESS)
				float wetSlope=1-dot(norm_far, flat_dir.xyz);
			#endif
		#endif
		
		#if defined(GLOBAL_PERLIN)
			norm_far.xy = global_bump_val.rg*3-1.5;
			norm_far.z = sqrt(1 - saturate(dot(norm_far.xy, norm_far.xy)));			
		#endif
		
		#ifdef RTP_CAUSTICS
		float damp_fct_caustics;
   		#if defined(RTP_WETNESS)
			float damp_fct_caustics_inv;
		#endif
		{
			float norm=saturate(1-flat_dir.z);
			norm*=norm;
			norm*=norm;  
			float CausticsWaterLevel=TERRAIN_CausticsWaterLevel+norm*TERRAIN_CausticsWaterLevelByAngle;
			damp_fct_caustics=saturate((IN.worldPos.y-CausticsWaterLevel+TERRAIN_CausticsWaterDeepFadeLength)/TERRAIN_CausticsWaterDeepFadeLength);
			float overwater=saturate(-(IN.worldPos.y-CausticsWaterLevel-TERRAIN_CausticsWaterShallowFadeLength)/TERRAIN_CausticsWaterShallowFadeLength);
			damp_fct_caustics*=overwater;
       		#if defined(RTP_WETNESS)
				damp_fct_caustics_inv=1-overwater;
			#endif
			damp_fct_caustics*=saturate(flat_dir.z+0.1)*0.9+0.1;
		}
		#endif			
		
		// snow initial step
		#ifdef RTP_SNOW
			float3 norm_for_snow=norm_far*0.3;
			norm_for_snow.z+=0.7;
			#if defined(VERTEX_COLOR_TO_SNOW_COVERAGE)
				rtp_snow_strength*=VERTEX_COLOR_TO_SNOW_COVERAGE;
			#endif	
			float snow_const = 0.5*rtp_snow_strength;
			snow_const-=perlinmask;
			float snow_height_fct=saturate((rtp_snow_height_treshold - IN.worldPos.y)/rtp_snow_height_transition)*4;
			snow_height_fct=snow_height_fct<0 ? 0 : snow_height_fct;
			snow_const -= snow_height_fct;
			
			float snow_val;
			#ifdef COLOR_MAP
				snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
			#else
				rtp_global_color_brightness_to_snow=0;
				snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
			#endif
			snow_val -= rtp_snow_slope_factor*( 1 - dot(norm_for_snow, flat_dir.xyz) );
	
			float snow_depth=snow_val-1;
			snow_depth=snow_depth<0 ? 0:snow_depth*6; 
			
			//float snow_depth_lerp=saturate(snow_depth-rtp_snow_deep_factor);
	
			fixed3 rtp_snow_color_tex=rtp_snow_color.rgb;
		#endif		
		
		#ifdef RTP_UV_BLEND
			float blendVal=(1.0-_uv_Relief_z*0.3);
			#if defined(TWO_LAYERS)
				blendVal *= dot( control, float2(_MixBlend0, _MixBlend1) );
			#else
				blendVal *= _MixBlend0;
			#endif
			#if defined(GLOBAL_PERLIN)
				blendVal*=saturate((global_bump_val.r*global_bump_val.g*2+0.3));
			#endif

			#if defined(TWO_LAYERS)
				float2 MixScaleRouted=float2(_MixScale0, _MixScale1);
			#else
				float MixScaleRouted=_MixScale0;
			#endif
		#endif		
		
		// layer emission - init step
		#ifdef RTP_EMISSION
			#if defined(TWO_LAYERS)
				float emission_valA=dot(control, float2(_LayerEmission0, _LayerEmission1) );
				half3 _LayerEmissionColor=control.x * _LayerEmissionColor0 + control.y * _LayerEmissionColor1;
				float layer_emission = emission_valA;
			#else
				half3 _LayerEmissionColor=_LayerEmissionColor0;
				float layer_emission = _LayerEmission0;
			#endif
		#endif		
		
		#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			// gloss vs. fresnel dependency
			#if defined(TWO_LAYERS)			
				float fresnelAtten=dot(control, float2(RTP_FresnelAtten0, RTP_FresnelAtten1) );
				o.RTP.x=dot(control, float2(RTP_Fresnel0, RTP_Fresnel1) );
			#else
				float fresnelAtten=RTP_FresnelAtten0;
				o.RTP.x=RTP_Fresnel0;
			#endif
		#endif

      	#if defined(RTP_SIMPLE_SHADING)
      		rayPos=IN.texCoords_FlatRef.xy;
      	#else
			#if defined(TWO_LAYERS)		      	
	      		rayPos=lerp(uv, uv2, control.y);
      		#else
    	  		rayPos=uv;
      		#endif
      	#endif
      	
	    #if defined(RTP_WETNESS) || defined(RTP_REFLECTION)
	        float p = 0;
	        float _WaterOpacity=0;
		#endif
		
		////////////////////////////////
		// water
		//
		#ifdef RTP_WETNESS
			#if defined(VERTEX_COLOR_TO_WATER_COVERAGE) || !defined(GLOBAL_PERLIN)
				float water_mask=IN.color.b;
			#else
				float mip_selector_tmp=saturate(_uv_Relief_w-1);// bug in compiler for forward pass, we have to specify mip level indirectly (can't be treated constant)
				float water_mask=tex2Dlod(_BumpMapGlobal, float4(globalUV*(1-2*_BumpMapGlobal_TexelSize.xx)+_BumpMapGlobal_TexelSize.xx, mip_selector_tmp.xx)).b;
			#endif
			#if defined(TWO_LAYERS)		      	
				float2 water_splat_control=control;
				float2 water_splat_control_nobleed=control_nobleed;
			#endif
			float TERRAIN_LayerWetStrength=saturate( 2*(1 - water_mask-perlinmask*(1-TERRAIN_GlobalWetness)) )*TERRAIN_GlobalWetness;
			float2 roff=0;
			float2 flowOffset	=0;

			wetSlope=saturate(wetSlope*TERRAIN_WaterLevelSlopeDamp);
			float _RippleDamp=saturate(TERRAIN_LayerWetStrength*2-1)*saturate(1-wetSlope*4)*_uv_Relief_z;
			TERRAIN_RainIntensity*=_RippleDamp;
			TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength*2);
			TERRAIN_WaterLevel=clamp(TERRAIN_WaterLevel + ((TERRAIN_LayerWetStrength - 1) - wetSlope)*2, 0, 2);
			#ifdef RTP_CAUSTICS
				TERRAIN_WaterLevel*=damp_fct_caustics_inv;
			#endif				
			TERRAIN_LayerWetStrength=saturate(TERRAIN_LayerWetStrength - (1-TERRAIN_LayerWetStrength)*actH*0.25);
			
			p = saturate((TERRAIN_WaterLevel - actH -(1-actH)*perlinmask*0.5)*TERRAIN_WaterEdge);
			p*=p;
	        _WaterOpacity=TERRAIN_WaterOpacity*p;
			#if defined(RTP_EMISSION)
				float wEmission = TERRAIN_WaterEmission*p;
				layer_emission = lerp( layer_emission, wEmission, _WaterOpacity);
				layer_emission = max( layer_emission, wEmission*(1-_WaterOpacity) );
			#endif					
			#if !defined(RTP_SIMPLE_SHADING) && !defined(SIMPLE_WATER)
				float2 flowUV=lerp(IN.texCoords_FlatRef.xy, rayPos.xy, 1-p*0.5)*TERRAIN_FlowScale;
				float _Tim=frac(_Time.x*TERRAIN_FlowCycleScale)*2;
				float ft=abs(frac(_Tim)*2 - 1);
				float2 flowSpeed=clamp((flat_dir.xy+0.01)*4,-1,1)/TERRAIN_FlowCycleScale;
				#ifdef FLOWMAP
					float4 vec=tex2D(_FlowMap, flowUV)*2-1;
					flowSpeed+=lerp(vec.xy, vec.zw, IN.color.r)*float2(-1,1)*TERRAIN_FlowSpeedMap;
				#endif
				flowUV*=TERRAIN_FlowScale;
				flowSpeed*=TERRAIN_FlowSpeed*TERRAIN_FlowScale;
				float rtp_mipoffset_add = (1-saturate(dot(flowSpeed, flowSpeed)*TERRAIN_mipoffset_flowSpeed))*TERRAIN_mipoffset_flowSpeed;
				rtp_mipoffset_add+=(1-TERRAIN_LayerWetStrength)*8;
				rtp_mipoffset_add+=TERRAIN_FlowMipOffset;
				#if defined(GLOBAL_PERLIN)
					flowOffset=tex2Dbias(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx)*flowSpeed, rtp_mipoffset_add.xx)).rg*2-1;
					flowOffset=lerp(flowOffset, tex2Dbias(_BumpMapGlobal, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.25, rtp_mipoffset_add.xx)).rg*2-1, ft);
				#else
					flowOffset=tex2Dbias(TERRAIN_FlowingMap, float4(flowUV+frac(_Tim.xx)*flowSpeed, rtp_mipoffset_add.xx)).ag*2-1;
					flowOffset=lerp(flowOffset, tex2Dbias(TERRAIN_FlowingMap, float4(flowUV+frac(_Tim.xx+0.5)*flowSpeed*1.25, rtp_mipoffset_add.xx)).ag*2-1, ft);
				#endif
				#ifdef RTP_SNOW
					flowOffset*=saturate(1-snow_val);
				#endif							
				flowOffset*=lerp(TERRAIN_WetFlow, TERRAIN_Flow, p)*_uv_Relief_z*TERRAIN_LayerWetStrength;
			#endif
			
			#if defined(RTP_WET_RIPPLE_TEXTURE) && !defined(RTP_SIMPLE_SHADING)
				float2 rippleUV = IN.texCoords_FlatRef.xy*TERRAIN_RippleScale + flowOffset*0.1*flowSpeed/TERRAIN_FlowScale;
			    float4 Ripple;
			  	{
			  	 	Ripple = tex2D(TERRAIN_RippleMap, rippleUV);
				    Ripple.xy = Ripple.xy * 2 - 1;
				
				    float DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
				    float TimeFrac = DropFrac - 1.0f + Ripple.z;
				    float DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
				    float FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
				  	roff = Ripple.xy * FinalFactor * 0.35f;
				  	
				  	rippleUV+=float2(0.25,0.25);
			  	 	Ripple = tex2D(TERRAIN_RippleMap, rippleUV);
				    Ripple.xy = Ripple.xy * 2 - 1;
				
				    DropFrac = frac(Ripple.w + _Time.x*TERRAIN_DropletsSpeed);
				    TimeFrac = DropFrac - 1.0f + Ripple.z;
				    DropFactor = saturate(0.2f + TERRAIN_RainIntensity * 0.8f - DropFrac);
				    FinalFactor = DropFactor * Ripple.z * sin( clamp(TimeFrac * 9.0f, 0.0f, 3.0f) * 3.1415);
				  	roff += Ripple.xy * FinalFactor * 0.35f;
			  	}
			  	roff*=4*_RippleDamp*lerp(TERRAIN_WetDropletsStrength, 1, p);
			  	#ifdef RTP_SNOW
			  		roff*=saturate(1-snow_val);
			  	#endif
			  	roff+=flowOffset;
			#else
				roff = flowOffset;
			#endif
			
			#if !defined(RTP_SIMPLE_SHADING)
				flowOffset=TERRAIN_Refraction*roff*max(p, TERRAIN_WetRefraction);
				#if !defined(RTP_TRIPLANAR)
					rayPos.xy+=flowOffset;
				#endif
			#endif
		#endif
		// water
		///////////////////////////////////////////	      	
	    
	    //
	    // diffuse color
	    //
		#if defined(TWO_LAYERS)		      	
	      	fixed4 col = control.x*tex2D(_MainTex, rayPos.xy) + control.y*tex2D(_MainTex2, rayPos.xy);
	    #else
	      	fixed4 col = tex2D(_MainTex, rayPos.xy);
	    #endif

	    // UV blend
		#if defined(RTP_UV_BLEND)
			#if defined(TWO_LAYERS)
				fixed4 colBlend = control.x * tex2D(_MainTex, IN.texCoords_FlatRef.xy * _MixScale0) + control.y * tex2D(_MainTex2, IN.texCoords_FlatRef.xy * _MixScale1);
				float3 colBlendDes=lerp((dot(colBlend.rgb, 0.33333)).xxx, colBlend.rgb, dot(control, float2(_MixSaturation0, _MixSaturation1)));
				float repl = dot( control, float2(_MixReplace0, _MixReplace1) );
				repl *= _uv_Relief_wz_no_overlap;
		        float3 blendNormal = UnpackNormal( tex2D(_BumpMap, IN.texCoords_FlatRef.xy*_MixScale0)*control.x + tex2D (_BumpMap2, IN.texCoords_FlatRef.xy*_MixScale1)*control.y );
				col.rgb=lerp(col.rgb, col.rgb*colBlendDes*dot(control, float2(_MixBrightness0, _MixBrightness1) ), lerp(blendVal, 1, repl));  
			#else
				fixed4 colBlend = tex2D(_MainTex, IN.texCoords_FlatRef.xy * _MixScale0);
				float3 colBlendDes=lerp((dot(colBlend.rgb, 0.33333)).xxx, colBlend.rgb, _MixSaturation0);
				float repl = _MixReplace0;
				repl *= _uv_Relief_wz_no_overlap;
				float3 blendNormal = UnpackNormal( tex2D(_BumpMap, IN.texCoords_FlatRef.xy*_MixScale0));
				col.rgb=lerp(col.rgb, col.rgb*colBlendDes*_MixBrightness0, lerp(blendVal, 1, repl));
			#endif
			col.rgb = lerp( col.rgb, colBlend.rgb , repl );
		#endif		    
	    
		#ifdef VERTICAL_TEXTURE
			float2 vert_tex_uv=float2(0, IN.worldPos.y/_VerticalTextureTiling);
			#ifdef GLOBAL_PERLIN
				vert_tex_uv += _VerticalTextureGlobalBumpInfluence*global_bump_val.xy;
			#endif
			half3 vert_tex=tex2D(_VerticalTexture, vert_tex_uv).rgb;
			#if defined(TWO_LAYERS)
				float _VerticalTextureStrength=dot(control, float2(_VerticalTextureStrength0, _VerticalTextureStrength1));
			#else
				float _VerticalTextureStrength=_VerticalTextureStrength0;
			#endif
			col.rgb=lerp( col.rgb, col.rgb*vert_tex*2, _VerticalTextureStrength );
		#endif
			    
      	fixed3 colAlbedo=0;
      	
      	//
      	// PBL specularity
      	//
      	float glcombined=col.a;
		#if defined(RTP_UV_BLEND)			
			glcombined=lerp(glcombined, colBlend.a, repl*0.5);					
		#endif		    
		#if defined(RTP_COLORSPACE_LINEAR)
		//glcombined=FastToLinear(glcombined);
		#endif
		#if defined(TWO_LAYERS)		      	
			float RTP_gloss2mask = dot(control, float2(RTP_gloss2mask0, RTP_gloss2mask1) );
			float _Spec = dot(control_nobleed, float2(_Spec0, _Spec1)); // anti-bleed subtraction
			float RTP_gloss_mult = dot(control, float2(RTP_gloss_mult0, RTP_gloss_mult1) );
			float RTP_gloss_shaping = dot(control, float2(RTP_gloss_shaping0, RTP_gloss_shaping1) );
		#else
			float RTP_gloss2mask = RTP_gloss2mask0;
			float _Spec = _Spec0;
			float RTP_gloss_mult = RTP_gloss_mult0;
			float RTP_gloss_shaping = RTP_gloss_shaping0;
		#endif
		float gls = saturate(glcombined * RTP_gloss_mult);
		o_Gloss =  lerp(1, gls, RTP_gloss2mask) * _Spec;
		
		float2 gloss_shaped=float2(gls, 1-gls);
		gloss_shaped=gloss_shaped*gloss_shaped*gloss_shaped;
		gls=lerp(gloss_shaped.x, 1-gloss_shaped.y, RTP_gloss_shaping);
		o.Specular = saturate(gls);
		// gloss vs. fresnel dependency
		#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			o.RTP.x*=lerp(1, 1-fresnelAtten, o.Specular*0.9+0.1);
		#endif
		half colDesat=dot(col.rgb,0.33333);
		#if defined(TWO_LAYERS)		 		
			col.rgb=lerp(colDesat.xxx, col.rgb, dot(control, float2(_LayerSaturation0, _LayerSaturation1)) );	
			float brightness2Spec=dot(control, float2(_LayerBrightness2Spec0, _LayerBrightness2Spec1));
        #else
			col.rgb=lerp(colDesat.xxx, col.rgb, _LayerSaturation0);
			float brightness2Spec=_LayerBrightness2Spec0;
        #endif
		o_Gloss*=lerp(1, colDesat, brightness2Spec);
		colAlbedo=col.rgb;
		#if defined(TWO_LAYERS)		 		
			col.rgb*=(control.x*_LayerColor0 + control.y*_LayerColor1)*2;
	      	
	        o.Normal = UnpackNormal( tex2D(_BumpMap, rayPos.xy)*control.x + tex2D (_BumpMap2, rayPos.xy)*control.y );
        #else
			col.rgb*=_LayerColor0*2;
	      	
	        o.Normal = UnpackNormal( tex2D(_BumpMap, rayPos.xy) );
        #endif
		#if defined(RTP_UV_BLEND)
			o.Normal=lerp(o.Normal, blendNormal, repl);
		#endif        
        #ifdef RTP_SNOW
        	o.Normal = lerp( o.Normal, float3(0,0,1), saturate(snow_depth)*0.5 );
        #endif
      	
		////////////////////////////////
		// water
		//
        #if defined(RTP_WETNESS)
			#ifdef RTP_CAUSTICS
				TERRAIN_WetSpecularity*=damp_fct_caustics_inv;
				TERRAIN_WetGloss*=damp_fct_caustics_inv;
				TERRAIN_WaterSpecularity*=damp_fct_caustics_inv;
				TERRAIN_WaterGloss*=damp_fct_caustics_inv;
			#endif
	  		float porosity = 1-saturate(o.Specular * 4 - 1);
			#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
	        o.RTP.x = lerp(o.RTP.x, TERRAIN_WaterColor.a, TERRAIN_LayerWetStrength);
	        #endif
			float wet_fct = saturate(TERRAIN_LayerWetStrength*2-0.4);
			float glossDamper=lerp( (1-TERRAIN_WaterGlossDamper), 1, _uv_Relief_z); // odległość>near daje całkowite tłumienie
	        o.Specular += lerp(TERRAIN_WetGloss, TERRAIN_WaterGloss, p) * wet_fct * glossDamper; // glossiness boost
	        o.Specular=saturate(o.Specular);
	        o_Gloss += lerp(TERRAIN_WetSpecularity, TERRAIN_WaterSpecularity, p) * wet_fct; // spec boost
	        o_Gloss=max(0, o_Gloss);
	  		
	  		// col - saturation, brightness
	  		half3 col_sat=col.rgb*col.rgb; // saturation z utrzymaniem jasności
	  		col_sat*=dot(col.rgb,1)/dot(col_sat,1);
	  		wet_fct=saturate(TERRAIN_LayerWetStrength*(2-perlinmask));
	  		porosity*=0.5;
	  		col.rgb=lerp(col.rgb, col_sat, wet_fct*porosity);
			col.rgb*=1-wet_fct*TERRAIN_WetDarkening*(porosity+0.5);
					  		
	        // col - colorisation
	        col.rgb *= lerp(half3(1,1,1), TERRAIN_WaterColor.rgb, p*p);
	        
 			// col - opacity
			col.rgb = lerp(col.rgb, TERRAIN_WaterColor.rgb, _WaterOpacity );
			colAlbedo=lerp(colAlbedo, col.rgb, _WaterOpacity); // potrzebne do spec color				
	        
	        o.Normal = lerp(o.Normal, float3(0,0,1), max(p*0.7, _WaterOpacity));
	        o.Normal.xy+=roff;
        #endif
		// water
		////////////////////////////////      	
		
		float3 norm_snowCov=o.Normal;

				
		#if defined(TWO_LAYERS)
			float _BumpMapGlobalStrengthPerLayer=dot(control, float2(_BumpMapGlobalStrength0, _BumpMapGlobalStrength1));
		#else
			float _BumpMapGlobalStrengthPerLayer=_BumpMapGlobalStrength0;
		#endif
		#if !defined(RTP_SIMPLE_SHADING)
			{
			float3 tangentBase = normalize(cross(float3(0.0,1.0,0.0), norm_far));
			float3 binormalBase = normalize(cross(norm_far, tangentBase));
			float3 combinedNormal = tangentBase * o.Normal.x + binormalBase * o.Normal.y + norm_far * o.Normal.z;
			o.Normal = lerp(o.Normal, combinedNormal, lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapGlobalStrengthPerLayer);
			}
		#else
			o.Normal+=norm_far*lerp(rtp_perlin_start_val,1, _uv_Relief_w)*_BumpMapGlobalStrengthPerLayer;	
		#endif


		#ifdef COLOR_MAP
			float colBrightness=dot(col,1);
			#ifdef RTP_WETNESS
				global_color_blend*=(1-_WaterOpacity);
			#endif
			
			// basic global colormap blending
			#ifdef COLOR_MAP_BLEND_MULTIPLY
				col.rgb=lerp(col.rgb, col.rgb*global_color_value.rgb*2, global_color_blend);
			#else
				col.rgb=lerp(col.rgb, global_color_value.rgb, global_color_blend);
			#endif

			#ifdef RTP_IBL_DIFFUSE
				half3 colBrightnessNotAffectedByColormap=col.rgb*colBrightness/max(0.01, dot(col.rgb,float3(1,1,1)));
			#endif
		#else
			#ifdef RTP_IBL_DIFFUSE
				half3 colBrightnessNotAffectedByColormap=col.rgb;
			#endif
		#endif		
		
		#ifdef RTP_SNOW
			//rayPos.xy=lerp(rayPos.xy, IN.texCoords_FlatRef.xy, snow_depth_lerp);
		
			#ifdef COLOR_MAP
				snow_val = snow_const + rtp_snow_strength*dot(1-global_color_value.rgb, rtp_global_color_brightness_to_snow.xxx)+rtp_snow_strength*2;
			#else
				snow_val = snow_const + rtp_snow_strength*0.5*rtp_global_color_brightness_to_snow+rtp_snow_strength*2;
			#endif
			
			snow_val -= rtp_snow_slope_factor*saturate( 1 - dot( (norm_snowCov*0.7+norm_for_snow*0.3), flat_dir.xyz) - 0*dot( norm_for_snow, flat_dir.xyz));
			
			snow_val=saturate(snow_val);
			snow_val=pow(abs(snow_val), rtp_snow_edge_definition);
			
			#ifdef COLOR_MAP
				half3 global_color_value_desaturated=dot(global_color_value.rgb, 0.37);//0.3333333); // będzie trochę jasniej
				#ifdef COLOR_MAP_BLEND_MULTIPLY
					rtp_snow_color_tex=lerp(rtp_snow_color_tex, rtp_snow_color_tex*global_color_value_desaturated.rgb*2, min(0.4,global_color_blend*0.7) );
				#else
					rtp_snow_color_tex=lerp(rtp_snow_color_tex, global_color_value_desaturated.rgb, min(0.4,global_color_blend*0.7) );
				#endif
			#endif
	
			col.rgb=lerp( col.rgb, rtp_snow_color_tex, snow_val );
			#ifdef RTP_IBL_DIFFUSE
				colBrightnessNotAffectedByColormap=lerp( colBrightnessNotAffectedByColormap, rtp_snow_color_tex, snow_val );
			#endif
			
			float3 snow_normal=o.Normal;
			snow_normal=norm_for_snow + 2*snow_normal*(_uv_Relief_z*0.5+0.5);
			
			snow_normal=normalize(snow_normal);
			o.Normal=lerp(o.Normal, snow_normal, snow_val);
			float rtp_snow_specular_distAtten=rtp_snow_specular;
			o_Gloss=lerp(o_Gloss, rtp_snow_specular, snow_val);
			// przeniesione pod emisję (która zależy od specular materiału _pod_ śniegiem)
			//o.Specular=lerp(o.Specular, rtp_snow_gloss, snow_val);
			#if defined(RTP_REFLECTION) || defined(RTP_IBL_SPEC) || defined(RTP_PBL_FRESNEL)
			o.RTP.x=lerp(o.RTP.x, rtp_snow_fresnel, snow_val);
			#endif
			float snow_damp=saturate(1-snow_val*2);
		#endif
				
		// emission of layer (inside)
		#ifdef RTP_EMISSION
			#ifdef RTP_SNOW
				layer_emission *= snow_damp*0.9+0.1; // delikatna emisja poprzez snieg
			#endif
			
			#if defined(RTP_WETNESS)
				layer_emission *= lerp(o.Specular, 1, p) * 2;
				// zróżnicowanie koloru na postawie animowanych normalnych
				#ifdef RTP_FUILD_EMISSION_WRAP
					float norm_fluid_val=lerp( 0.7, saturate(dot(o.Normal.xy*4, o.Normal.xy*4)), _uv_Relief_z);
					o.Emission += (col.rgb + _LayerEmissionColor.rgb ) * ( norm_fluid_val*p+0.15 ) * layer_emission * 4;
				#else
					float norm_fluid_val=lerp( 0.5, (o.Normal.x+o.Normal.y), _uv_Relief_z);
					o.Emission += (col.rgb + _LayerEmissionColor.rgb ) * ( saturate(norm_fluid_val*2*p)*1.2+0.15 ) * layer_emission * 4;
				#endif
			#else
				layer_emission *= o.Specular * 2;
				o.Emission += (col.rgb + _LayerEmissionColor.rgb*0.2 ) * layer_emission * 4;
			#endif
			layer_emission = max(0, 1 - layer_emission);
			o_Gloss *= layer_emission;
			o.Specular *= layer_emission;
			col.rgb *= layer_emission;
		#endif		
		
		// przeniesione pod emisję (która zależy od specular materiału _pod_ śniegiem)
		#ifdef RTP_SNOW
			o.Specular=lerp(o.Specular, rtp_snow_gloss, snow_val);
		#endif	
			
		o.Normal=normalize(o.Normal);
		o.Albedo=col.rgb;
		
		#if defined(VERTEX_COLOR_AO_DAMP)
			o.RTP.y=VERTEX_COLOR_AO_DAMP;
		#endif
				
		// ^4 shaped diffuse fresnel term for soft surface layers (grass)
		#if defined(TWO_LAYERS)		
			float _DiffFresnel=dot( control, float2(RTP_DiffFresnel0, RTP_DiffFresnel1) );
		#else
			float _DiffFresnel=RTP_DiffFresnel0;
		#endif
		// diffuse fresnel term for snow
		#ifdef RTP_SNOW
			_DiffFresnel=lerp(_DiffFresnel, rtp_snow_diff_fresnel, snow_val);
		#endif
		float diffuseScatteringFactor=1.0 + diffFresnel*_DiffFresnel;
		o.Albedo *= diffuseScatteringFactor;
		#ifdef RTP_IBL_DIFFUSE
			colBrightnessNotAffectedByColormap *= diffuseScatteringFactor;
		#endif
		
		//  spec color from albedo (metal tint)
		#if defined(TWO_LAYERS)
			float Albedo2SpecColor=dot(control, float2(_LayerAlbedo2SpecColor0, _LayerAlbedo2SpecColor1) );
		#else
			float Albedo2SpecColor=_LayerAlbedo2SpecColor0;
		#endif
		#ifdef RTP_SNOW
			Albedo2SpecColor*=snow_damp;
		#endif
		#ifdef RTP_WETNESS
			colAlbedo=lerp(colAlbedo, o.Albedo, p);
		#endif
		#if defined(TWO_LAYERS)
			o_Gloss=lerp( saturate(o_Gloss+4*dot(control_nobleed, float2(_FarSpecCorrection0,_FarSpecCorrection1) )), o_Gloss, (1-_uv_Relief_w)*(1-_uv_Relief_w) );
		#else
			o_Gloss=lerp( saturate(o_Gloss+4*_FarSpecCorrection0), o_Gloss, (1-_uv_Relief_w)*(1-_uv_Relief_w) );
		#endif
		float colAlbedoRGBmax=max(max(colAlbedo.r, colAlbedo.g), colAlbedo.b);
		float3 colAlbedoNew=lerp(half3(1,1,1), colAlbedo.rgb/colAlbedoRGBmax.xxx, saturate(colAlbedoRGBmax*4)*Albedo2SpecColor);
		half3 SpecColor=_SpecColor.rgb*o_Gloss*colAlbedoNew*colAlbedoNew; // spec color for IBL/Refl
		o.SpecColor=SpecColor;
		
		#if defined(RTP_AMBIENT_EMISSIVE_MAP)
			float4 eMapVal=tex2D(_AmbientEmissiveMapGlobal, globalUV);
			o.Emission+=o.Albedo*eMapVal.rgb*_AmbientEmissiveMultiplier*lerp(1, saturate(o.Normal.z*o.Normal.z-(1-actH)*(1-o.Normal.z*o.Normal.z)), _AmbientEmissiveRelief);
			float pixel_trees_shadow_val=saturate((_distance - _shadow_distance_start) / _shadow_distance_transition);
			pixel_trees_shadow_val=lerp(1, eMapVal.a, pixel_trees_shadow_val);
			float o_RTP_y_without_shadowmap_distancefade=o.RTP.y*lerp(1, eMapVal.a, _shadow_value);
			o.RTP.y*=lerp((1-_shadow_value), 1, pixel_trees_shadow_val);
		#else
			#define o_RTP_y_without_shadowmap_distancefade (o.RTP.y)
		#endif		
				
		#if defined(TWO_LAYERS)	
			float IBL_bump_smoothness=dot(control, float2(RTP_IBL_bump_smoothness0, RTP_IBL_bump_smoothness1) );
			#ifdef RTP_IBL_DIFFUSE
				float RTP_IBL_DiffStrength=dot(control, float2(RTP_IBL_DiffuseStrength0, RTP_IBL_DiffuseStrength1) );
			#endif
			#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
				// anti-bleed subtraction
				float RTP_IBL_SpecStrength=dot(control_nobleed, float2(RTP_IBL_SpecStrength0, RTP_IBL_SpecStrength1) );
			#endif
		#else
			float IBL_bump_smoothness=RTP_IBL_bump_smoothness0;
			#ifdef RTP_IBL_DIFFUSE
				float RTP_IBL_DiffStrength=RTP_IBL_DiffuseStrength0;
			#endif
			#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
				// anti-bleed subtraction
				float RTP_IBL_SpecStrength=RTP_IBL_SpecStrength0;
			#endif
		#endif
		
		#if defined(RTP_IBL_DIFFUSE) || defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			float3 IBLNormal=lerp(o.Normal, float3(0,0,1), IBL_bump_smoothness);
			
			float3 normalW=WorldNormalVector(IN, IBLNormal);
		#endif
		// lerp IBL values with wet / snow
		#ifdef RTP_IBL_DIFFUSE
			#ifdef RTP_SNOW
				RTP_IBL_DiffStrength=lerp(RTP_IBL_DiffStrength, rtp_snow_IBL_DiffuseStrength, snow_val);
			#endif
	   		#if defined(RTP_SKYSHOP_SKY_ROTATION)
				float3 normalWR = _SkyMatrix[0].xyz*normalW.x + _SkyMatrix[1].xyz*normalW.y + _SkyMatrix[2].xyz*normalW.z;
			#else
				float3 normalWR = normalW;
			#endif					
			#ifdef RTP_SKYSHOP_SYNC
				half3 IBLDiffuseCol = SHLookup(normalWR)*RTP_IBL_DiffStrength;
			#else
				#ifdef UNITY_PI
					half3 IBLDiffuseCol = ShadeSH9(float4(normalWR,1))*RTP_IBL_DiffStrength;
					//IBLDiffuseCol=RTP_IBL_DiffStrength;
				#else
					half3 IBLDiffuseCol = DecodeRGBM(texCUBElod(_DiffCubeIBL, float4(normalWR,0)))*RTP_IBL_DiffStrength;
				#endif	
			#endif
			IBLDiffuseCol*=colBrightnessNotAffectedByColormap * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBL_DiffAO_Damp);
			#ifdef RTP_SKYSHOP_SYNC
				IBLDiffuseCol*=_ExposureIBL.x;
			#endif	
			#ifndef RTP_IBL_SPEC
			o.Emission += IBLDiffuseCol.rgb;
			#else
			// dodamy za chwilę poprzez introplację, która zachowa energie
			#endif
		#endif
		#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			#ifdef RTP_WETNESS
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, TERRAIN_WaterIBL_SpecWetStrength, TERRAIN_LayerWetStrength);
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, TERRAIN_WaterIBL_SpecWaterStrength, p*p);
			#endif
			#ifdef RTP_SNOW
				RTP_IBL_SpecStrength=lerp(RTP_IBL_SpecStrength, rtp_snow_IBL_SpecStrength, snow_val);
			#endif
		#endif		
		
		// kompresuję odwrotnie mip blur (łatwiej osiągnąć "lustro")
		#if defined(RTP_IBL_SPEC) || defined(RTP_REFLECTION)
			float o_SpecularInvSquared = (1-o.Specular)*(1-o.Specular);
			float3 reflVec=WorldReflectionVector(IN, IBLNormal);
			#if defined(RTP_SKYSHOP_SKY_ROTATION)
			 	reflVec = _SkyMatrix[0].xyz*reflVec.x + _SkyMatrix[1].xyz*reflVec.y + _SkyMatrix[2].xyz*reflVec.z;
			#endif		
		#endif
		
		#ifdef RTP_IBL_SPEC
			float n_dot_v = saturate(dot(IBLNormal, IN.viewDir));
			float exponential = exp2(SchlickFresnelApproxExp2Const*n_dot_v);
			
			// skyshop fit (I'd like people to get similar results in gamma / linear)
			#if defined(RTP_COLORSPACE_LINEAR)
				exponential=0.03+0.97*exponential;
			#else
				exponential=0.25+0.75*exponential;
			#endif
			float spec_fresnel = lerp (1.0f, exponential, o.RTP.x);
			
			// U5 reflection probe
			#ifdef UNITY_PI
				half4 rgbm = SampleCubeReflection( unity_SpecCube0, reflVec, o_SpecularInvSquared*(6-exponential*o.RTP.x*3) );
				half3 IBLSpecCol = DecodeHDR_NoLinearSupportInSM2 (rgbm, unity_SpecCube0_HDR) * RTP_IBL_SpecStrength;
			#else
		        // U4 reflection probe
	   			half3 IBLSpecCol = DecodeRGBM(texCUBElod (_SpecCubeIBL, float4(reflVec, o_SpecularInvSquared*(6-exponential*o.RTP.x*3))))*RTP_IBL_SpecStrength;
			#endif 	
			
			IBLSpecCol.rgb*=spec_fresnel * SpecColor * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBLRefl_SpecAO_Damp);
			#ifdef RTP_SKYSHOP_SYNC
				IBLSpecCol.rgb*=_ExposureIBL.y;
			#endif			
			#ifdef RTP_IBL_DIFFUSE
				// link difuse and spec IBL together with energy conservation
				o.Emission += saturate(1-IBLSpecCol.rgb) * IBLDiffuseCol.rgb + IBLSpecCol.rgb;
			#else
				o.Emission+=IBLSpecCol.rgb;
			#endif
		#endif
	
	    #if defined(RTP_REFLECTION)
			float2 mip_selectorRefl=o_SpecularInvSquared*(8-diffFresnel*o.RTP.x*4);
			#ifdef RTP_ROTATE_REFLECTION
				float3 refl_rot;
				refl_rot.x=sin(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.y=cos(_Time.x*TERRAIN_ReflectionRotSpeed);
				refl_rot.z=-refl_rot.x;
				float2 tmpRefl;
				tmpRefl.x=dot(reflVec.xz, refl_rot.yz);
				tmpRefl.y=dot(reflVec.xz, refl_rot.xy);
				float t=tex2Dlod(_BumpMapGlobal, float4(tmpRefl*0.5+0.5, mip_selectorRefl)).a;
			#else
				float t=tex2Dlod(_BumpMapGlobal, float4(reflVec.xz*0.5+0.5, mip_selectorRefl)).a;
			#endif				
			#ifdef RTP_IBL_SPEC
				half rim=spec_fresnel;
			#else
				float n_dot_v = saturate(dot(IBLNormal, IN.viewDir));
				float exponential = exp2(SchlickFresnelApproxExp2Const*n_dot_v);	
				half rim= lerp(1, exponential, o.RTP.x);
		    #endif
		    float downSideEnvelope=saturate(reflVec.y*3);
		    t *= downSideEnvelope;
		    rim *= downSideEnvelope*0.7+0.3;
			#if defined(RTP_SIMPLE_SHADING)
				rim*=RTP_IBL_SpecStrength*_uv_Relief_z;
			#else
				rim*=RTP_IBL_SpecStrength;
			#endif
			rim-=o_SpecularInvSquared*rim*TERRAIN_ReflGlossAttenuation; // attenuate low gloss
			
			half3 reflCol;
			reflCol=lerp(TERRAIN_ReflColorB.rgb, TERRAIN_ReflColorC.rgb, saturate(TERRAIN_ReflColorCenter-t) / TERRAIN_ReflColorCenter );
			reflCol=lerp(reflCol.rgb, TERRAIN_ReflColorA.rgb, saturate(t-TERRAIN_ReflColorCenter) / (1-TERRAIN_ReflColorCenter) );
			o.Emission += reflCol * SpecColor * lerp(1, o_RTP_y_without_shadowmap_distancefade, TERRAIN_IBLRefl_SpecAO_Damp) * rim * 2;
		#endif		
		
		#ifdef RTP_CAUSTICS
		{
			float tim=_Time.x*TERRAIN_CausticsAnimSpeed;
			rayPos.xy=IN.worldPos.xz*TERRAIN_CausticsTilingScale;
			#ifdef RTP_VERTALPHA_CAUSTICS
				float3 _Emission=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(tim, tim) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(-tim, -tim*0.873) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*1.1+float2(tim, 0) ).aaa;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*0.5+float2(0, tim*0.83) ).aaa;
			#else
				float3 _Emission=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(tim, tim) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy+float2(-tim, -tim*0.873) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*1.1+float2(tim, 0) ).rgb;
				_Emission+=tex2D(TERRAIN_CausticsTex, rayPos.xy*0.5+float2(0, tim*0.83) ).rgb;
			#endif
			_Emission=saturate(_Emission-1.55);
			_Emission*=_Emission;
			_Emission*=_Emission;
			_Emission*=TERRAIN_CausticsColor.rgb*8;
			_Emission*=damp_fct_caustics;
			//_Emission*=(1-_uv_Relief_w);
			o.Emission+=_Emission;
		} 
		#endif		
		
		#if defined(UNITY_PASS_PREPASSBASE)	 || defined(UNITY_PASS_PREPASSFINAL)
			o.Specular=max(0.01, o.Specular);
		#endif
	
		#if defined(UNITY_PASS_PREPASSFINAL)	
			#if defined(RTP_DEFERRED_PBL_NORMALISATION)
				o.Specular=1-o.Specular;
				o.Specular*=o.Specular;
				o.Specular=1-o.Specular;
				// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
				float specular_power=exp2(10*o.Specular+1) - 1.75;
				float normalisation_term = specular_power / 8.0f;
				o.SpecColor*=normalisation_term;
			#endif
		#endif		
		
		#if defined(GEOM_BLEND)
			#if defined(BLENDING_HEIGHT)
				float4 terrain_coverage=tex2D(_TERRAIN_Control, globalUV);
				float2 tiledUV=(IN.worldPos.xz-_TERRAIN_PosSize.xy+_TERRAIN_Tiling.zw)/_TERRAIN_Tiling.xy;
				float4 splat_control1=terrain_coverage * tex2D(_TERRAIN_HeightMap, tiledUV) * IN.color.a;
				#if defined(TWO_LAYERS)
					float4 splat_control2=float4(control_orig, 0, 0) * (1-IN.color.a);
				#else
					float4 splat_control2=float4(actH+0.01, 0, 0, 0) * (1-IN.color.a);
				#endif

				float blend_coverage=dot(terrain_coverage, 1);
				if (blend_coverage>0.1) {

					splat_control1*=splat_control1;
					splat_control1*=splat_control1;
					splat_control1*=splat_control1;
					splat_control2*=splat_control2;
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
				o.SpecColor*=o.Alpha;
			#endif		
		#endif
		
		// HACK-ish workaround - Unity skips passing color if IN.color is not explicitly used here in shader
		o.Albedo+=IN.color.xyz*0.0001;	
	}