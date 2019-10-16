﻿Shader "SlidingTiles/SpawnIndicator"
{
    Properties
    {
		_Radius("Radius", Range(0,2)) = 0
    }
    SubShader
    {
        Tags { 
			"RenderType"="Transparent" 
			"Queue"="Transparent"
		}

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

			float inverseLerp(float a, float b, float v) {
				return (v - a) / (b - a);
			}

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
				float4 color: COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float _Radius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
				o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {	
				float2 remappedUv = (i.uv * 2) - 1;
				float len = length(remappedUv);
				//float dist = pow(saturate(1 - (_Radius - len)),3) - step(min(_Radius, 0.92), len);
				float dist = pow(saturate(1 - abs(_Radius - len)), 5) - max(len - 0.875, 0) * 15;
				dist = saturate(dist);
				float4 col = i.color;
				col.a *= dist;
				return col;
            }
            ENDCG
        }
    }
}