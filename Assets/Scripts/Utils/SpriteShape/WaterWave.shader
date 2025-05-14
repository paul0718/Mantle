Shader "Custom/WaterWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5
        _BlurStrength ("Blur Strength", Range(0, 1)) = 0.2
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _FillAmount;
            float _BlurStrength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                if (i.uv.y<_FillAmount)
                    return col;
                // �����黯���������� Fill Amount ���������黯Ч��
                float fadeFactor = 1-smoothstep(_FillAmount, _FillAmount + _BlurStrength, i.uv.y);
                
                // ����͸���ȣ��ӽ�ˮ��Ĳ��ֻ��͸����ģ���黯
                col.a *= fadeFactor;
                return col;
            }
            ENDCG
        }
    }
}
