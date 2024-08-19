Shader "Custom/SonarEffectForObjects"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _BaseColor ("Base Color", Color) = (1, 1, 1, 1)
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

            // CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float _WaveDistance;
                float _Threshold;
                sampler2D _CameraDepthTexture;
            // CBUFFER_END

            // Easing function for the wave length
            float ease(float x)
            {
                return x * x * (3.0 - 2.0 * x);
            }

            // Check the distance between depth texture and wave distance
            float waveAlpha(float depth, float waveDist, float threshold)
            {
                float dist = abs(depth - waveDist);
                float t = saturate(dist / threshold);
                return ease(t);
            }

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.pos = UnityObjectToClipPos(input.vertex);
                float zDepth = LinearEyeDepth(output.pos.z / output.pos.w);

                float alpha = waveAlpha(zDepth, 3 * _WaveDistance, _Threshold);

                output.depth = zDepth;
                output.color = _BaseColor;
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
