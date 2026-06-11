Shader "BeanShootout/TVShader"
{
    Properties
    {
        _BluelightColor("Bluelight Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
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
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            struct Attributes
            {
                half4 positionOS   : POSITION;
                half2 uv           : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                half4 positionHCS  : SV_POSITION;
                half2 uv           : TEXCOORD0;
                
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseMap_ST;
                half4 _BluelightColor;
            CBUFFER_END

            void Unity_ReplaceColor_half(half3 In, half3 From, half3 To, half Range, out half3 Out)
            {
                half Distance = distance(From, In);
                Out = lerp(To, In, saturate(Distance - Range));
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                ZERO_INITIALIZE(Varyings, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 ReplaceColorOut;
                Unity_ReplaceColor_half(color.xyz, FastSRGBToLinear(half3(0.6415094, 0.3056248, 0.3056248)), _BluelightColor, half(0.002), ReplaceColorOut);
                return half4(ReplaceColorOut.x, ReplaceColorOut.y, ReplaceColorOut.z, half(1));
            }
            ENDHLSL
        }
    }
}
