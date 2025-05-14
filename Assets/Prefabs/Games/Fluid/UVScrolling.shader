Shader "Custom/UVScrolling"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" { }
        _ScrollSpeed ("Scroll Speed", Vector) = (0.5, 0, 0, 0) // X = horizontal speed, Y = vertical speed
        _Direction ("Direction", Vector) = (1, 0, 0, 0) // X = horizontal, Y = vertical (1 or -1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ScrollSpeed;
            float4 _Direction;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 ApplyUVScroll(float2 uv)
            {
                uv += _Time.y * _ScrollSpeed.xy * _Direction.xy;
                uv = frac(uv); // To make the UVs wrap around
                return uv;
            }

            half4 frag(v2f i) : SV_Target
            {
                i.uv = ApplyUVScroll(i.uv);
                half4 col = tex2D(_MainTex, i.uv);
                return col;
            }

            ENDCG
        }
    }
    Fallback "Diffuse"
}
