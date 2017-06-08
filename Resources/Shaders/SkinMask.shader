Shader "Custom/Mask" {
	Properties
	{
		_MainTex("Albedo", 2D) = "white" {}
		_MaskTex("Mask", 2D) = "white" {}

		[Enum(Translation, 0, Scale, 1, Rotation, 2)] _Transform("Transform", float) = 0
		_Translation("Translation", Vector) = (0.0, 0.0, 0.0, 0.0)
		_Scale("Scale", Vector) = (0.0, 0.0, 0.0, 0.0)
	}

	SubShader
	{
		Pass
		{
			//Blend One OneMinusSrcAlpha
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			uniform float4 _Translation;
			uniform float4 _Scale;

			sampler2D _MainTex;
			sampler2D _MaskTex;
			float4 _MaskTex_ST;

			struct Input
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Output {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

				float2 uvT : TEXCOORD1;
			};

			float4x4 Translational(float4 translation)
			{
				return float4x4(1.0, 0.0, 0.0, translation.x,
								0.0, 1.0, 0.0, translation.y,
								0.0, 0.0, 1.0, translation.z,
								0.0, 0.0, 0.0, 1.0);
			}

			float4x4 Scale(float4 scale)
			{
				return float4x4(scale.x, 0.0, 0.0, 0.0,
								0.0, scale.y, 0.0, 0.0,
								0.0, 0.0, scale.z, 0.0,
								0.0, 0.0, 0.0, 1.0);
			}

			Output vert(Input i)
			{
				Output o;

				//o.uvT = mul(Scale(float4(0.5,0.5,0.5,1)), float4(i.uv, 1, 1));
				
				o.uvT = mul(Translational(_Translation), float4(i.uv, 1, 1));
				//o.uvT = i.uv * 0.7;
				//o.uvT = i.uv * 0.7;
				o.uvT = mul(Scale(_Scale), float4(o.uvT, 1, 1));
				o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
				
				//i.uv = mul(Scale(float4(2, 2, 2, 1)), float4(i.uv, 1, 1));
				o.uv = TRANSFORM_TEX(i.uv, _MaskTex);

				return o;
			} 

			fixed4 frag(Output o) : SV_Target
			{
				fixed4 c = 0;
				fixed4 temp = tex2D(_MainTex, o.uvT);
				c.rgb = (1 - tex2D(_MaskTex, o.uv).a) * temp.rgb * temp.a;

				c.a = (1 - tex2D(_MaskTex, o.uv).a) * temp.a;
				return c;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
