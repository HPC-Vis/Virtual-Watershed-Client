Shader "UniStorm/Non-Dynamic Light Clouds" {

Properties {

    _Color ("Cloud Color", Color) = (0.5,0.5,0.5,0.5)

    _MainTex ("Particle Texture", 2D) = "white" {}

}

 

Category {

    Tags { "Queue"="Transparent-390" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Tags { "LightMode" = "Vertex" }
    Blend SrcAlpha OneMinusSrcAlpha
	Fog  { Density .0001 } 
	Fog  { Range 0 , 8000 }
    AlphaTest Greater .01

    ColorMask RGB

    Cull Off 
    Lighting On 
    ZWrite Off
    //Material { Emission [_Color] }
	ColorMaterial AmbientAndDiffuse


    BindChannels {

        Bind "Color", color

        Bind "Vertex", vertex

        Bind "TexCoord", texcoord

    }

    

    // ---- Dual texture cards

    SubShader {

        Pass {

            SetTexture [_MainTex] {

                constantColor [_Color]

                combine constant * primary

            }

            SetTexture [_MainTex] {

                combine texture * previous DOUBLE

            }

        }

    }

    

    // ---- Single texture cards (does not do color tint)

    SubShader {

        Pass {

            SetTexture [_MainTex] {

                combine texture * primary

            }

        }

    }

}

}