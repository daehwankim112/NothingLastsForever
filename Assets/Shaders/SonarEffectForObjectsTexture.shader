Shader "Custom/SonarEffectForObjectsTexture"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _MaxWaveDistance ("Max Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Texture", 2D) = "white" {}
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
                float2 uv : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
                float4 color : COLOR;
                float2 uv : TEXCOORD1;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _MaxWaveDistance;
                float _WaveDistance;
                float _Threshold;
                sampler2D _CameraDepthTexture;
                sampler2D _MainTex;
            CBUFFER_END

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

            Varyings vert (Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = input.uv;

                output.pos = UnityObjectToClipPos(input.vertex);
                float zDepth = LinearEyeDepth(output.pos.z / output.pos.w);

                float alpha = waveAlpha(zDepth, _WaveDistance, _Threshold, _MaxWaveDistance);

                output.depth = zDepth;
                output.color = _Color;
                output.color.a = 1 - alpha;
                return output;
            }

            fixed4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                fixed4 finalColor = input.color;
                // finalColor.rgb = tex2D(_MainTex, input.uv).rgb + input.color.rgb;
                finalColor.rgb = tex2D(_MainTex, input.uv).rgb;
                // finalColor.a = input.color.a;
                // finalColor = tex2D(_MainTex, input.uv);
                // finalColor.rgb = LinearEyeDepth(i.depth);
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
