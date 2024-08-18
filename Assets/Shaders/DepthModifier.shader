Shader "Custom/DepthModifier"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        // _DepthOffset ("Depth Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float depth : TEXCOORD0;
                float4 color : COLOR;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // User-defined properties
            float4 _Color;
            // float _DepthOffset;
            sampler2D _CameraDepthTexture;

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.pos = UnityObjectToClipPos(v.vertex);
                // Access the depth value from the depth texture
                float zDepth = 1 - LinearEyeDepth(o.pos.z / o.pos.w);

                
                // Apply depth offset in clip space
                // o.pos.z += _DepthOffset;
                    
                // _Color.a = zDepth;
                o.depth = zDepth;
                o.color = _Color;
                o.color.a = zDepth;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                fixed4 finalColor = i.color;
                // finalColor.rgb = LinearEyeDepth(i.depth);
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
