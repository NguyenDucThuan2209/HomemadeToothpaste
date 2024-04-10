Shader "Custom/Orbeez"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
       [HDR]_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
       [PowerSlider(4)] _FresnelExponent ("Fresnel Exponent", Range(0, 10)) = 1
       
       //       [HDR]_InnerColor("Inner Color", Color) = (1,1,1,1)
       //[PowerSlider(4)] _InnerGlowSize ("Inner Glow Size", Range(0, 10)) = 1
    }
    SubShader
    {

        Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
        LOD 200
   
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
       #pragma surface surf Standard fullforwardshadows alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        //#pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldNormal;
            float3 viewDir;
            INTERNAL_DATA
        };

        half _Glossiness;
        half _Metallic;
        float4 _Color;
        float4 _FresnelColor;
        float _FresnelExponent;
        //  float4 _InnerColor;
        //float _InnerGlowSize;
        
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
           //get the dot product between the normal and the view direction
            float fresnel = dot(IN.worldNormal, IN.viewDir);
            
            
            //float innerGlow = fresnel;
            //innerGlow = pow(innerGlow, _InnerGlowSize);
            //float3 innerColor = innerGlow * _InnerColor;
            //invert the fresnel so the big values are on the outside
            fresnel = saturate(1 - fresnel);
            
             //raise the fresnel value to the exponents power to be able to adjust it
            fresnel = pow(fresnel, _FresnelExponent);
            //combine the fresnel value with a color
            float3 fresnelColor = fresnel * _FresnelColor;
            

            //apply the fresnel value to the emission
            o.Emission = fresnelColor;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Standard"
}
