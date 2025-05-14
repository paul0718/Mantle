Shader "Custom/DynamicColorLineShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Speed ("Movement Speed", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite on
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _Speed;
            float4 _MainTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate the time offset
                float time = _Time.y * _Speed;
                float offset = time * 0.5 + 0.5; // oscillates between 0 and 1

                // Set the UV offset
                i.uv.x += offset;

                // Sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a=1;
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}