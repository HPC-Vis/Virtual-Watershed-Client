//(Currently in Beta)
Shader "UniStorm/Dynamic Snow" 
{
    Properties 
    {
    
    	_TextColor ("Main Color", Color) = (1,1,1,1)
    	_SpecColor ("Main Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _Shininess ("Main Shininess", Range (0.03, 1)) = 0.078125
    	
        _MainTex ("Main Texture", 2D) = "white" {}
        _MainBump ("Main Bump", 2D) = "bump" {}
        
        _SnowColor ("Snow Color", Color) = (1,1,1,1)
        _SnowSpecColor ("Snow Specular Color", Color) = (0.5, 0.5, 0.5, 1)
        _SnowShininess ("Snow Shininess", Range (0.03, 1)) = 0.078125
        
        _LayerTex ("Snow Texture", 2D) = "white" {}
        _LayerBump ("Snow Bump", 2D) ="bump" {}
        
        _LayerDirection ("Snow Direction", Vector) = (0, 1, 0)
        _LayerDepth ("Snow Depth", Range(0, 0.005)) = 0.0005
    }
   
    SubShader 
    {
        Tags { "RenderType"="Opaque" }
        LOD 400
       
        CGPROGRAM
        #pragma target 3.0
        #pragma surface surf BlinnPhong vertex:vert
 
        sampler2D _MainTex;
        sampler2D _MainBump;
        sampler2D _LayerTex;
        sampler2D _LayerBump;
        
        fixed4 _TextColor;
        fixed4 _SnowColor;
        fixed4 _SnowSpecColor;
        
 		half _Shininess;
 		half _SnowShininess;
        
        float _LayerStrength;
        float3 _LayerDirection;
        float _LayerDepth;
 
        struct Input {
            float2 uv_MainTex;
            float2 uv_MainBump;
            float2 uv_LayerTex;
            float2 uv_LayerBump;
            float3 worldNormal;
            INTERNAL_DATA
        };
       
        void vert (inout appdata_full v) 
        {
            float3 sn = mul((float3x3)_World2Object, _LayerDirection);
           
            if (dot(v.normal, sn.xyz) >= lerp(1, -1, (_LayerStrength * 2) / 3))
            {
                v.vertex.xyz += (sn.xyz + v.normal) * _LayerDepth * _LayerStrength;
            }
        }
 
        void surf (Input IN, inout SurfaceOutput o) 
        {

            half4 mainDiffuse = tex2D(_MainTex, IN.uv_MainTex);
            half4 layerDiffuse = tex2D(_LayerTex, IN.uv_LayerTex);

            o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));
            half3 layerNormal = half3(0, 0, 0);

            half sm = dot(WorldNormalVector(IN, o.Normal), _LayerDirection);
            sm = pow(0.5 * sm + 0.5, 2.0);

            if (sm >= lerp(1, 0, _LayerStrength))
            {
    	    	fixed4 snow = tex2D(_LayerTex, IN.uv_LayerTex);
     			o.Albedo = snow.rgb * _SnowColor.rgb;
    			o.Gloss = snow.a;
     			o.Alpha = snow.a * _SnowColor.a;
    			o.Specular = _SnowShininess;
    	   		o.Normal = UnpackNormal(tex2D(_LayerBump, IN.uv_LayerBump));
                
            }

            else
            {
                //Base
            	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
     			o.Albedo = tex.rgb * _TextColor.rgb;
    			o.Gloss = tex.a;
     			o.Alpha = tex.a * _TextColor.a;
    			o.Specular = _Shininess;
    	    	o.Normal = UnpackNormal(tex2D(_MainBump, IN.uv_MainBump));
            }
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}