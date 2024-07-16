Shader "Custom/SonarEffect"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _CameraDepthTexture ("Camera Depth Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            ZWrite On
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityShaderVariables.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _CameraDepthTexture;
            float _WaveDistance;
            float _Threshold;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            float SampleLinearDepth(float rawDepth)
            {
                float z = rawDepth * 2.0 - 1.0;
                float linearDepth = (2.0 * _ProjectionParams.y) / (_ProjectionParams.z + _ProjectionParams.w - z * (_ProjectionParams.z - _ProjectionParams.w));
                return linearDepth;
            }

            float ease(float x)
            {
                return x * x * (3.0 - 2.0 * x);
            }

            float waveAlpha(float depth, float waveDist, float threshold)
            {
                float dist = abs(depth - waveDist);
                float t = saturate(dist / threshold);
                return ease(t);
            }

            float2 ComputeReprojectedUV(float4 worldPos, int eyeIndex)
            {
                float4 clipPos = mul(UNITY_MATRIX_VP, worldPos);
                float2 ndcPos = clipPos.xy / clipPos.w;
                return 0.5 * (ndcPos + float2(1.0, 1.0));
            }

            float4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

                float2 uv = ComputeReprojectedUV(i.worldPos, unity_StereoEyeIndex);
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
                float linearDepth = SampleLinearDepth(depth);

                float alpha = waveAlpha(linearDepth, _WaveDistance, _Threshold);

                return float4(0, 0, 0, alpha);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
