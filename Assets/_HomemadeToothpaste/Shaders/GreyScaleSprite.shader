Shader "Unlit/GreyScale" {
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" { }
        _GreyScale ("GreyScale", Range(0,1)) = 1
    }
    SubShader {
        Pass {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            sampler2D _MainTex;
            float _GreyScale;
            
            struct v2f {
                float4  pos : SV_POSITION;
                float2  uv : TEXCOORD0;
            };
            
            float4 _MainTex_ST;
            float4 _Color;
            
            v2f vert (appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.uv = TRANSFORM_TEX (v.texcoord, _MainTex);
                return o;
            }
            
            half4 frag (v2f i) : COLOR
            {
                half4 texcol = tex2D (_MainTex, i.uv);
                half4 temp = texcol;
                texcol.rgb = dot(texcol.rgb, float3(0.3, 0.59, 0.11));
                half4 result = lerp(texcol, temp, _GreyScale);
                return result * _Color;
            }
            ENDCG
            
        }
    }
Fallback "VertexLit"}