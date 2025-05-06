Shader "Custom/TerrainLitRampURP"
{
    Properties
    {
        [HideInInspector] _Control("Control (RGBA)", 2D) = "white" {}
        [HideInInspector] _Splat0("Layer 0", 2D) = "white" {}
        [HideInInspector] _Splat1("Layer 1", 2D) = "white" {}
        [HideInInspector] _Splat2("Layer 2", 2D) = "white" {}
        [HideInInspector] _Splat3("Layer 3", 2D) = "white" {}

        _RampTex("Ramp Texture", 2D) = "white" {}

        _DarkColor("Dark Color", Color) = (0.1, 0.1, 0.1, 1)
        _LitColor("Lit Color", Color) = (1, 1, 1, 1)

        _ShadowColor("Shadow Color", Color) = (0.2, 0.2, 0.2, 1) // цвет теней
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry-100" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv0        : TEXCOORD0;
                float2 uv1        : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uvSplat     : TEXCOORD0;
                float2 uvControl   : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                float3 worldPosWS  : TEXCOORD4;
            };

            sampler2D _Splat0, _Splat1, _Splat2, _Splat3;
            sampler2D _Control;
            sampler2D _RampTex;

            float4 _DarkColor;
            float4 _LitColor;
            float4 _ShadowColor; // новое поле

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uvSplat     = IN.uv0;
                OUT.uvControl   = IN.uv1;
                OUT.normalWS    = TransformObjectToWorldNormal(IN.normalOS);
                OUT.worldPosWS  = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.shadowCoord = TransformWorldToShadowCoord(OUT.worldPosWS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 control = tex2D(_Control, IN.uvControl);
                float3 c0 = tex2D(_Splat0, IN.uvSplat).rgb;
                float3 c1 = tex2D(_Splat1, IN.uvSplat).rgb;
                float3 c2 = tex2D(_Splat2, IN.uvSplat).rgb;
                float3 c3 = tex2D(_Splat3, IN.uvSplat).rgb;
                float3 albedo = c0*control.r + c1*control.g + c2*control.b + c3*control.a;

                float3 normal = normalize(IN.normalWS);
                Light mainLight = GetMainLight(IN.shadowCoord);

                float ndotl = saturate(dot(normal, mainLight.direction));
                float rampVal = tex2D(_RampTex, float2(ndotl,0.5)).r;
                float3 rampColor = lerp(_DarkColor.rgb, _LitColor.rgb, rampVal);

                float3 biasedPos = ApplyShadowBias(IN.worldPosWS, normal, mainLight.direction);
                float4 shadowCoordBI = TransformWorldToShadowCoord(biasedPos);
                half rawShadow = MainLightRealtimeShadow(shadowCoordBI);
                half fade = GetMainLightShadowFade(IN.worldPosWS);
                half shadowAtten = lerp(rawShadow, 1.0, fade);

                // регулируем цвет тени: shadowAtten=0 → _ShadowColor, shadowAtten=1 → белый (без тени)
                float3 shadowTint = lerp(_ShadowColor.rgb, float3(1,1,1), shadowAtten);

                float3 finalColor = albedo * rampColor * mainLight.color * shadowTint;
                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/InternalErrorShader"
}
