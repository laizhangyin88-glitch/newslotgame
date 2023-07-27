// from "Mobile/Bumped Specular (1 Directional Realtime Light)"

Shader "SlotMaker2/3D/Inferno Ball" 
{
    Properties
    {
        _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
        [NoScaleOffset] _BumpMap ("Normal Map", 2D) = "bump" {}
        
        _Color ("[Instancing] Base Tint (RGB) Gray (A)", Color) = (1,1,1,0)
        
        [Header(Tiling Offset)]
        [Toggle(_ENABLE_TILING_OFFSET)] _EnableTilingOffset ("Enable Tiling Offset", Float) = 0
        _TilingOffset ("[Instancing] Tiling (XY) Offset (ZW)", Vector) = (1,1,0,0)
        
        [Header(Grayscale)]
        [Toggle(_ENABLE_GRAYSCALE)] _EnableGrayscale ("Enable Grayscale", Float) = 0
        _GrayColor ("Luma Coefficient", Color) = (0.3,0.59,0.11,1)

        [Header(Environment)]
        [Toggle(_ENABLE_ENV)] _EnableEnv ("Enable Environment Map", Float) = 0
        [NoScaleOffset] _EnvMap ("Environment Map", CUBE) = "" {}
        _ReflectionAmount("[Instancing] Reflection Amount", Range(0, 1)) = 0

        [Header(Ambient)]
        _AmbientColor("Ambient Color", Color) = (0,0,0,1)
        
        [Header(Specular)]
        _SpecularColor("Specular Color", Color) = (0.9,0.9,0.9,1)
        _SpecularPower("Specular Power", Float) = 32
        _SpecularThickness("Specular Thickness", Range(0,1)) = 0.0075
        _SpecularSoftness("Specular Softness", Range(0,1)) = 0.0025

        [Header(Rim)]
        _RimColor("[Instancing] Rim Color", Color) = (1,1,1,1)
        _RimPower("[Instancing] Rim Power", Float) = 1
        _RimThickness("[Instancing] Rim Thickness", Range(0,1)) =  0.716
        _RimSoftness("[Instancing] Rim Softness", Range(0,1)) = 0.01
    }    

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 250

        CGPROGRAM
            #pragma surface surf MobileFresnel vertex:vert exclude_path:prepass nolightmap noforwardadd novertexlights
            #pragma multi_compile __ _ENABLE_TILING_OFFSET
            #pragma multi_compile __ _ENABLE_GRAYSCALE
            #pragma multi_compile __ _ENABLE_ENV

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _BumpMap;
            samplerCUBE _EnvMap;

            fixed4 _GrayColor;
            fixed4 _AmbientColor;
            fixed4 _SpecularColor;
            fixed _SpecularPower;
            fixed _SpecularThickness;
            fixed _SpecularSoftness;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _Color)
        #if defined(_ENABLE_TILING_OFFSET)
            UNITY_DEFINE_INSTANCED_PROP(float4, _TilingOffset)
        #endif
            UNITY_DEFINE_INSTANCED_PROP(fixed4, _RimColor)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _RimPower)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _RimThickness)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _RimSoftness)
        #if defined(_ENABLE_ENV)
            UNITY_DEFINE_INSTANCED_PROP(fixed, _ReflectionAmount)
        #endif
            UNITY_INSTANCING_BUFFER_END(Props)
            
            struct Input 
            {
                float2 uv_MainTex;
            #if defined(_ENABLE_ENV)
                float3 worldRefl;
                INTERNAL_DATA
            #endif
            };
            
            void vert (inout appdata_full v) 
            {
            #if defined(_ENABLE_TILING_OFFSET)
                float4 tilingOffset = UNITY_ACCESS_INSTANCED_PROP(Props, _TilingOffset);
                v.texcoord.xy = v.texcoord.xy * tilingOffset.xy + tilingOffset.zw;
            #endif    
            }
            
            inline fixed4 LightingMobileFresnel(SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
            {
                fixed4 _color = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed4 _rimColor = UNITY_ACCESS_INSTANCED_PROP(Props, _RimColor);
                fixed _rimPower = UNITY_ACCESS_INSTANCED_PROP(Props, _RimPower);
                fixed _rimThickness = UNITY_ACCESS_INSTANCED_PROP(Props, _RimThickness);
                fixed _rimSoftness = UNITY_ACCESS_INSTANCED_PROP(Props, _RimSoftness);

                fixed3 halfDir = normalize(lightDir + viewDir);
                
                fixed NdotL = max(0, dot(s.Normal, lightDir));
                fixed NdotH = max(0, dot(s.Normal, halfDir));
                fixed NdotV = max(0, dot(s.Normal, viewDir));

                // Diffuse Wrap
                fixed lightIntensity = NdotL * 0.5 + 0.5;

                fixed3 diffuse = s.Albedo * _color.rgb * _LightColor0.rgb * lightIntensity;
            #if defined(_ENABLE_GRAYSCALE)
                diffuse = lerp(diffuse, dot(_GrayColor.rgb, diffuse), _color.a);
            #endif
                
                // Specular
                fixed specularIntensity = pow(NdotH, _SpecularPower) * s.Gloss;
                fixed specularIntensitySmooth = smoothstep(_SpecularThickness - _SpecularSoftness, _SpecularThickness + _SpecularSoftness, specularIntensity);
                fixed3 specular = _LightColor0.rgb * _SpecularColor * specularIntensitySmooth;
                
                // Rim
                fixed rimIntensity = pow((1 - NdotV), _rimPower);
                fixed rimIntensitySmooth = smoothstep(_rimThickness - _rimSoftness, _rimThickness + _rimSoftness, rimIntensity);
                fixed3 rim = _rimColor * rimIntensitySmooth;
                
                fixed4 c;
                c.rgb = (_AmbientColor + diffuse + specular + rim) * atten;
                UNITY_OPAQUE_ALPHA(c.a);
                return c;
            }

            void surf (Input IN, inout SurfaceOutput o) 
            {
                fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
                o.Albedo = tex.rgb;
                o.Gloss = tex.a;
                o.Alpha = tex.a;
                o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
            #if defined(_ENABLE_ENV)
                o.Emission = texCUBE(_EnvMap, WorldReflectionVector(IN, o.Normal)).rgb * UNITY_ACCESS_INSTANCED_PROP(Props, _ReflectionAmount) * o.Gloss;
            #endif
            }
        ENDCG
    }
}