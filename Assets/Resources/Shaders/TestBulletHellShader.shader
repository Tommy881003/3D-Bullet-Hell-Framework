Shader "Unlit/TestBulletHellShader"
{
    Properties
    {
        [Gamma][HDR]_Color ("Color", Color) = (1,1,1,1)
        [Gamma][HDR]_WarnColor ("Warn Color", Color) = (1,1,1,1)
        [HideInInspector]_BulletPosition ("Bullet Position", Vector) = (-9999,-9999,-9999)
        [HideInInspector]_PlayerPosition ("Player Position", Vector) = (99999,99999,99999)
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
            // make instancing work
            #pragma multi_compile_instancing
            // make shadow work
            #pragma multi_compile_shadowcaster

            #include "UnityCG.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
                UNITY_DEFINE_INSTANCED_PROP(float3, _BulletPosition)
                UNITY_DEFINE_INSTANCED_PROP(float3, _PlayerPosition)
            UNITY_INSTANCING_BUFFER_END(Props)

            static const float COLOR_CHANGE_MAXDISTANCE = 7.5f;
            static const float COLOR_CHANGE_MINDISTANCE = 3.5f;
            
            fixed4 _WarnColor;
            
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
                float3 worldPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };


            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                fixed4 verx = v.vertex;

                o.vertex = UnityObjectToClipPos(verx);
                o.worldPos = mul(unity_ObjectToWorld, verx);
                o.normal = UnityObjectToWorldNormal(v.normal);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


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
                // Get per-instance data
                UNITY_SETUP_INSTANCE_ID(i);
                fixed4 color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                float3 bulletPos = UNITY_ACCESS_INSTANCED_PROP(Props, _BulletPosition);
                float3 playerPos = UNITY_ACCESS_INSTANCED_PROP(Props, _PlayerPosition);
                
                // Calculate Dot product
                fixed3 camNorm = normalize(_WorldSpaceCameraPos - i.worldPos); 
                float dotProduct = dot(camNorm, i.normal);

                // Calculate base color based on distance between player and bullet
                float baseColorLerp = saturate((distance(bulletPos, playerPos) - COLOR_CHANGE_MINDISTANCE) / (COLOR_CHANGE_MAXDISTANCE - COLOR_CHANGE_MINDISTANCE));
                fixed4 _BaseColor = lerp(_WarnColor, color, baseColorLerp);

                fixed4 _RimColor = pow(_BaseColor, curve(dotProduct) + 0.2f);
                fixed4 _CenterColor = _BaseColor * 0.1f;
                _CenterColor.w = 1;

                dotProduct = pow(dotProduct, 1.1f);
                fixed4 col = lerp(_RimColor, _CenterColor, dotProduct);
                
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
