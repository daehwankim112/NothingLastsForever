Shader "Custom/SonarEffectForObjectsSubLight"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _MaxWaveDistance ("Max Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _Color ("Color", Color) = (1, 1, 1, 1)
        _NearbyObjectColor ("Nearby Object Color", Color) = (1, 0, 0, 1)
        _SubmarinesPositions ("Object Count", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            struct Attributes
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half4 _NearbyObjectColor;
                float _MaxWaveDistance;
                float _WaveDistance;
                float _Threshold;
                int _ObjectCount;
                sampler2D _CameraDepthTexture;
                StructuredBuffer<float3> _SubmarinesPositions;
            CBUFFER_END

            
            float ease(float x)
            {
                return x * x * (3.0 - 2.0 * x);
            }

            float waveAlpha(float depth, float waveDist, float threshold, float maxWaveDistance)
            {
                float dist = abs(depth - waveDist);
                float t = saturate(dist / threshold);
                return ease(t) + (waveDist / maxWaveDistance);
            }

            float computeNearestObjectDistance(float3 boidPosition, StructuredBuffer<float3> objectPositions, int objectCount)
            {
                float nearestDist = 1e15;
                for (int i = 0; i < objectCount; i++)
                {
                    float dist = distance(boidPosition, objectPositions[i]);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                    }
                }
                return nearestDist;
            }

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.pos = UnityObjectToClipPos(input.vertex);
                float zDepth = LinearEyeDepth(output.pos.z / output.pos.w);

                float alpha = waveAlpha(zDepth, _WaveDistance, _Threshold, _MaxWaveDistance);

                float nearestObjectDistance = computeNearestObjectDistance(input.vertex.xyz, _ObjectPositions, _ObjectCount);

                if (nearestObjectDistance < _WaveDistance)
                {
                    output.color = _NearbyObjectColor; // Change to nearby object color
                    alpha = nearestObjectDistance / _WaveDistance; // Modify alpha based on distance
                }
                else
                {
                    output.color = _Color; // Default color
                }

                output.depth = zDepth;
                output.color.a = 1 - alpha;
                return output;
            }

            fixed4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                fixed4 finalColor = input.color;
                // finalColor.rgb = LinearEyeDepth(i.depth);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
