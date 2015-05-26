// 
// Author: Roger Lew (rogerlew@vandals.uidaho.edu || rogerlew@gmail.com)
// Date: 1/6/2015
// License: Public Domain
//
// This is hacked to death from 
// http://forum.unity3d.com/threads/how-to-darken-a-projector-part-under-a-mesh.159475/
// http://answers.unity3d.com/questions/40357/projector-projector-shader-and-cg.html
// http://answers.unity3d.com/questions/347898/how-to-use-a-transparentcutout-shader-with-a-proje.html
//
// VERY IMPORTANT
// To avoid tiling and other problems the cookie needs to:
//   1. Have transparency along all four sides
//   2. Be imported as advanced
//   3. Have wrap mode set to clamp
//   4. Have generate mipmaps disabled

Shader "Projector/Slide" {
  Properties { 
     _ShadowTex ("Cookie", 2D) = "" { TexGen ObjectLinear }
	 _Opacity ("Opacity", Range(0,1)) = 1
  }
  Subshader
  {
    Lighting On

	BindChannels
	{
		Bind "vertex", vertex
		Bind "texcoord", texcoord
	}
	
	Pass
    {
        Blend SrcAlpha OneMinusSrcAlpha

        SetTexture [_ShadowTex] {
            constantColor (1, 1, 1, [_Opacity])
            combine texture * constant
            Matrix [_Projector]
       }
    }

    Pass
    {
        ZWrite Off
        ZTest LEqual
        Blend SrcColor One
        Offset -1, -1

        SetTexture [_ShadowTex] {
            combine previous * texture alpha
            Matrix [_Projector]
        }
     }
  }
}