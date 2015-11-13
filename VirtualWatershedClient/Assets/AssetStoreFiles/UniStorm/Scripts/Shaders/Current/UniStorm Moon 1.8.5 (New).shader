Shader "UniStorm/Moon (1.8.5 (New))"
{
   Properties
   {
      _MainTex ("Moon Texture (RGB) Alpha (A)", 2D) = "white"
      
      //_Mask ("Culling Mask", 2D) = "white" {}
      
      //_StarMask ("Star Mask", 2D) = "white" {}
      _StarMask ("Dark Side of Moon Texture (RGBA) ", 2D) = "white" {}
      _MoonColor ("Dark Side of Moon Color", Color) = (0,0,0,0) 
      _FogColor2 ("Fog Color", Color) = (1,1,1,1) 
      //_NewMoonColor ("Moon Face Color", Color) = (1,1,1,1)
      //_Cutoff ("Alpha cutoff", Range (0,1)) = 0.1
      _FloatMin ("FloatMin", Range (0,1)) = 0
      _FloatMax ("FloatMax", Range (0,1)) = 0
      //_FogAmountMin ("Fog Blend Amount Min", Range (-50000,0)) = 0
      //_FogAmountMax ("Fog Blend Amount Max", Range (0,50000)) = 0
      _FogAmountMin ("Fog Blend Amount Min", Range (-50000,0)) = 0
      _FogAmountMax ("Fog Blend Amount Max", Range (0,50000)) = 0
   }
   SubShader
   {
      Tags {"Queue"="Transparent-400"}
      
      Lighting Off
      Cull Off
      //Fog { Mode off }
      Blend OneMinusDstColor One // Hides black not stars
      ZWrite Off
      //Blend One One
      //Blend SrcAlpha OneMinusSrcAlpha // Best so far
      //Blend OneMinusDstColor Zero
      //AlphaTest GEqual [_Cutoff]
      //ColorMask RGB
      //ZTest Greater
      
    

      
      Pass {
			Tags { "Queue"="Transparent-425"}
			ColorMaterial AmbientAndDiffuse
			Blend SrcAlpha OneMinusSrcAlpha
			//Fog  { Color [_FogColor2]} 
			//ColorMask RGBA
			//Fog  { Density .0001 } 
			//Fog  { Range [_FogAmountMin] , [_FogAmountMax]}
			Fog  { Range [_FogAmountMin] , [_FogAmountMax]}
			
			
            SetTexture [_StarMask] {

                constantColor [_MoonColor]

                combine constant * primary

            }

            SetTexture [_StarMask] {

                combine texture * previous DOUBLE

            }

        }
      
      
      Pass
      {
      	//Tags {"Queue"="Transparent-400"}
         //Blend OneMinusDstColor One // Soft Additive
         //Blend OneMinusDstColor DstAlpha
      	 //AlphaTest LEqual [_Cutoff]
      	  Fog { Mode off }
      	
		 //AlphaTest GEqual [_Cutoff]
	        	 
         SetTexture [_Mask] {combine texture}
         SetTexture [_MainTex] {combine texture, previous}
         

      }
      
	
	
      
   
   }
}