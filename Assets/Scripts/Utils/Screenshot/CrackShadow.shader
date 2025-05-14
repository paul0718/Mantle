Shader "Custom/CrackShadow"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1, 0, 0, 1)
        _FadeWidth ("Fade Width", Range(0, 0.5)) = 0.05
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off
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
                float2 uv : TEXCOORD0;
                float3 bary: TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 bary : TEXCOORD1; 
            };

            sampler2D _MainTex;
            float4 _EdgeColor;
            float _FadeWidth;

            // Receive world-space vertices from C#
            float3 _Vertex1;
            float3 _Vertex2;
            float3 _Vertex3;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.bary = v.bary;
                return o;
            }

            // Function to compute distance from point to line segment
            float DistanceToLineSegment(float3 p, float3 v1, float3 v2)
            {
                float3 v2v1 = v2 - v1;
                float3 v2p = p - v2;
                float3 v1p = p - v1;

                float dot_v2v1 = dot(v2v1, v2v1);
                float dot_v1p = dot(v1p, v2v1);
                float dot_v2p = dot(v2p, v2v1);

                float t = dot_v1p / dot_v2v1;
                if (t < 0.0)
                    return length(v1 - p);
                else if (t > 1.0)
                    return length(v2 - p);
                else
                    return length(v1 + t * v2v1 - p);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 p = i.bary; // Get the fragment world position

                // Compute distances to the edges of the triangle
                float dist1 = DistanceToLineSegment(p, _Vertex1, _Vertex2);
                float dist2 = DistanceToLineSegment(p, _Vertex2, _Vertex3);
                float dist3 = DistanceToLineSegment(p, _Vertex3, _Vertex1);

                float minDist = min(min(dist1, dist2), dist3);
                fixed4 texColor = tex2D(_MainTex, i.uv);

                 //if (minDist < 0.02)
                   // return fixed4(0,0,0,1);
                //if (minDist<0.04)
                    //return fixed4(1,1,1,1);

                //if (minDist==dist3)
                    //return texColor;
               
                
                if(_FadeWidth<0.1)
                    return texColor;

               float edgeFactor = minDist > 0.03 ? 1.0 : 0.0;


                
                fixed4 finalColor = lerp(_EdgeColor, texColor, edgeFactor);
                finalColor.a = texColor.a;

                return finalColor;
            }
            ENDCG
        }
    }
}
