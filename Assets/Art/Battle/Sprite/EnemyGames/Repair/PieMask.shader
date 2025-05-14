Shader "Unlit/PieMask"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Fill ("Fill", Range(0, 1)) = 1
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Fill;
            float4 _MainTex_ST;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - float2(0.5, 0.5);
                float angle = atan2(uv.y, uv.x);
                float dist = length(uv);
                angle = (angle + UNITY_PI) / (2 * UNITY_PI); // [0,1]
                if (angle > _Fill) discard;

                return tex2D(_MainTex, i.uv) * _Color;
            }
            ENDCG
        }
    }
}

