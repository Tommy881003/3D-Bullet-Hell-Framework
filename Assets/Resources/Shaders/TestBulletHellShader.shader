Shader "Unlit/TestBulletHellShader"
{
    Properties
    {
        [Gamma][HDR]_Color ("Color", Color) = (1,1,1,1)
        _Freq ("Frequency", float) = 1
        _Amp ("Amplitude", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            struct appdata 
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float3 normal : NORMAL; 
                UNITY_FOG_COORDS(1)
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            float _Freq;
            float _Amp;

            float random(float2 uv)
            {
                return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                fixed4 verx = v.vertex + fixed4(v.normal.x, v.normal.y, v.normal.z, 0) * (sin((_Time.y + random(v.vertex.xy)) * 6.28f * _Freq) - 1) * _Amp;

                o.vertex = UnityObjectToClipPos(verx);
                o.worldPos = mul(unity_ObjectToWorld, verx);
                o.normal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 _Color;
            static const fixed4 WHITE = fixed4(1, 1, 1, 1);

            float curve(float t)
            {
                float chooser = step(0.1f, t);
                float firstHalf = t * 2;
                float secondHalf = (t - 0.5f) / 0.5f;
                float first = pow((1 - firstHalf), 2);
                float second = pow(secondHalf, 2);

                return lerp(first, second, chooser);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                fixed3 camNorm = normalize(_WorldSpaceCameraPos - i.worldPos); 
                float dotProduct = dot(camNorm, i.normal);
                
                fixed4 _TrueColor = pow(_Color, curve(dotProduct) + 0.2f);
                fixed4 _MainColor = _Color * 0.1f;
                _MainColor.w = 1;

                dotProduct = pow(dotProduct, 1.1f);
                fixed4 col = lerp(_TrueColor, _MainColor, dotProduct);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
