Shader "TA Customs/3D/Fire Tornado"
{
    Properties
    {
        [Header(_______________________________________________________________________________________________________________________________________)]
        [Header(Base Fire Color)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color("In Color", Color) = (1,1,1,1)
        _Color2("Out Color", Color) = (1,1,1,1)
        _Dissolve_1("Fire Edge Amount", Range(0,1)) = 0

        [Header(_______________________________________________________________________________________________________________________________________)]
        [Header(Fire Animation)]
        _OffsetMove_X("Offset Move_X", float) = 1
        _OffsetMove_Y("Offset Move_Y", float) = 1

        [Header(_______________________________________________________________________________________________________________________________________)]
        [Header(Texture Distortion)]
        _DistortionTex("Distortion Texture(Noise)", 2D) = "black" {}
        _DistortionIntensity("Distortion Intensity", float) = 0.3
        _DistortionIMove_X("Distortion Speed_X", float) = 1
        _DistortionIMove_Y("Distortion Speed_Y", float) = 1

        [Header(_______________________________________________________________________________________________________________________________________)]
        [Header(Vertex Animation)]
        _VATex("Vertex Animation Tex", 2D) = "black" {}
        _VAIntensity("Vertex Animation Intensity", float) = 0.0001
        _VASpeedX("Vertex Animation SpeedX", float) = 0
        _VASpeedY("Vertex Animation SpeedY", float) = 0.3

        [Header(_______________________________________________________________________________________________________________________________________)]
        _Cutoff("Alpha Cutoff", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

        Lighting Off
        Cull Off
        ZWrite Off

        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            // Note: This one is used for tex2Dlod() call in the vertex shader.
            //       That tex2Dlod() makes it available to sample textures in vertex shader,
            //       but it might not be supported on devices that only support OpenGL ES 2.0 (e.g. Galaxy S3 LTE)
            //       So I've added the fallback subshader under this subshader.
            #pragma require samplelod

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DistortionTex;
            float4 _DistortionTex_ST;
            
            float _DistortionIntensity;
            float _DistortionIMove_X;
            float _DistortionIMove_Y;

            fixed _Dissolve_1;
            float _OffsetMove_X;
            float _OffsetMove_Y;

            float4 _Color;
            float4 _Color2;

            sampler2D _VATex;
            float4 _VATex_ST;
            float _VAIntensity;
            float _VASpeedX;
            float _VASpeedY;

            fixed _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;

                // Vertex Animation from textrue
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                fixed4 VA = tex2Dlod(_VATex, float4(o.uv.xy * _VATex_ST.xy + _VATex_ST.zw + (_Time.y * float2(_VASpeedX, _VASpeedY)), 0,0));
                float3 ani = v.vertex + normalize(v.normal) * VA.x*_VAIntensity;

                // MVP vertex data(Model view projection)
                o.vertex = UnityObjectToClipPos(ani);

                // Vertex color to fragment 
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 디스토션 샘플링 
                float2 fM = (_Time.y * float2(_DistortionIMove_X, _DistortionIMove_Y));
                float4 distortion = tex2D(_DistortionTex, i.uv * _DistortionTex_ST.xy + _DistortionTex_ST.zw + fM) * _DistortionIntensity;

                // MainTexture 텍스쳐  두개섞어서 씀
                fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + distortion.x + (_MainTex_ST.zw + (_Time.y * float2(_OffsetMove_X, _OffsetMove_Y))));
                fixed4 col2 = tex2D(_MainTex, i.uv * _MainTex_ST.xy + (_MainTex_ST.zw - (_Time.y*0.1f * float2(_OffsetMove_X, _OffsetMove_Y))));

                // 디졸브    
                col.r -= _Dissolve_1;

                // 화염색깔 안밖표현 
                float4 fC = 1;
                fC.rgb = (col.r * _Color.rgb) + (col2.r * _Color2.rgb) * 2.0f;
                fC.a = col.r * col2.r;
                fC.a = ceil(fC.a) * i.color.r;

                //clip(fC.a - _Cutoff);
                return fC;
            }
            ENDCG
        }
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

        Lighting Off
        Cull Off
        ZWrite Off

        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            // Fallback Shader for Older Devices using OpenGL ES 2.0
            #pragma target 2.0
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _DistortionTex;
            float4 _DistortionTex_ST;
            
            float _DistortionIntensity;
            float _DistortionIMove_X;
            float _DistortionIMove_Y;

            fixed _Dissolve_1;
            float _OffsetMove_X;
            float _OffsetMove_Y;

            float4 _Color;
            float4 _Color2;

            sampler2D _VATex;
            float4 _VATex_ST;
            float _VAIntensity;
            float _VASpeedX;
            float _VASpeedY;

            fixed _Cutoff;

            v2f vert (appdata v)
            {
                v2f o;

                // Removed Vertex Animation
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // MVP vertex data(Model view projection)
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Vertex color to fragment 
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 디스토션 샘플링 
                float2 fM = (_Time.y * float2(_DistortionIMove_X, _DistortionIMove_Y));
                float4 distortion = tex2D(_DistortionTex, i.uv * _DistortionTex_ST.xy + _DistortionTex_ST.zw + fM) * _DistortionIntensity;

                // MainTexture 텍스쳐  두개섞어서 씀
                fixed4 col = tex2D(_MainTex, i.uv * _MainTex_ST.xy + distortion.x + (_MainTex_ST.zw + (_Time.y * float2(_OffsetMove_X, _OffsetMove_Y))));
                fixed4 col2 = tex2D(_MainTex, i.uv * _MainTex_ST.xy + (_MainTex_ST.zw - (_Time.y*0.1f * float2(_OffsetMove_X, _OffsetMove_Y))));

                // 디졸브    
                col.r -= _Dissolve_1;

                // 화염색깔 안밖표현 
                float4 fC = 1;
                fC.rgb = (col.r * _Color.rgb) + (col2.r * _Color2.rgb) * 2.0f;
                fC.a = col.r * col2.r;
                fC.a = ceil(fC.a) * i.color.r;

                //clip(fC.a - _Cutoff);
                return fC;
            }
            ENDCG
        }
    }
}
