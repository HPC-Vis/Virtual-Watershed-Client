Shader "Particles/Regular Distant Cloud Shader" {
Properties {
	_Color ("Main Color", Color) = (.2,.2,.2,0)
	_MainTex ("Particle Texture", 2D) = "white" {}
}

SubShader {
	Tags { "Queue"="Transparent-400" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Tags { "LightMode" = "Vertex" }
	Fog { Density .0001 }
	Cull Off
	Lighting On
	Material { Emission [_Color] }
	ColorMaterial AmbientAndDiffuse
	ZWrite Off
	ColorMask RGB
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .001
	Pass { 
		SetTexture [_MainTex] { combine primary * texture }
	}
}
}