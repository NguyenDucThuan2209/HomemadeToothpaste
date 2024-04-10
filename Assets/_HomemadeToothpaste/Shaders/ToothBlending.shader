Shader "Custom/Tooth Blending"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Dirt Texture", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
     
        
        _CleanColor ("Clean Color", Color) = (1,1,1,1)
        _CleanTex ("Clean Texture", 2D) = "white" {}
        _CleanGlossiness ("Smoothness", Range(0,1)) = 0.5
        _CleanMetallic ("Metallic", Range(0,1)) = 0.0
        
           _Cleanliness("Cleanliness", Range(0,1)) = 0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _CleanTex;
        
        
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_CleanTex;
        };
        
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Cleanliness;
        
        half _CleanGlossiness;
        half _CleanMetallic;
        fixed4 _CleanColor;
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        
      float3 blend(float4 texture1, float a1, float4 texture2, float a2)
        {
            float depth = 0.2;
            float ma = max(texture1.a + a1, texture2.a + a2) - depth;

            float b1 = max(texture1.a + a1 - ma, 0);
            float b2 = max(texture2.a + a2 - ma, 0);

            return (texture1.rgb * b1 + texture2.rgb * b2) / (b1 + b2);
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 c1 = tex2D (_CleanTex, IN.uv_CleanTex) * _CleanColor;
            
            o.Albedo = lerp(c,c1,_Cleanliness);
            //o.Albedo = c + c1;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            //o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
