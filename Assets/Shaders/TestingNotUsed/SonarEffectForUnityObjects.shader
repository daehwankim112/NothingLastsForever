Shader "Custom/SonarEffectForUnityObjects"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _BaseColor ("Base Color", Color) = (0, 0, 0, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }

        ZWrite On
        ZTest LEqual
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
                // float2 uv : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                // float2 uv : TEXVOORD0;
                // float4 ScreenPos : TEXCOORD0;
                float depth : TEXCOORD0;
                // float4 worldPos : TEXCOORD1;

                META_DEPTH_VERTEX_OUTPUT(0)

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            /* CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _WaveDistance;
                float _Threshold;
            CBUFFER_END */

            // ssampler2D _BaseColor;
            // half4 _BaseColor;
            // sampler2D _CameraDepthTexture;
            // float4 _MainTex_ST;

            /* CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                sampler2D _CameraDepthTexture;
            CBUFFER_END */

            half4 _BaseColor;
            sampler2D _CameraDepthTexture;

            Varyings vert(Attributes input)
            {
                Varyings output;

                META_DEPTH_INITIALIZE_VERTEX_OUTPUT(output, input.vertex);

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.positionCS = UnityObjectToClipPos(input.vertex);
                // output.ScreenPos = ComputeScreenPos(output.positionCS);
                output.depth = output.positionCS.z / output.positionCS.w;

                // UNITY_TRANSFER_DEPTH(output.uv)
                // output.uv = input.uv;
                // output.worldPos = mul(unity_ObjectToWorld, input.vertex);

                return output;
            }

            // Easing function for the wave length
            /* float ease(float x)
            {
                return x * x * (3.0 - 2.0 * x);
            } */

            // Check the distance between depth texture and wave distance
            /* float waveAlpha(float depth, float waveDist, float threshold)
            {
                float dist = abs(depth - waveDist);
                float t = saturate(dist / threshold);
                return ease(t);
            } */

            /*float sampleDepthTexture(float2 uv)
			{
// #if defined(REQUIRE_DEPTH_TEXTURE)
// #if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				// half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
                half depth = SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;

				//half depth = LinearEyeDepth(SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
// #else
				// half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(uv));
                // half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.positionCS)));
// #endif
                return depth;
// #endif
                // return 0;
			}*/

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // fixed4 finalColor = _BaseColor;
                // fixed4 finalColor = tex2D(_CameraDepthTexture, input.uv);
                // UNITY_OUTPUT_DEPTH(input.uv);

                // float2 ScreenspaceUV = input.ScreenPos.xy / input.ScreenPos.w;
                // fixed4 finalColor = tex2D(_CameraDepthTexture, ScreenspaceUV);

                // float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.positionCS)));
/* #if defined(REQUIRE_DEPTH_TEXTURE)
#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
				half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.positionCS)));
				//half depth = LinearEyeDepth(SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex).r;
#else
				half depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(input.positionCS)));
#endif
#endif

                half depth = LinearEyeDepth(SAMPLE_TEXTURE2D_ARRAY(_CameraDepthTexture, UNITY_PROJ_COORD(input.positionCS))); */
                //half depth = sampleDepthTexture(input.positionCS.xy / input.positionCS.w);
                //fixed4 finalColor = fixed4(depth, depth, depth, 1.0);
                fixed4 finalColor = fixed4(1,1,1,1);
                

                // float4 depthSpace = mul(_EnvironmentDepthReprojectionMatrices[unity_StereoEyeIndex], float4(input.worldPos.xyz, 1.0));
                // float2 uvCoords = (depthSpace.xy / depthSpace.w + 1.0f) * 0.5f;
                // float linearSceneDepth = SampleEnvironmentDepthLinear_Internal(uvCoords);

                // float alpha = waveAlpha(linearSceneDepth, 3 * _WaveDistance, _Threshold);
                // finalColor.a *= alpha;
                // finalColor.a = alpha;
                // finalColor = float4(0, 1 - linearSceneDepth/1.5, 0, 0.9);
                META_DEPTH_OCCLUDE_OUTPUT_PREMULTIPLY(input, finalColor, 0);

                return finalColor;
            }
            ENDCG
        }
    }
}
