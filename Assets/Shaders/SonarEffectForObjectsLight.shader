Shader "Custom/SonarEffectForObjectsTextureLight"
{
    Properties
    {
        _WaveDistance ("Wave Distance", Float) = 1.0
        _MaxWaveDistance ("Max Wave Distance", Float) = 1.0
        _Threshold ("Threshold", Float) = 0.1
        _Color ("Color", Color) = (1, 1, 1, 1)
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
            #pragma multi_compile_fwdbasealpha
            #include "UnityCG.cginc"
            #include  "Lighting.cginc"


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
                float4 worldPos: TEXCOORD1;
                float3 worldNormal: TEXCOORD2;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                float _MaxWaveDistance;
                float _WaveDistance;
                float _Threshold;
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

                output.pos = UnityObjectToClipPos(input.vertex);
                float3 worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
                output.worldPos = float4(worldPos, 1.0);
                output.worldNormal = mul((float3x3)unity_ObjectToWorld, normalize(input.vertex.xyz));

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

                // Lighting calculations
                float3 worldNormal = normalize(input.worldNormal);
                float3 worldPos = input.worldPos.xyz;

                // Initialize final color as the base color
                fixed4 finalColor = input.color;

                // Handle multiple real-time lights with attenuation (e.g., point lights)
                float3 lighting = 0;

               // Handle the main directional light
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float ndotl = max(0, dot(worldNormal, lightDir)); // Lambertian reflectance
                float3 lightColor = _LightColor0.rgb;

                lighting += ndotl * lightColor;
                
                // Handle point lights
                #ifdef _VERTEX_LIGHTS
                lighting += Shade4PointLights(worldNormal, worldPos);
                #endif

                // Combine lighting with the base color
                finalColor.rgb *= lighting + UNITY_LIGHTMODEL_AMBIENT.rgb;

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
