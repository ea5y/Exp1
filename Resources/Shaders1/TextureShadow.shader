Shader "Custom/TextureShadow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
        LOD 100
        Tags
        {
            "Queue" = "Transparent-1"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
        }


        Cull Off
        Lighting Off
        ZWrite Off
        Fog {Mode Off}
        Blend SrcAlpha OneMinusSrcAlpha
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Output
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			Output vert (Input i)
			{
				Output o;
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				o.uv = TRANSFORM_TEX(i.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (Output o) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, o.uv);
                col.rgb = (1-ceil(col.a));
				return col;
			}
			ENDCG
		}
	}
    FallBack "Diffuse"
}
