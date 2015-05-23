Shader "Envrionment/Environment" {
	
	Properties {
		_MainTex2 ("Texture", 2D) = "white" {}
		_3DTex ("3D", 3D) = "white" {}
		_Point1("Vector 1", Vector) = (1,1,1,1)
		_Point2("Vector 2", Vector) = (1,1,1,1)
		_Max ("Min", float) = -1
		_Min ("Max", float) = -1
	}
	
    SubShader {
    Pass{
        cull off
        CGPROGRAM
        #pragma fragment frag
        #pragma vertex vert
	 	#include "UnityCG.cginc"
	 	//MainTex needs to be renamed because it is used by default by default texture shader -- old note
	 	sampler2D _MainTex2; // Your heightmap
	 	sampler2D _MainTex; // Default Unity Texture --- This is for information
        sampler3D _3DTex; // Soil or some data
        
        // Two points on the terrain
		float2 _Point1;
		float2 _Point2;
				
		// This will be later used for the depth of the data
		float _Max;
		float _Min;
		
		// This is needed to make 2D texture working and TRANSFORM_TEX
//		float4 _MainTex_ST;

        
        // a struct for passing information to the fragment shader. 
        struct v2f {
            float4 pos : SV_POSITION;
            float3 uv : TEXCOORD1;
            float2 uv2 : TEXCOORD0;
        };
        
        // appdata_base contains vertexs, texcoords, and anything generic for a vertex shader
        v2f vert (appdata_base v)
        {
			v2f o;
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
			o.uv.xz = _Point1.xy;
			if(v.texcoord.x == 1)
			{
               o.uv.xz = _Point2.xy;
            }
            o.uv.y = v.texcoord.y*(_Max-_Min) + _Min;
            o.uv2.x = v.texcoord.x;
            o.uv2.y = v.texcoord.y*(_Max-_Min) + _Min;
            o.uv2 = v.texcoord;
            return o;
    	}
    	
		// This works! 
		float4 frag (v2f IN) : COLOR
		{
			// parsing the 3D texture
			float4 e = tex3D(_3DTex, IN.uv);
						
			// Create a line between the two specified points
			float2 li = _Point1 - _Point2;
			
//			float4 s = tex2D(_MainTex2,IN.uv);
			
			// move along the one point to the other point based on the line.
			float4 b = tex2D(_MainTex2,IN.uv2.x*li+_Point2);
			
			// temporary variable -- that holds a preset color for colors the line
			float4 c = float4(0,0,.1,1);
			
			// terrain data is stored in the alpha component/anything below the height is set as the default color
			if(b.a > IN.uv2.y)
			{
				c = b;
			}
			
			// lerp between the data of the height map and 3D texture.
			float4 f = lerp(e,c,.5);
//            f = tex2D(_MainTex2,IN.uv2);
//            float region = .1;
//            if(f.a > 0)
//            f = float4(1,0,0,1);
//            else
//            f = float4(0,1,0,1);
//            if (((IN.uv2.x >= _Point1.x -region && IN.uv2.x <= _Point1.x) && (IN.uv2.y >= _Point1.y -region && IN.uv2.y <= _Point1.y)) || ((IN.uv2.x >= _Point2.x -region && IN.uv2.x <= _Point2.x) && (IN.uv2.y >= _Point2.y -region && IN.uv2.y <= _Point2.y)))
//            f = lerp(float4(0,0,1,1),f,.5);
			return f;

		}
        ENDCG
        
    } 
    }
    
}