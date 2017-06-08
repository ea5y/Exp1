Shader "Custom/UIDim (SoftClip)"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Distance("Distance", Float) = 0.015
		_Color("Tint", Color) = (1,1,1,1)
	}

		SubShader
		{
			Tags
			{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			}

			Pass
			{
				Cull Off
			    Lighting Off
			    ZWrite Off
			    Offset -1, -1
				Fog
				{
					Mode Off
				}
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float2 _ClipSharpness = float2(20.0, 20.0);

				struct Input
				{
					float4 vertex   : POSITION;
					float2 uv : TEXCOORD0;
					float4 color : COLOR;
				};

				struct Output
				{
					float4 vertex   : POSITION;
					float2 uv  : TEXCOORD0;
					fixed4 color : COLOR;
					float2 worldPos : TEXCOORD1;
				};

				sampler2D _MainTex;
				float _Distance;
				float4 _MainTex_ST;
				fixed4 _Color;
				Output vert(Input i)
				{
					Output o;
					o.vertex = mul(UNITY_MATRIX_MVP, i.vertex);
					o.uv = i.uv;
					o.color = i.color * _Color;
					o.worldPos = TRANSFORM_TEX(i.vertex.xy, _MainTex);
					return o;
				}

				fixed4 frag(Output o) : SV_Target
				{
					float2 factor = (float2(1.0, 1.0) - abs(o.worldPos)) * _ClipSharpness;

					float distance = _Distance;
					fixed4 color = tex2D(_MainTex, o.uv) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x + distance, o.uv.y + distance)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x + distance, o.uv.y)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x, o.uv.y + distance)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x - distance, o.uv.y - distance)) * o.color;

					color += tex2D(_MainTex, half2(o.uv.x + distance, o.uv.y - distance)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x, o.uv.y - distance)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x - distance, o.uv.y)) * o.color;
					color += tex2D(_MainTex, half2(o.uv.x - distance, o.uv.y + distance)) * o.color;

					color = color / 9;
					//color.a = 0.5;
					color.a *= clamp(min(factor.x, factor.y), 0.0, 1.0);
					return color;
				}
				ENDCG
			}
		}
}