Shader "SCM/UI/ui_VCMT_Alpha"
{
Properties {
    	_Color ("Multiplied Color (RGBA)", Color) = (1, 1, 1, 1)
        _MainTex ("Base Texture (RGBA)", 2D) = "white" {}

    }

    SubShader {
	Tags { "Queue"="Transparent"}
        Cull Off
        ZWrite Off
        ZTest Always 
		Blend SrcAlpha OneMinusSrcAlpha
        LOD 0
        
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  			#pragma fragmentoption ARB_precision_hint_fastest		
           	#include "UnityCG.cginc"
           	

            
			struct appdata_t {
				fixed4 vertex : POSITION;
				fixed4 color : COLOR;
				fixed2 texcoord : TEXCOORD0;
			};
            struct v2f {
                fixed4 position : SV_POSITION;
                fixed4 color : COLOR;
                fixed2 texcoord : TEXCOORD0;
            };
            
            fixed4 _MainTex_ST;

            v2f vert(appdata_t v) {
                v2f o;
                o.position = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }
  
            fixed	_Intensity;
            fixed4 _Color;          
            sampler2D _MainTex;
            
            fixed4 frag(v2f i) : COLOR {
                fixed4 mc = tex2D(_MainTex, i.texcoord);
                fixed4 c;
                c.rgb = mc.rgb * i.color.rgb * _Color.rgb;
                c.a = mc.a * i.color.a * _Color.a;
                return c;
            }

            ENDCG
        }
    }
}