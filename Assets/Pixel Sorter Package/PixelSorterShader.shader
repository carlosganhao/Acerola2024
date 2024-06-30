Shader "Hidden/PixelSorterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LiveTex ("Texture", 2D) = "white" {}
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

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            sampler2D _LiveTex;

            fixed4 frag (v2f i, UNITY_VPOS_TYPE fragCoord : SV_POSITION) : SV_Target
            {
                fixed4 col;
                fixed2 texel = fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y);
                
                float step_y = texel.y;
                fixed2 s  = fixed2(0.0, -step_y);
                fixed2 n  = fixed2(0.0, step_y);

                fixed4 im_n =  tex2D(_MainTex, i.uv+n);
                fixed4 im =    tex2D(_MainTex, i.uv);
                fixed4 im_s =  tex2D(_MainTex, i.uv+s);
                
                float len_n = length(im_n);
                float len = length(im);
                float len_s = length(im_s);
                
                if(int(fmod(float(_Time.y) + fragCoord.y, 2.0)) == 0) {
                    if ((len_s > len)) { 
                        im = im_s;    
                    }
                } else {
                    if ((len_n < len)) { 
                        im = im_n;    
                    }   
                }
                
                // initialize with image
                
                if(_Time.y<5) {
                    col = tex2D(_LiveTex, i.uv);
                } else {
                    col = im;
                }

                //fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                //col.rgb = 1 - col.rgb;
                return col;
            }
            ENDCG
        }
    }
}
