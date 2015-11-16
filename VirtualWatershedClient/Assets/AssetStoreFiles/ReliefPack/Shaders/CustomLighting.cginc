/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// aux functions & lighting

// quick gamma to linear approx of pow(n,2.2) function
inline float FastToLinear(float t) {
		t *= t * (t * 0.305306011 + 0.682171111) + 0.012522878;
		return t;
}

half3 DecodeRGBM(float4 rgbm)
{
	#ifdef IBL_HDR_RGBM
		// gamma/linear RGBM decoding
		#if defined(RTP_COLORSPACE_LINEAR)
	    	return rgbm.rgb * FastToLinear(rgbm.a) * 8;
	    #else
	    	return rgbm.rgb * rgbm.a * 8;
	    #endif
	#else
    	return rgbm.rgb;
	#endif
}

half2 normalEncode (half3 n)
{
    half scale = 1.7777;
    half2 enc = n.xy / (n.z+1);
    enc /= scale;
    enc = enc*0.5+0.5;
    return enc;
}

half3 normalDecode(half2 enc)
{
    half scale = 1.7777;
    half3 nn =
        enc.xyy*half3(2*scale,2*scale,0) +
        half3(-scale,-scale,1);
    half g = 2.0 / dot(nn.xyz,nn.xyz);
    half3 n;
    n.xy = g*nn.xy;
    n.z = g-1;
    return n;
}

#define SchlickFresnelApproxExp2Const (-8.6562)

fixed3 RTP_ambLight;
// (pre pass lighting function for surface shaders looks the same for U4 and U5)
inline fixed4 LightingCustomBlinnPhong_PrePass (in RTPSurfaceOutput s, half4 light)
{
	#if defined(RTP_DEFERRED_PBL_NORMALISATION)
		fixed spec = light.a; // *SpecColor intensity ? (it's s.Gloss in original Unity's prepass function for BlinnPhong)
	#else
		// we assume Lux is used with its compressed specular luminance term
		fixed spec = exp2(light.a) - 1;
	#endif
	
	fixed4 c;
	s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
	c.rgb = (s.Albedo * light.rgb + light.rgb * s.SpecColor * spec) *s.RTP.y + rtp_customAmbientCorrection*0.5;
	#if defined(LIGHTMAP_OFF) && !defined(RTP_AMBIENT_EMISSIVE_MAP)
		c.rgb += (s.Albedo*RTP_ambLight)*(1-s.RTP.y);
	#endif
	c.a = s.Alpha;// + spec * _SpecColor.a;
	return c;
}

#ifdef UNITY_PI
	//
	// Unity 5
	//
	// function called by LightingCustomBlinnPhong (called in surface shader) below
	inline fixed4 RTPLightingCustomBlinnPhong (in RTPSurfaceOutput s, half3 viewDir, UnityLight light, bool main_light)
	{
		half3 h = normalize (light.dir + viewDir);
		
		fixed diff;
		float diffBack;
		if (main_light) { // (uwaga - to jest static branch (main_light jest stałą)
			#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
				diff = dot (s.Normal, light.dir); // n_dot_l (light.ndotl jest saturowany, wiec tutaj sie nie nadaje)
				float tmpdiff=diff+0.2;
				diffBack = tmpdiff<0 ? tmpdiff*RTP_BackLightStrength : 0;
				diff = saturate(diff);
			#else
				diff = light.ndotl; // bez complementary i backlight mozemy wziaść gotową wartość ndotl
			#endif
		} else {
			diff = light.ndotl;
		}
				
		float n_dot_l=diff;
		float n_dot_h = saturate(dot (s.Normal, h));
		float h_dot_l = dot (h, light.dir);
		// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
		float specular_power=exp2(10*s.Specular+1) - 1.75;
		float normalisation_term = specular_power / 16.0f; // /8.0f in equations, but we multiply (atten * 2) in lighting below
		float blinn_phong = pow( n_dot_h, specular_power );    // n_dot_h is the saturated dot product of the normal and half vectors
		float specular_term = normalisation_term * blinn_phong;
		#ifdef NO_SPECULARITY
		specular_term=0;
		s.SpecColor=0;
		#endif
		#ifdef RTP_PBL_FRESNEL
			// fresnel
			//float exponential = pow( 1.0f - h_dot_l, 5.0f ); // Schlick's approx to fresnel
			// below pow 4 looks OK and is cheaper than pow() call
			float exponential = exp2(SchlickFresnelApproxExp2Const*h_dot_l);
			// skyshop fit (I'd like people to get similar results in gamma / linear)
			#if defined(RTP_COLORSPACE_LINEAR)
				exponential=0.01+0.99*exponential;
			#else
				exponential=0.25+0.75*exponential;
			#endif
			float pbl_fresnel_term = lerp (1.0f, exponential,  s.RTP.x); // o.RTP.x - _Fresnel
		#endif
			
		#ifdef RTP_PBL_VISIBILITY_FUNCTION
			// visibility
			float n_dot_v = saturate(dot (s.Normal, viewDir));
			float alpha = 1.0f / ( sqrt( 3.1415/4 * specular_power + 3.1415/2 ) );
			float pbl_visibility_term = ( n_dot_l * ( 1.0f - alpha ) + alpha ) * ( n_dot_v * ( 1.0f - alpha ) + alpha ); // Both dot products should be saturated
			pbl_visibility_term = 1.0f / pbl_visibility_term;	
		#endif
		
		float spec = specular_term * pbl_fresnel_term * pbl_visibility_term * diff;
		
		fixed4 c;
		c.rgb = 0;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		#ifdef RTP_COLORSPACE_LINEAR
			c.rgb += (s.Albedo * light.color * diff + light.color * spec * s.SpecColor.rgb) + rtp_customAmbientCorrection*0.5;
		#else
			// shape ^2 spec golor (to fit IBL and not overbright spot for high glossy)
			c.rgb += (s.Albedo * light.color * diff + light.color * spec * s.SpecColor.rgb * s.SpecColor.rgb) + rtp_customAmbientCorrection*0.5;
		#endif
		c.a = s.Alpha;

		if (main_light) { // (uwaga - to jest static branch (main_light jest stałą)
		#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
			//		
			// reflex lights (U5 - obracane w worldSpace)
			//
			float3 normForDiffuse=s.Normal; // s.Normal jest w worldSpace w U5 - nie mozemy tego interpolowac do (0,0,1)
			float3 normForSpec=s.Normal; // j.w.
			//normForSpec=normalize(normForSpec);
			float sGloss=saturate(dot(s.SpecColor,0.3))+1;
			diffBack=saturate(1+diffBack);
			float glossDiff1=sGloss*RTP_ReflexLightDiffuseColor1.a*diffBack;
			float glossDiff2=sGloss*RTP_ReflexLightDiffuseColor2.a*diffBack;
				
			// specularity from the opposite view direction
			#if defined (RTP_SPEC_COMPLEMENTARY_LIGHTS)
				viewDir.xz=-viewDir.xz;
				h = normalize ( light.dir + viewDir );
				float nh = abs(dot (normForSpec, h));
				specular_power=RTP_ReflexLightSpecularity;
				normalisation_term = ( specular_power - 1.75f ) / 8.0f;
				blinn_phong = pow( nh, specular_power );
				specular_term = normalisation_term * blinn_phong;		
				c.rgb += light.color * RTP_ReflexLightSpecColor.rgb * specular_term * s.SpecColor * RTP_ReflexLightSpecColor.a;
			#endif
			
			half3 lightDir=light.dir;
			//lightDir.y=-0.7; // 45 degrees
			//lightDir=normalize(lightDir);
			
			float3 lightDirRefl;
			float3 refl_rot;
			refl_rot.x=0.86602540378443864676372317075294;// = sin(+120deg);
			refl_rot.y=-0.5; // = cos(+/-120deg);
			refl_rot.z=-refl_rot.x;
			
			// 1st reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yz);
			lightDirRefl.y=lightDir.y;
			lightDirRefl.z=dot(lightDir.xz, refl_rot.xy);	
			diff = abs( dot (normForDiffuse, lightDirRefl) )*glossDiff1; 
			float3 reflexRGB=RTP_ReflexLightDiffuseColor1.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;
			
			// 2nd reflex
			lightDirRefl.x=dot(lightDir.xz, refl_rot.yx);
			lightDirRefl.z=dot(lightDir.xz, refl_rot.zy);	
			diff = abs ( dot (normForDiffuse, lightDirRefl) )*glossDiff2;
			reflexRGB=RTP_ReflexLightDiffuseColor2.rgb * diff * diff;
			c.rgb += s.Albedo * reflexRGB;				
		#endif
		}

		return c;
	}		
	
	// function called in surface shader
	inline fixed4 LightingCustomBlinnPhong (RTPSurfaceOutput s, half3 viewDir, UnityGI gi)
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
		#ifdef UNITY_PASS_FORWARDADD
			// gi.light.color juz ma zaaplikowane atten (gi.light.color=_LightColor0.rgb*atten), wiec tutaj musimy to sprogować dla forward add
			gi.light.color.rgb = min(gi.light.color.rgb, _LightColor0.rgb*s.RTP.y);
		#endif
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
	
	inline half4 LightingCustomBlinnPhong_Deferred (RTPSurfaceOutput s, half3 viewDir, UnityGI gi, out half4 outDiffuseOcclusion, out half4 outSpecSmoothness, out half4 outNormal)
	{
		outDiffuseOcclusion = half4(s.Albedo, 1); // TODO should we put occlusion here (GAE) ? but it's not implemented Unity side (U5rc2) yet
		outSpecSmoothness = half4(s.SpecColor, s.Specular);
		outNormal = half4(s.Normal * 0.5 + 0.5, s.RTP.y); // tip: for s.RTP.y we could redefine deferred lighting pass using some of unused channels of MRTs
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
		RTPSurfaceOutput s,
		UnityGIInput data,
		inout UnityGI gi)
	{
		//inline UnityGI UnityGlobalIllumination (UnityGIInput data, half occlusion, half oneMinusRoughness, half3 normalWorld, bool reflections)
		data.atten=min(data.atten, s.RTP.y);
		// we could use here RTP.y from GAE as occlusion (now it's 1.0)
		// moze potem rozbije s.RTP na .y (self_shadow z oświetlenia direct) i .z (occlusion z GAE - mógłbym tutaj wstawić RTP.z wtedy)
		// ogólnie wniosek jest taki, że to chyba zostanie 1.0 (self-shadows i VERTEX_COLOR_AO_DAMP są "direct", a tylko GAE jest "indirect" ale w U5 używanie tego feature jest dyskusyjne...)
		// occlusion tłumi gi.indirect.diffuse i gi.indirect.specular (IBL spec)
		gi = UnityGlobalIllumination (data, 1.0, s.Specular, s.Normal, false);
	}
	
#else
	//
	// Unity 4
	//
	inline fixed4 LightingCustomBlinnPhong (in RTPSurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
	{
		half3 h = normalize (lightDir + viewDir);
		
		fixed diff = dot (s.Normal, lightDir); // n_dot_l
		#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
			float tmpdiff=diff+0.2;
			float diffBack = tmpdiff<0 ? tmpdiff*RTP_BackLightStrength : 0;
		#endif
		diff = saturate(diff);
		
		float n_dot_l=diff;
		float n_dot_h = saturate(dot (s.Normal, h));
		float h_dot_l = dot (h, lightDir);
		// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
		float specular_power=exp2(10*s.Specular+1) - 1.75;
		float normalisation_term = specular_power / 16.0f; // /8.0f in equations, but we multiply (atten * 2) in lighting below
		float blinn_phong = pow( n_dot_h, specular_power );    // n_dot_h is the saturated dot product of the normal and half vectors
		float specular_term = normalisation_term * blinn_phong;
		#ifdef NO_SPECULARITY
		specular_term=0;
		s.SpecColor=0;
		#endif
		#ifdef RTP_PBL_FRESNEL
			// fresnel
			//float exponential = pow( 1.0f - h_dot_l, 5.0f ); // Schlick's approx to fresnel
			// below pow 4 looks OK and is cheaper than pow() call
			float exponential = exp2(SchlickFresnelApproxExp2Const*h_dot_l);
			// skyshop fit (I'd like people to get similar results in gamma / linear)
			#if defined(RTP_COLORSPACE_LINEAR)
				exponential=0.01+0.99*exponential;
			#else
				exponential=0.25+0.75*exponential;
			#endif
			float pbl_fresnel_term = lerp (1.0f, exponential,  s.RTP.x); // o.RTP.x - _Fresnel
		#endif
			
		#ifdef RTP_PBL_VISIBILITY_FUNCTION
			// visibility
			float n_dot_v = saturate(dot (s.Normal, viewDir));
			float alpha = 1.0f / ( sqrt( 3.1415/4 * specular_power + 3.1415/2 ) );
			float pbl_visibility_term = ( n_dot_l * ( 1.0f - alpha ) + alpha ) * ( n_dot_v * ( 1.0f - alpha ) + alpha ); // Both dot products should be saturated
			pbl_visibility_term = 1.0f / pbl_visibility_term;	
		#endif
		
		float spec = specular_term * pbl_fresnel_term * pbl_visibility_term * diff;
		
		fixed4 c;
		c.rgb = 0;
		s.Albedo.rgb*=rtp_customAmbientCorrection*2+1;
		#ifdef RTP_COLORSPACE_LINEAR
			// s.RTP.y - self - shadow atten from surf()
			c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.SpecColor.rgb) * (min(atten, s.RTP.y) * 2)  + rtp_customAmbientCorrection*0.5;
		#else
			// shape ^2 spec golor (to fit IBL and not overbright spot for high glossy)
			// s.RTP.y - self - shadow atten from surf()
			c.rgb += (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec * s.SpecColor.rgb * s.SpecColor.rgb) * (min(atten, s.RTP.y) * 2)  + rtp_customAmbientCorrection*0.5;
		#endif
		c.a = s.Alpha;// + _LightColor0.a * _SpecColor.a * spec * atten;

		#if defined (RTP_COMPLEMENTARY_LIGHTS) && defined(DIRECTIONAL) && !defined(UNITY_PASS_FORWARDADD)
			//		
			// reflex lights
			//
			float3 normForDiffuse=lerp(s.Normal, float3(0,0,1), RTP_ReflexLightDiffuseSoftness);
			float3 normForSpec=s.Normal;//lerp(s.Normal, float3(0,0,1), RTP_ReflexLightSpecSoftness);
			//normForSpec=normalize(normForSpec);
			float sGloss=saturate(dot(s.SpecColor,0.3));
			float glossDiff1=(sGloss+1)*RTP_ReflexLightDiffuseColor1.a*saturate(1+diffBack);
			float glossDiff2=(sGloss+1)*RTP_ReflexLightDiffuseColor2.a*saturate(1+diffBack);
				
			// specularity from the opposite view direction
			#if defined (RTP_SPEC_COMPLEMENTARY_LIGHTS)
				viewDir.xy=-viewDir.xy;
				h = normalize ( lightDir + viewDir );
				float nh = abs(dot (normForSpec, h));
				specular_power=RTP_ReflexLightSpecularity;
				normalisation_term = ( specular_power - 1.75f ) / 8.0f;
				blinn_phong = pow( nh, specular_power );
				specular_term = normalisation_term * blinn_phong;		
				c.rgb += _LightColor0.rgb * RTP_ReflexLightSpecColor.rgb * specular_term * s.SpecColor * RTP_ReflexLightSpecColor.a;
			#endif
			
			fixed3 complColor=lerp(RTP_ReflexLightDiffuseColor1.rgb*glossDiff1, RTP_ReflexLightDiffuseColor2.rgb*glossDiff2, dot(normForDiffuse.xy, lightDir.xy)*0.5+0.5);
			c.rgb += s.Albedo * complColor * (abs(normForDiffuse.z)*0.6+0.4)*(-lightDir.z*0.3+0.7);
		#endif

		return c;
	}	
	
	inline half4 LightingCustomBlinnPhong_DirLightmap (RTPSurfaceOutput s, fixed4 color, fixed4 scale, half3 viewDir, bool surfFuncWritesNormal, out half3 specColor)
	{
		UNITY_DIRBASIS
		half3 scalePerBasisVector;
		
		color.rgb*=rtp_customAmbientCorrection*2+1;
		
		half3 lm = DirLightmapDiffuse (unity_DirBasis, color, scale, s.Normal, surfFuncWritesNormal, scalePerBasisVector) + rtp_customAmbientCorrection*0.5;
		half3 lightDir = normalize (scalePerBasisVector.x * unity_DirBasis[0] + scalePerBasisVector.y * unity_DirBasis[1] + scalePerBasisVector.z * unity_DirBasis[2]);

		// PBL	
		half3 h = normalize (lightDir + viewDir);
		fixed diff = dot (s.Normal, lightDir); // n_dot_l
		diff = saturate(diff);
		
		float n_dot_l=diff;
		float n_dot_h = saturate(dot (s.Normal, h));
		float h_dot_l = dot (h, lightDir);
		// hacking spec normalisation to get quiet a dark spec for max roughness (will be 0.25/16)
		float specular_power=exp2(10*s.Specular+1) - 1.75;
		float normalisation_term = specular_power / 16.0f; // /8.0f in equations, but we multiply (atten * 2) in lighting below
		float blinn_phong = pow( n_dot_h, specular_power );    // n_dot_h is the saturated dot product of the normal and half vectors
		float specular_term = normalisation_term * blinn_phong;
		#ifdef NO_SPECULARITY
		specular_term=0;
		s.SpecColor=0;
		#endif	
		#ifdef RTP_PBL_FRESNEL
			// fresnel
			float exponential = exp2(SchlickFresnelApproxExp2Const*h_dot_l);
			// skyshop fit (I'd like people to get similar results in gamma / linear)
			#if defined(RTP_COLORSPACE_LINEAR)
				exponential=0.01+0.99*exponential;
			#else
				exponential=0.25+0.75*exponential;
			#endif
			float pbl_fresnel_term = lerp (1.0f, exponential,  s.RTP.x); // o.RTP.x - _Fresnel
		#endif
			
		#ifdef RTP_PBL_VISIBILITY_FUNCTION
			// visibility
			float n_dot_v = saturate(dot (s.Normal, viewDir));
			float alpha = 1.0f / ( sqrt( 3.1415/4 * specular_power + 3.1415/2 ) );
			float pbl_visibility_term = ( n_dot_l * ( 1.0f - alpha ) + alpha ) * ( n_dot_v * ( 1.0f - alpha ) + alpha ); // both dot products should be saturated
			pbl_visibility_term = 1.0f / pbl_visibility_term;	
		#endif
		
		float spec = specular_term * diff * pbl_fresnel_term * pbl_visibility_term;
		
		// specColor used outside in the forward path, compiled out in prepass
		#ifdef RTP_COLORSPACE_LINEAR
			specColor = lm * s.SpecColor.rgb * spec;
		#else
			// shape ^2 spec golor (to fit IBL and not overbright spot for high glossy)
			specColor = lm * s.SpecColor.rgb * s.SpecColor.rgb * spec;
		#endif

		#if defined(RTP_DEFERRED_PBL_NORMALISATION)
			// spec from the alpha component is used to calculate specular
			// in the Lighting*_Prepass function, it's not used in forward
			return half4(lm, spec)*s.RTP.y; // s.RTP.y - self - shadow atten from surf()
		#else
			// (part taken from Lux)
			// spec from the alpha component is used to calculate specular
			// in the Lighting*_Prepass function, it's not used in forward
			// we have to compress spec like we do in the "Intrenal-PrepassLighting" shader
			return half4(lm, log2(spec + 1))*s.RTP.y; // s.RTP.y - self - shadow atten from surf()
		#endif
	}

#endif

//
// EOF lighting & misc functions
//
///////////////////////////////////////	