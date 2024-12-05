Shader "Igloo/Warp"
{
    Properties
    {
		//_screenIndex("Screen Index", Int) = 1
		//_screenSpace("Screen Space", Float) = 1.00
		_canvasPosU("Canvas U Start", Range(0.00,1.00)) = 0.00
		_canvasWidth("Canvas Subsection Width", Float) = 1.00
		_overlapLeft("Left Overlap", Range(0.00,1.00)) = 0.00
		_overlapRight("Right Overlap", Range(0.00,1.00)) = 0.00
		_displayPosX("Display Pos X", Range(0.00,1.00)) = 0.00
		_displayPosY("Display Pos Y", Range(0.00,1.00)) = 0.00
		_displayScaleX("Display Scale X", Range(0.00,1.00)) = 0.00
		_displayScaleY("Display Scale Y", Range(0.00,1.00)) = 0.00
		_stereoMode("Stereo Mode", Float) = 0.00
		_MainTex ("Texture", 2D) = "white" {}
		_LeftEyeTex("LeftEyeTexture", 2D) = "" {}
		_RightEyeTex("RightEyeTexture", 2D) = "" {}		
		_WarpTex("WarpTexture", 2D) = "" {}
		_BlendTex("BlendTexture", 2D) = "" {}
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
			sampler2D _BlendTex;
			sampler2D _LeftEyeTex;
			sampler2D _RightEyeTex;
			uniform float _stereoMode;
			uniform float _canvasPosU;
			uniform float _canvasWidth;
			uniform float _overlapLeft;
			uniform float _overlapRight;
			uniform float _displayPosX;
			uniform float _displayPosY;
			uniform float _displayScaleX;
			uniform float _displayScaleY;
			uniform float _testAlpha;

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
				
				// Cull fragment if outside the rectangle.
				if (i.uv[0] <= _displayPosX || i.uv[1] <= _displayPosY || i.uv[0] >= _displayPosX + _displayScaleX || i.uv[1] >= _displayPosY + _displayScaleY) {
					col = half4 (0, 0, 0, 0.0);
				}
				else {

				
					v2f sampleCoords = i;
					// side-by-side
					if (_stereoMode == 1.0f) sampleCoords.uv[0] = sampleCoords.uv[0] * 2.0f;
					// top-bottom
					else if (_stereoMode == 2.0f) sampleCoords.uv[1] = sampleCoords.uv[1] * 2.0f;

					float scaleX = _displayScaleX == 0.0f ? 1.0f : 1.0f / max(_displayScaleX, 0.01);
					float scaleY = _displayScaleY == 0.0f ? 1.0f : 1.0f / max(_displayScaleY, 0.01);

					sampleCoords.uv[0] = (sampleCoords.uv[0] -_displayPosX) * scaleX; // Scale and translate X
					//sampleCoords.uv[0] -= _displayPosX * scaleX;	  //Translate X
				
				    sampleCoords.uv[1] = (sampleCoords.uv[1] -_displayPosY) * scaleY; // Scale and translate Y
				   // sampleCoords.uv[1] -= _displayPosY * scaleY;      //Translate Y

					float4 warpColour = tex2D(_WarpTex, sampleCoords.uv);
					float4 blendColour = tex2D(_BlendTex, sampleCoords.uv);

					float range = (1.0f + _overlapLeft + _overlapRight) * _canvasWidth;
					float start = (_canvasPosU + _canvasWidth * 0.5) - (_canvasWidth * 0.5f + _overlapLeft * _canvasWidth);
				
					t.uv[0] = start + range * warpColour[0];
					t.uv[1] = warpColour[1];

					// side-by-side
					if (_stereoMode == 1.0f) {
						// left eye
						if (i.uv[0] < _displayPosX + (_displayScaleX*0.5f)) col = tex2D(_LeftEyeTex, t.uv) * blendColour;
						// right eye
						else col = tex2D(_RightEyeTex, t.uv) * blendColour;
					}
					else if (_stereoMode == 2.0f) {
						// left eye
						if (i.uv[1] < _displayPosY + (_displayScaleY * 0.5f)) col = tex2D(_LeftEyeTex, t.uv) * blendColour;
						// right eye
						else col = tex2D(_RightEyeTex, t.uv) * blendColour;
					}
					else col = tex2D(_MainTex, t.uv) * blendColour;

				}
				return col;
			}
        
            ENDCG
        }
    }
}
