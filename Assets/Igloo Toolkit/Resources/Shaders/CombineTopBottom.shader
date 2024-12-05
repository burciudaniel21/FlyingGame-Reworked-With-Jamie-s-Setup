Shader "Hidden/Igloo/CombineTopBottom"
{
    Properties
    {
        _MainTex ("Texture1", 2D) = "" {}
		_Texture2("Texture2", 2D) = "" {}
	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
			sampler2D _Texture2;
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
			v2f t = i;
			fixed4 col = float4(1, 0, 0, 1);
			if (t.uv[1] < 0.5f) {
				t.uv[1] = t.uv[1] * 2.0f;
				col = tex2D(_MainTex, t.uv);
			}
			else  {
				t.uv[1] = (t.uv[1]-0.5f) * 2.0f;
				col = tex2D(_Texture2, t.uv);
			}
                return col;
            }
            ENDCG
        }
    }
}
