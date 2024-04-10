// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/PowderShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _SecondaryColor ("Secondary Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _TargetColor ("Secondary Map Color", Color) = (1,1,1,1)
        _TargetTex("Secondary Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _BlackThreshold("BlackThreshold", Range(0,1)) = 0.0
                _NoiseMask("NoiseMask", 2D) = "white" {}

    }
    SubShader
    {
   
        Tags { "RenderType"="Transparent" }
        LOD 200
        Cull Off
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
         sampler2D _TargetTex;
             sampler2D _NoiseMask;
         
        struct Input
        {
            float2 uv_MainTex;
            float3 localPos;
            float3 worldPos;
            float facing : VFACE;
            float2 uv_TargetTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _SecondaryColor;
                fixed4 _TargetColor;
                float _BlackThreshold;
        
         void vert (inout appdata_full v, out Input o) {
           UNITY_INITIALIZE_OUTPUT(Input,o);
           o.localPos = mul(unity_WorldToObject, o.worldPos);
           o.worldPos = mul(unity_ObjectToWorld,v.vertex);
         }
         
         float Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax)
        {
            return OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        float3 blend(float4 texture1, float a1, float4 texture2, float a2)
        {
             float depth = 0.2;
            float ma = max(texture1.a + a1, texture2.a + a2) - depth;

            float b1 = max(texture1.a + a1 - ma, 0);
            float b2 = max(texture2.a + a2 - ma, 0);

            return (texture1.rgb * b1 + texture2.rgb * b2) / (b1 + b2);
        }
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 c1 = tex2D (_TargetTex, IN.uv_TargetTex) * _TargetColor;
            fixed4 mask = tex2D(_NoiseMask, IN.uv_MainTex) *_Color;
                o.Albedo = blend(c,c.a,c1,c1.a);
            
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a ;
            if(mask.r < (1 -  _BlackThreshold) && mask.g < (1 -  _BlackThreshold) && mask.b < (1 -  _BlackThreshold))
            {
                o.Albedo *= _SecondaryColor;
            }
          
        }
        ENDCG
    }
    FallBack "Diffuse"
}
