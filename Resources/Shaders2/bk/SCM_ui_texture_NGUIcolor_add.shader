Shader "SCM/UI/NGUIcolor_add"
{
	Properties
	{
		//_Color ("Main Color (RGB)", Color) = (1.0, 1.0, 1.0, 1)
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
		
	}
	
	SubShader
	{
		LOD 80

		Tags
		{
			"Queue"="Transparent+1" 
		}
		
		Pass
		{
			Cull Back
			Lighting Off
			ZWrite Off
			ZTest Always 
			
			Blend SrcAlpha One 
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				//ConstantColor [_Color]
				Combine Texture * primary
			}
		}
	}
}