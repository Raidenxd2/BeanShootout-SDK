Shader "BeanShootout/UIGradient"
{
    Properties
    {
        [HideInInspector] _MainTex("MainTex", 2D) = "white" {}
        [MainColor] _Color_A("Color A", Color) = (1, 1, 1, 1)
        _Color_B("Color B", Color) = (1, 1, 1, 1)
        _BlendHeight("BlendHeight", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "PreviewType" = "Plane"}

        Pass
        {
            Offset 1, 1
            
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _Color_A;
                float4 _Color_B;
                float _BlendHeight;
            CBUFFER_END
            
            void Unity_Rotate_Degrees_half(half2 UV, half2 Center, half Rotation, out half1 Out)
            {
                Rotation = Rotation * (3.1415926f/180.0f);
                UV -= Center;
                half s, c;
                sincos(Rotation, s, c);
                half2 r3 = half2(c, s);
                half1 r1;
                r1.x = dot(UV, r3);
                Out = r1 + Center;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                UNITY_SETUP_INSTANCE_ID(IN);
                ZERO_INITIALIZE(Varyings, OUT);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half1 newUV;
                Unity_Rotate_Degrees_half(IN.uv, half2(0.5, 0.5), 45, newUV);
                
                half1 uvm = newUV.x * _BlendHeight;
                
                half4 color = lerp(_Color_A, _Color_B, half4(uvm, uvm, uvm, 1));
                return color;
            }
            ENDHLSL
        }
    }
}
