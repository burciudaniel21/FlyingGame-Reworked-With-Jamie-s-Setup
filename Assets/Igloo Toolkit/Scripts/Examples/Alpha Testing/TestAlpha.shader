Shader "Igloo/TestAlpha"
{
    Properties
    {
        _MainTex ("Texture1", 2D) = "" {}
        _startX("Start X", Range(0.00,1.00)) = 0.00
        _startY("Start Y", Range(0.00,1.00)) = 0.00
        _endX("End X", Range(0.00,1.00)) = 1.00
        _endY("End Y", Range(0.00,1.00)) = 1.00
        _alpha("Alpha", Range(0.00,1.00)) = 1.00
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
            uniform float _startX;
            uniform float _startY;
            uniform float _endX;
            uniform float _endY;
            uniform float _alpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
			    v2f t = i;
                fixed4 col = float4(0, 0, 0, 0);
                
                col = tex2D(_MainTex, t.uv);
                
                if (i.uv[0] <= _startX || i.uv[1] <= _startY || i.uv[0] >= _endX || i.uv[1] >= _endY) col = half4 (0, 0, 0, 0);

                return col;
            }
            ENDCG
        }
    }
}
