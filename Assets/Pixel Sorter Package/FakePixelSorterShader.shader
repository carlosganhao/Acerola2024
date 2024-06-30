Shader "Hidden/FakePixelSorterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TexelOffset ("Offset", Range(0, 200)) = 0
        _OffsetDirection ("Offset Direction", Range(-1, 1)) = 0
        _Grainyness ("Grainyness", Range(0, 1)) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float rand(float co) { return frac(sin(co*(91.3458)) * 47453.5453); }

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _TexelOffset;
            float _OffsetDirection;
            float _Grainyness;

            // Based on shader by curiouspers, in: https://www.shadertoy.com/view/wljyRz
            fixed4 frag (v2f i, UNITY_VPOS_TYPE fragCoord : SV_POSITION) : SV_Target
            {
                fixed2 texel = fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
                
                fixed4 img = tex2D(_MainTex, i.uv);
                
                float step_y = texel.y*(rand(i.uv.x)*_TexelOffset);
                step_y += rand(i.uv.x*i.uv.y*_Time.y)*_Grainyness;
                
                i.uv.y += _OffsetDirection * step_y;
                
                img = tex2D(_MainTex, i.uv);
                return img;   
            }
            ENDCG
        }
    }
}
