Shader "BW/URP_LitShader"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}

        _Color("Color", Color) = (1,0,0,1)
        _Cutoff("Base Alpha cutoff", Range(0,.9)) = .1
    }
    
    SubShader
    {
        Tags {
            "RenderPipeline" = "UniversalPipeline"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            Ztest lequal
            ZWrite on

            HLSLPROGRAM
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Structure of the vertex
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR0;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // What we pass to the pixel shader
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _BaseMap;
            float _Cutoff;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.worldPos = TransformObjectToWorld(v.vertex);
                o.color = v.color;
                o.normal = TransformObjectToWorldNormal(v.normal);
                o.uv = v.uv;
                return o;
            }

            // Colour passed in from material
            float4 _Color;

            float3 CalculateLambertTerm(float3 lightColor, float3 lightDir, float3 normal)
            {
                float NdotL = saturate(dot(normal, lightDir));
                return lightColor * NdotL;
            }

            float4 frag(v2f i) : SV_Target
            {
                // Start with material colour modulated with vertex colour
                half4 albedo = tex2D(_BaseMap, i.uv);

                float4 color = _Color * i.color;// *albedo;

                float3 lightPos = _MainLightPosition.xyz;
                float3 lightCol = CalculateLambertTerm(_MainLightColor * unity_LightData.z, lightPos, i.normal);

                uint lightsCount = GetAdditionalLightsCount();
                for (int j = 0; j < lightsCount; j++)
                {
                    Light light = GetAdditionalLight(j, i.worldPos);
                    lightCol += CalculateLambertTerm(light.color * (light.distanceAttenuation * light.shadowAttenuation), light.direction, i.normal);
                }

                color.rgb += i.color;
                clip(-(color.a - 0.019));

                return color;
            }
            ENDHLSL
        }
    }

}