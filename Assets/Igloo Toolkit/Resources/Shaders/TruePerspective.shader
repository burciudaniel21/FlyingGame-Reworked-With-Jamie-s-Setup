Shader "Igloo/TruePerspective"
{
    Properties
    {
		//_screenIndex("Screen Index", Int) = 1
		//_screenSpace("Screen Space", Float) = 1.00
		_MainTex ("Texture", 2D) = "white" {}
		_WarpTex("WarpTexture", 2D) = "" {}

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
			sampler2D _WarpTex;


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
				fixed4 col = float4(0, 0, 0,0);
								
				v2f sampleCoords = i;
				float4 warpColour = tex2D(_WarpTex, sampleCoords.uv);

                //if (warpColour[2] == 0.0f) {
                //    col = float4(0.0,0.0f,0.0f,0.0f);
                //}
                //else {
                    t.uv[0] = warpColour[0];
                    t.uv[1] = warpColour[1];

                    col = tex2D(_MainTex, t.uv);
                //}
				
				return col;
			}
        
            ENDCG
        }
    }
}
