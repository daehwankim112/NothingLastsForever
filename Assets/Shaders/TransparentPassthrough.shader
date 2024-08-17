Shader "Custom/TransparentPassthrough"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM

            #pragma multi_compile _ HARD_OCCLUSION SOFT_OCCLUSION

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Packages/com.meta.xr.depthapi/Runtime/BiRP/EnvironmentOcclusionBiRP.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 ScreenPos : TEXCOORD0;

                META_DEPTH_VERTEX_OUTPUT(0)

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                sampler2D _CameraDepthTexture;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.ScreenPos = ComputeScreenPos(output.positionCS);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 ScreenspaceUV = input.ScreenPos.xy / input.ScreenPos.w;
                fixed4 finalColor = tex2D(_CameraDepthTexture, ScreenspaceUV);
                
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, 0);

                return finalColor;
            }
            ENDCG
        }
    }
}
