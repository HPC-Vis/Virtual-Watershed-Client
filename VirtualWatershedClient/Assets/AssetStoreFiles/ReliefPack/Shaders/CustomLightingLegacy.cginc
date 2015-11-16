half3 rtp_customAmbientCorrection;

#define RTP_BackLightStrength RTP_LightDefVector.x
#define RTP_ReflexLightDiffuseSoftness RTP_LightDefVector.y
float4 RTP_LightDefVector;
half4 RTP_ReflexLightDiffuseColor1;
half4 RTP_ReflexLightSpecColor;	

#ifdef UNITY_PI
	//
	// Unity 5
	//
	// function called by LightingCustomBlinnPhong (called in surface shader) below
	inline fixed4 RTPLightingCustomBlinnPhong (SurfaceOutput s, half3 viewDir, UnityLight light, bool main_light)
	{
		half3 h = normalize (light.dir + viewDir);
		
		half3 lightDir=light.dir;
		fixed diff = light.ndotl;
		
		float nh = max (0, dot (s.Normal, h));
		float spec = pow (nh, s.Specular*128.0) * s.Gloss;
		
		fixed4 c=0;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		c.rgb += (s.Albedo * light.color.rgb * diff + light.color.rgb * _SpecColor.rgb * spec) + rtp_customAmbientCorrection*0.5;
		c.a = s.Alpha;// + _LightColor0.a * _SpecColor.a * spec * atten;
			
		//		
		// reflex lights
		//
		if (main_light) {
		#if defined(RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
			float3 normForDiffuse=lerp(s.Normal, float3(0,0,1), RTP_ReflexLightDiffuseSoftness);
			float glossDiff=(saturate(s.Gloss)+1)*RTP_ReflexLightDiffuseColor1.a;
					
			lightDir.y=-0.7; // 45 degrees
			lightDir=normalize(lightDir);
			
			float3 lightDirRefl;
			float3 refl_rot;
			refl_rot.x=0.86602540378443864676372317075294;// = sin(+120deg);
			refl_rot.y=-0.5; // = cos(+/-120deg);
			refl_rot.z=-refl_rot.x;
			
			// 1st reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yz);
			lightDirRefl.y=lightDir.y;
			lightDirRefl.z=dot(lightDir.xz, refl_rot.xy);	
			diff = abs( dot (normForDiffuse, lightDirRefl) )*glossDiff; 
			float3 reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;
			
			// 2nd reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
			lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
			diff = abs ( dot (normForDiffuse, lightDirRefl) )*glossDiff;
			reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;
		#endif
		}
		
		return c;
	}

	// function called in surface shader
	inline fixed4 LightingCustomBlinnPhong (SurfaceOutput s, half3 viewDir, UnityGI gi)
	{
		// input gi pochodzi z LightingCustomBlinnPhong_GI() i wywolywanego w nim UnityGlobalIllumination()
		// zawiera light, light2, light3 + indirect

		// jesli mamy mix realtime light z bakowanymi lightmapami - ForwardBase obsluzy lightmapy i ambient, ForwardAdd obsluzy realtime light
		// ForwadBase w ogóle nie potrafi obsłużyć realtime'owych świateł innych niż directional (wywołuje ForwardBase z zerowym gi.light, więc miejmy nadzieję, że wywołanie RTPLightingCustomBlinnPhong((UnityLight)0) sie wykompiluje),
		// Unity natomiast zawsze obsłuży w ForwadBase oświetlenie ambient i indirect

		// non-directional:
		//
		// całe światło z lightmap (baked i dynamic) jest akumulowane w gi.indirect.diffuse
		// dodatkowo jego natężenie jest przpuszczone przez MixLightmapWithRealtimeAttenuation() aby połączyć to z atten
		//
		
		// directional:
		//
		// całe światło z lightmap (baked i dynamic) jest akumulowane w gi.indirect.diffuse, ale jego natężenie jest ukierunkowane (taki prosty "Lambert")
		// dodatkowo natężenie światła jest przpuszczone przez MixLightmapWithRealtimeAttenuation() aby połączyć to z atten
		//
		
		// separate directional (kapkę zakręcone):
		//
		// 1. dla lightmap bake'owanych samplujemy 2 lightmapy (direct i indirect, ta druga najprawdopodobniej zawiera światło odbite i emisyjne)
		// każdy z wyników jest rozbijany na 2 składniki "ambient" i "direct" (w funkcji DecodeDirectionalSpecularLightmap())
		// składnik "ambient" akumuluje się w indirect.diffuse (dla direct i indirect)
		// składnik "direct" odkłada się w gi.light dla lightmapy direct (tutaj zapewne jest już zbake'owany atten) i w gi.light2 dla lightmapy indirect
		//
		// 2. dla lightmap dynamicznych samplujemy 1 lightmapę (tą lo-res, zawierającą światło indirect - odbite i emisyjne)
		// jak dla bake'owanych wynik jest rozbijany na 2 składniki "ambient" i "direct" (w funkcji DecodeDirectionalSpecularLightmap())
		// składnik "ambient" akumuluje się w indirect.diffuse
		// składnik "direct" odkłada się w gi.light3

		// gi.light - realtime direct light (z zaaplikowanym atten) kiedy nie mamy baked lightmap typu separate directional (czyli zawsze dla ForwardAdd)
		//		    - baked "direct term" z lightmapy direct dla lightmap typu separate directional
		//			- 0 (wszystkie skladniki wyzerowane) dla pozostalych baked lightmap (non directional i directional)
		//			    dla powyzszego oswietlenie z baked lightmap jest zawarte tylko w gi.indirect.diffuse (jak wspomniałem powyżej miejmy nadzieje, ze wywolanie RTPLightingCustomBlinnPhong z zerowym swiatlem sie po prostu wykompiluje, choć idealnie by było gdyby gi.light zawierało światlo realtime, zamiast wywoływać je w ForwardAdd niezależnie od rodzaju lightmapy...)
		//
		// gi.light2 - tylko dla baked lightmap typu separate directional - zawiera "direct term" z lightmapy indirect
		//
		// gi.light3 - tylko dla realtime lightmap - zawiera "direct term" z lightmapy (która jest z definicji indirect, czyli zawiera światło odbite i emisyjne)
		//
		fixed4 c=0;
		// direct realtime light lub "direct" term z lightmapy direct dla separate directional
		c = RTPLightingCustomBlinnPhong (s, viewDir, gi.light, true); // complementary lighting & backlight applied only here (RTPLightingCustomBlinnPhong() call with last param == true)

		#if defined(DIRLIGHTMAP_SEPARATE)
			#ifdef LIGHTMAP_ON
				// baked indirect lightmap with its "direct term"
				c += RTPLightingCustomBlinnPhong (s, viewDir, gi.light2, false);
			#endif
			#ifdef DYNAMICLIGHTMAP_ON
				// dyn lightmap (czyli swiatlo indirect) with its "direct term"
				c += RTPLightingCustomBlinnPhong (s, viewDir, gi.light3, false);
			#endif
		#endif

		// UNITY_LIGHT_FUNCTION_APPLY_INDIRECT = defined(UNITY_SHOULD_SAMPLE_SH) || defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)
		// (nigdy sie nie wywola dla ForwardAdd)
		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			// indirect.diffuse sklada sie z:
			// bez lightmap - SH ambient - zdefiniowane UNITY_SHOULD_SAMPLE_SH = ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )
			//		- ShadeSH9 per pixel w UnityGlobalIllumination - jesli jest zdefiniowane UNITY_SAMPLE_FULL_SH_PER_PIXEL (a nie jest - nigdzie nie wystepuje dla surface shaderow, a w UnityStandardConfig.cginc jest zdefiniowane defaultowo na 0)
			//		UNITY_SAMPLE_FULL_SH_PER_PIXEL nie zdefiniowane:
			//		- ShadeSH9 per vertex dla SM<3 
			//		- ShadeSH3Order per vertex dla SM>=3 + ShadeSH12Order per pixel w UnityGlobalIllumination
			// lightmapy (osobno baked i realtime dodawane):
			//		- non directional - tylko kolor z lightmapy po obróbce funkcją MixLightmapWithRealtimeAttenuation (mix koloru z lightmapy z realtime shadows)
			//		- directional combined - kolor z lightmapy ale z uwzględnieniem textury kierunku (czyli oswietlenie typu "Lambertian") + obróbka MixLightmapWithRealtimeAttenuation
			//		- directional separate - samplowane 2 kierunkowe textury (direct+indirect), ale brany jest tutaj tylko ich składnik "ambient" (funkcja dekodująca rozbija kolor na "direct" i "ambient")
			c.rgb += s.Albedo * gi.indirect.diffuse;
		#endif

		return c;
	}
	
	inline half4 LightingCustomBlinnPhong_Deferred (SurfaceOutput s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
	{
		outDiffuseOcclusion = half4(s.Albedo, 1);
		outSpecSmoothness = half4(_SpecColor.rgb, s.Specular);
		outNormal = half4(s.Normal * 0.5 + 0.5, 1);
		half4 emission = half4(s.Emission, 1);
		
		emission.rgb += RTPLightingCustomBlinnPhong (s, viewDir, gi.light, true).rgb; // complementary lighting & backlight applied only here (RTPLightingCustomBlinnPhong() call with last param == true)

		#if defined(DIRLIGHTMAP_SEPARATE)
			#ifdef LIGHTMAP_ON
				// baked indirect lightmap with its "direct term"
				emission.rgb += RTPLightingCustomBlinnPhong (s, viewDir, gi.light2, false);
			#endif
			#ifdef DYNAMICLIGHTMAP_ON
				// dyn lightmap (czyli swiatlo indirect) with its "direct term"
				emission.rgb += RTPLightingCustomBlinnPhong (s, viewDir, gi.light3, false);
			#endif
		#endif
		
		#ifdef UNITY_LIGHT_FUNCTION_APPLY_INDIRECT
			emission.rgb += s.Albedo * gi.indirect.diffuse;
		#endif
		
		return emission;
	}

	inline void LightingCustomBlinnPhong_GI (
		SurfaceOutput s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		//inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld, bool reflections)
		//FIXME - s.Gloss (jak w Lighting.cginc) czy jednak s.Specular ???
		gi = UnityGlobalIllumination (data, 1.0, s.Gloss, s.Normal, false);
	}

	inline fixed4 LightingCustomBlinnPhong_PrePass (SurfaceOutput s, half4 light)
	{
		fixed spec = light.a * s.Gloss;
		
		fixed4 c;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec) + rtp_customAmbientCorrection*0.5;
		c.a = s.Alpha;// + spec * _SpecColor.a;
		return c;
	}

#else
	//
	// Unity 4
	//
	inline fixed4 LightingCustomBlinnPhong (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
	{
		half3 h = normalize (lightDir + viewDir);
		
		fixed diff = dot (s.Normal, lightDir);
		diff = saturate(diff);
		
		float nh = max (0, dot (s.Normal, h));
		float spec = pow (nh, s.Specular*128.0) * s.Gloss;
		
		fixed4 c=0;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2)  + rtp_customAmbientCorrection*0.5;
		c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
			
		//		
		// reflex lights
		//
		#if defined(RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
			float3 normForDiffuse=lerp(s.Normal, float3(0,0,1), RTP_ReflexLightDiffuseSoftness);
			float glossDiff=(saturate(s.Gloss)+1)*RTP_ReflexLightDiffuseColor1.a;
					
			lightDir.y=-0.7; // 45 degrees
			lightDir=normalize(lightDir);
			
			float3 lightDirRefl;
			float3 refl_rot;
			refl_rot.x=0.86602540378443864676372317075294;// = sin(+120deg);
			refl_rot.y=-0.5; // = cos(+/-120deg);
			refl_rot.z=-refl_rot.x;
			
			// 1st reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yz);
			lightDirRefl.y=lightDir.y;
			lightDirRefl.z=dot(lightDir.xz, refl_rot.xy);	
			diff = abs( dot (normForDiffuse, lightDirRefl) )*glossDiff; 
			float3 reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;
			
			// 2nd reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
			lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
			diff = abs ( dot (normForDiffuse, lightDirRefl) )*glossDiff;
			reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;
		#endif
		
		return c;
	}

	inline fixed4 LightingCustomBlinnPhong_PrePass (SurfaceOutput s, half4 light)
	{
		fixed spec = light.a * s.Gloss;
		
		fixed4 c;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		c.rgb = (s.Albedo * light.rgb + light.rgb * _SpecColor.rgb * spec) + rtp_customAmbientCorrection*0.5;
		c.a = s.Alpha + spec * _SpecColor.a;
		return c;
	}

	inline half4 LightingCustomBlinnPhong_DirLightmap (SurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
	{
		UNITY_DIRBASIS
		half3 scalePerBasisVector;
		
		color.rgb*=rtp_customAmbientCorrection*2+1;
		half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector) + rtp_customAmbientCorrection*0.5;
		
		half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);
		half3 h = normalize (lightDir + viewDir);

		float nh = max (0, dot (s.Normal, h));
		float spec = pow (nh, s.Specular * 128.0);
		
		// specColor used outside in the forward path, compiled out in prepass
		specColor = lm * _SpecColor.rgb * s.Gloss * spec;
		
		// spec from the alpha component is used to calculate specular
		// in the Lighting*_Prepass function, it's not used in forward
		return half4(lm, spec);
	}
#endif