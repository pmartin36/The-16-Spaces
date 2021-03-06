﻿Shader "SlidingTiles/BasicTileTintAndTex"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_ImmobileTex("Immobile Texture", 2D) = "white" {}

		_MobileColor("Color", Color) = (1,1,1,1)
		_ImmobileColor("Immobile Color", Color) = (1,1,1,1)

		_Mobile("Mobile", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent" 
			"Queue"="Transparent-100"
		}

        Pass
        {
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off

            CGPROGRAM	
            #pragma vertex vert
            #pragma fragment frag
			
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float4 color: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _ImmobileTex;
            float4 _MainTex_ST;
			float4 _MobileColor;
			float4 _ImmobileColor;
			float _Mobile;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color * lerp(_ImmobileColor, _MobileColor, _Mobile);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {		
                fixed4 col = lerp(
					tex2D(_ImmobileTex, i.uv),
					tex2D(_MainTex, i.uv),
					_Mobile
				) * i.color;
				return col;
            }
            ENDCG
        }
    }
}
