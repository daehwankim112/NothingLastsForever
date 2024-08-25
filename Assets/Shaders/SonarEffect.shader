Shader "Custom/SonarEffect"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _MaxWaveDistance ("Max Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _Color ("Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        // Ensure proper blending against transparent background (passthrough)
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
                float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 worldPos : TEXCOORD1;

                META_DEPTH_VERTEX_OUTPUT(0)

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _MaxWaveDistance;
                float _WaveDistance;
                float _Threshold;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;

                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = UnityObjectToClipPos(input.vertex);
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);

                return output;
            }

            // Easing function for the wave length
            float ease(float x)
            {
                return x * x * (3.0 - 2.0 * x);
            }

            // Check the distance between depth texture and wave distance
            float waveAlpha(float depth, float waveDist, float threshold, float maxWaveDistance)
            {
                float dist = abs(depth - waveDist);
                // float t = saturate(dist / (threshold * ((maxWaveDistance - waveDist) / maxWaveDistance)));
                float t = saturate(dist / threshold);
                return ease(t) + (waveDist / maxWaveDistance);
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                half4 finalColor = _Color;

                float4 depthSpace = mul(_EnvironmentDepthReprojectionMatrices[unity_StereoEyeIndex], float4(input.worldPos.xyz, 1.0));
                float2 uvCoords = (depthSpace.xy / depthSpace.w + 1.0f) * 0.5f;
                float linearSceneDepth = SampleEnvironmentDepthLinear_Internal(uvCoords);

                float alpha = waveAlpha(linearSceneDepth, _WaveDistance, _Threshold, _MaxWaveDistance);
                // finalColor.a *= alpha;
                finalColor.a = alpha;
                // finalColor = float4(0, 1 - linearSceneDepth/1.5, 0, 0.9);

                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, 0);

                return finalColor;
            }

            ENDCG
        }
    }
}
