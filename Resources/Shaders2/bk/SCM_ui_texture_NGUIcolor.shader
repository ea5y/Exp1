Shader "SCM/UI/texture_NGUIcolor"
{
	Properties
	{
		//_Color ("Color Tint", Color) = (1,1,1,1)   
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 80

		Tags
		{
			"Queue"="Transparent" 
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			ZTest Always 
			
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				//ConstantColor [_Color]
				Combine Texture * primary
			}
		}
	}
}