Shader "Panoply/URPPanelShader"
{
    Properties
    {
        _MainTex ("Albedo Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100

        ZWrite Off
        Blend Off

        Pass
        {
            HLSLPROGRAM

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            sampler2D _MainTex;
            float4 _TintColor[24];
            float4 _BackgroundColor;
            
            uniform float4 _MainTex_TexelSize;
            float _BorderSize[24];
            float4 _BorderColor[24];

            float4x4 _ProjectionMatrix[24];
            float _CameraPos[72];

            float _OffscreenLeftEdge[24];
            float _OffscreenRightEdge[24];
            float _OffscreenTopEdge[24];
            float _OffscreenBottomEdge[24];

            bool VectorNotEqual( float4 a, float4 b )
            {
                float4 diff = a-b;
                return diff != 0;
            }

            bool MatrixNotEqual( float4x4 a, float4x4 b )
            {
                bool result = false;
                for (int i=0; i<4; i++) {
                    if (VectorNotEqual(a[i], b[i])) {
                        result = true;
                    }
                }
                return result;
            }
            
            half4 frag (Varyings i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float4 originalColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, i.texcoord);

                // get the index of the currently rendering camera by comparing its
                // projection matrix and position with possible options stored in the material
                float ci = -1;
                float3 cameraPos;
                for (int j = 0; j < 24; j++) {
                    if (!MatrixNotEqual(_ProjectionMatrix[j], unity_CameraProjection)) {
                        cameraPos = float3(_CameraPos[j*3], _CameraPos[j*3+1], _CameraPos[j*3+2]);
                        bool positionsEqual = all(cameraPos == _WorldSpaceCameraPos);
                        if (positionsEqual) {
                            ci = j;
                            break;
                        }
                    }
                }

                half4 matteBlendedColor = originalColor;
                if (ci != -1) {
                    matteBlendedColor = lerp(originalColor, _TintColor[ci], _TintColor[ci][3]);
                    matteBlendedColor[3] = 1;

                    ComputeScreenPos(i.positionCS);
                    float2 screenSize = _ScreenParams.xy;
                    float2 xy = i.positionCS.xy;

                    // Calculate border conditions
                    bool isBorder = (xy.x < _BorderSize[ci] || xy.x > (screenSize.x - _BorderSize[ci]) || xy.y < _BorderSize[ci] || xy.y > (screenSize.y - _BorderSize[ci]));

                    // Blend colors based on border condition
                    half4 borderBlendedColor = lerp(matteBlendedColor, _BorderColor[ci], _BorderColor[ci][3]);
                    borderBlendedColor[3] = 1;
                    borderBlendedColor = isBorder ? borderBlendedColor : matteBlendedColor;

                    //return borderBlendedColor;

                    // letter/pillarboxing
                    bool isWithinLetterboxOrPillarbox = xy.x < _OffscreenLeftEdge[ci] || xy.x > _OffscreenRightEdge[ci] || xy.y < _OffscreenBottomEdge[ci] || xy.y > _OffscreenTopEdge[ci];
                    return isWithinLetterboxOrPillarbox ? _BackgroundColor : borderBlendedColor;
                } else {
                    return originalColor;
                }
            }
            ENDHLSL
        }
    }
}