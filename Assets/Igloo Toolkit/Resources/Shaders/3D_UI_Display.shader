Shader "IglooVision/3D_UI_Display"
{
    Properties
    {
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
         ZTest Always
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog alpha
         
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                 half4 c = tex2D(_MainTex, i.uv);
                if (i.uv.x <= 0.0f || i.uv.y <= 0.0f || i.uv.x >= 1.0f || i.uv.y >= 1.0f)
                    c = half4 (0,0,0,0);
                return c;
               //// sample the texture
               //fixed4 col = tex2D(_MainTex, i.uv);
               //// apply fog
               //UNITY_APPLY_FOG(i.fogCoord, col);
               // return col;
            }
            ENDCG
        }
    }
}
