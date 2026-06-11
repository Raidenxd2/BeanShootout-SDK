Shader "SharedShaders/MobileBasic"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _BumpMap("Normal Map", 2D) = "bump" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            #pragma shader_feature_local _NORMALMAP
            
            #pragma multi_compile _ LIGHTMAP_ON

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 normalOS : NORMAL;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : NORMAL;
                float2 uv0 : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(uv1, vertexSH, 1);
                float2 uv2 : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap);
#if defined(_NORMALMAP)
            TEXTURE2D(_BumpMap);
#endif
            SAMPLER(sampler_BaseMap);
#if defined(_NORMALMAP)
            SAMPLER(sampler_BumpMap);
#endif

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                float4 _BumpMap_ST;
            CBUFFER_END
            
            void InitializeInputData(Varyings input, out InputData inputData)
            {
                inputData = (InputData)0;

                inputData.positionWS = float3(0, 0, 0);
                inputData.viewDirectionWS = half3(0, 0, 1);

                inputData.normalWS = input.normalWS;
                
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionHCS);
            }
            
            void InitializeBakedGIData(Varyings input, inout InputData inputData)
            {
                inputData.bakedGI = SAMPLE_GI(input.uv1, input.vertexSH, inputData.normalWS);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv0 = TRANSFORM_TEX(IN.uv0, _BaseMap);
#if defined(_NORMALMAP)
                OUT.uv2 = TRANSFORM_TEX(IN.uv0, _BumpMap);
#endif
                
                OUTPUT_LIGHTMAP_UV(IN.uv1, unity_LightmapST, OUT.uv1);
                
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
#if defined(_NORMALMAP)
                half4 nrm = SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, IN.uv2);
                nrm.rgb = UnpackNormal(nrm) * 0.05;
                
                float nrmCol = nrm.r + nrm.g;
#endif
                
                InputData inputData;
                InitializeInputData(IN, inputData);
                
                InitializeBakedGIData(IN, inputData);
                
#if defined(_NORMALMAP)
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv0) + half4(nrmCol, nrmCol, nrmCol, 1);
                color = color * half4(inputData.bakedGI.r, inputData.bakedGI.g, inputData.bakedGI.b, 1) * _BaseColor;
#else
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv0) * half4(inputData.bakedGI.r, inputData.bakedGI.g, inputData.bakedGI.b, 1) * _BaseColor;
#endif
                
                return color;
            }
            ENDHLSL
        }
    }

    CustomEditor "MobileBasicGUI"
}