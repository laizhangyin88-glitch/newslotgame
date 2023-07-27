Shader "CVSTA/UI/Distortion(Lagacy)"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo (RGB)", 2D) = "white" {}
        _FlowStrength ("Flow Strength", Range(0,10)) = 1
        _Reposition ("Reposition", Range(-10,0)) = 0
        _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 200

        CGPROGRAM

        #pragma surface surf NoLighting alpha:fade
      

        sampler2D _MainTex;
        sampler2D _MainTex2;
        fixed _FlowStrength;
        fixed _Reposition;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
        };
        
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 d = tex2D (_MainTex2, float2(IN.uv_MainTex2.x , IN.uv_MainTex2.y - _Time.y));
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex + _Reposition + (d.r * _FlowStrength)) * _Color;
          
            o.Emission = c.rgb;
            o.Alpha = c.a;
        }

        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
            fixed4 c;
            c.rgb = s.Albedo;
            c.a = s.Alpha;
            return c;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
