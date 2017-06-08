Shader "SCM/UI/texture_vColor"
{
	Properties
	{
		_Color ("Main Color (RGB)", Color) = (1.0, 1.0, 1.0, 1)
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
				ConstantColor [_Color]
				Combine constant  * primary//,constant 
			}
		}
	}
}