Shader "Custom/Heatmap" {

	Properties {
		_MainTex ("Color (RGB) Alpha (A)", 2D) = "white"
		_Color1("Vector 1", Vector) = (1,1,1,1)
		_Color2("Vector 2", Vector) = (1,1,1,1)
		_Point1("Point 1", Vector) = (1,1,1,1)
		_Point2("Point 2", Vector) = (1,1,1,1)
		_Position1("Position 1", Vector) = (0,0,0,0)
		_Orientation1("Orientation 1", Vector) = (0,1,1,1)
	}

	SubShader {
	Tags { "RenderType"="Transparent" "Queue"="Transparent"}
	ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha
    Pass{
        cull off
        CGPROGRAM
        #pragma fragment frag alpha
        #pragma vertex vert
	 	#include "UnityCG.cginc"
	 	//MainTex needs to be renamed because it is used by default by default texture shader -- old note
	 	sampler2D _MainTex; // Default Unity Texture --- This is for information
        
        // Two colors
		float4 _Color1;
		float4 _Color2;
		
		// two points
		float4 _Point1;
		float4 _Point2;
		
		// player position
		float4 _Position1;
		
		// player direction
		float2 _Orientation1;
				
		// This is needed to make 2D texture working and TRANSFORM_TEX
//		float4 _MainTex_ST;

        
        // a struct for passing information to the fragment shader. 
        struct v2f {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };
        
        struct lline {
        	float m;
        	float yint;
        };

		// 2d cartesion distance squared
        float dist (float2 a,float2 b)
        {
        	return (a.x-b.x)*(a.x-b.x) + (a.y-b.y)*(a.y-b.y);
        }
        
        float get_slope(float2 a,float2 b)
        {
        	return (a.y-b.y)/(a.x-b.x);
        }
        
        
        float get_yint(float2 a,float2 b)
        {
        	return (a.x*b.y - b.x*a.y)/(a.x-b.x);
        }
        
        lline get_line(float2 a,float2 b)
        {
        	lline l;
        	l.m = get_slope(a,b);
        	l.yint = get_yint(a,b);
        	
        	return l;
        }
        
        bool test_point(lline l,float2 a)
        {
        	float y0 = a.x * l.m + l.yint;
        	return (y0 < a.y);
        }
        
        float dist_point_to_line(float2 p,float2 a,float2 b)
        {
        	float dx = b.x-a.x;
        	float dy = b.y-a.y;
        	
        	float d = abs( dy*p.x - dx*p.y - a.x*b.y + b.x*a.y ) / sqrt(dx*dx + dy*dy);
        	return d;
        }
        
                        
        // appdata_base contains vertexs, texcoords, and anything generic for a vertex shader
        v2f vert (appdata_base v)
        {
			v2f o;
			
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
            o.uv = v.texcoord;
            
            return o;
    	}
 
    	
		// This works! 
		float4 frag (v2f IN) : COLOR
		{
			// parsing the 2d texture
			float4 e = tex2D(_MainTex, IN.uv);
			
			float d = dist(IN.uv,_Position1.xy);
			float4 f = float4(0,0,0,1);			// default to black
			
			float2 vect = _Position1.xy + 0.05*_Orientation1;
			float d2 = dist(IN.uv,vect);
			
			// lerp between the two colors
			if( e.a > 0 )
				f = lerp(_Color1,_Color2,e.x);
        	else
        		f.a = 0;
			
			if( d < 0.0005 || d2 < 0.0001 )
			{
				// invert color if near player position
				f.rgb = float3(1.0-f.r,1.0-f.g,1.0-f.b);
				f.a = 1.0;
			}
			
			if( _Point1.x >= 0 )
			{
				float2 top;
				float2 left;
				float2 right;
				
				top = _Point1 + float2(0,1)*0.05;
				left = _Point1 + float2(-0.866,-0.5)*0.05;
				right = _Point1 + float2(0.866,-0.5)*0.05;
				
				lline lright;
				lline lleft;
				lline lbottom;
				
				lright = get_line(top,right);
				lleft = get_line(top,left);
				lbottom = get_line(left,right);
				
				if( test_point(lbottom,IN.uv) && !test_point(lright,IN.uv) && !test_point(lleft,IN.uv) )
				{
					f.rgb = float3(1.0-f.r,1.0-f.g,1.0-f.b);
					f.a = 1.0;
				}
			}
			
			if( _Point2.x >= 0 )
			{
				float2 top;
				float2 left;
				float2 right;
				
				top = _Point2 + float2(0,1)*0.05;
				left = _Point2 + float2(-0.866,-0.5)*0.05;
				right = _Point2 + float2(0.866,-0.5)*0.05;
				
				lline lright;
				lline lleft;
				lline lbottom;
				
				lright = get_line(top,right);
				lleft = get_line(top,left);
				lbottom = get_line(left,right);
				
				if( test_point(lbottom,IN.uv) && !test_point(lright,IN.uv) && !test_point(lleft,IN.uv) )
				{
					f.rgb = float3(1.0-f.r,1.0-f.g,1.0-f.b);
					f.a = 1.0;
				}
			}
			
			if( _Point2.x >= 0 && _Point1.x >= 0 )
			{
				float dpline = dist_point_to_line(IN.uv,_Point1,_Point2);
				float dp1 = dist(IN.uv,_Point1);
				float dp2 = dist(IN.uv,_Point2);
				float d12 = dist(_Point1,_Point2);
			
				if( dpline < 0.0075 )
				{
					if( (dp1 < d12) && (dp2 < d12) )
					{
						f.rgb = float3(1.0-f.r,1.0-f.g,1.0-f.b);
						f.a = 1.0;				
					}
				}
			}
			
			return f;
		}
        ENDCG
        
    }
	}
}
