Shader "IglooVision/3D_UI_Display_EXPERIMENTAL"
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
            float4 _MainTex_ST; // TilingX : [0], TilingY : [1], offsetX : [2], offsetY : [3]
            float scaleToRange(float value) {
                float OldRange = (2 - 0);
                float NewRange = (1 - (-1));
                float NewValue = (((value - 0) * NewRange) / OldRange) + (-1);

                return(NewValue);

                }
            float Remap( float value, float from1, float to1, float from2, float to2) {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                int multiplier = 0.0f;
                //float2 newOffset = _MainTex_ST.zw;
                //newOffset.x= Remap(newOffset.x, -1, 1, 0, 2);
                //if (fmod(_MainTex_ST.z, 2) == 0.0f)
                //    multiplier = 1.0f;
                //
                //float2 offsetLoop = _MainTex_ST.zw;
                //offsetLoop.x = fmod(offsetLoop.x, 1) + multiplier;
               //// offsetLoop.x = scaleToRange(offsetLoop.x);
                //o.uv = v.uv.xy * _MainTex_ST.xy + offsetLoop;
                //o.uv.x = Remap(o.uv.x, 0, 2, -1, 1);
                //_MainTex_ST.z = fmod(_MainTex_ST.z, _MainTex_ST.x);
                //_MainTex_ST.z = fmod(_MainTex_ST.z, _MainTex_ST.x);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                 half4 c = tex2D(_MainTex,i.uv);

                //if (i.uv.x > 1)
                //    i.uv.x = -1 + fmod(i.uv.x, 1.0f);
                 //c.r = _MainTex_ST.x;
                 //c.g = _MainTex_ST.y;
                 //c.b = _MainTex_ST.z;
                 //c.a = _MainTex_ST.w;
                 if (i.uv.x > 0.0f) {

                    if (fmod(i.uv.x, _MainTex_ST.x) > _MainTex_ST.x * (1.0f / _MainTex_ST.x)) {

                        c = half4 (1, 0, 0, 1);
                    }
                 }
                 else {
                     float x = fmod(i.uv.x, -1 * _MainTex_ST.x);
                     if(x < 0.0f && x > -_MainTex_ST.x * (1.0f / _MainTex_ST.x))
                        c = half4 (0, 0, 1, 1);
                    //float x = fmod(i.uv.x, -1 * _MainTex_ST.x);
                    //if (x < -_MainTex_ST.x + (  _MainTex_ST.x * (1.0f / _MainTex_ST.x))) {
                    //
                    //    c = half4 (0, 0, 1, 1);
                    //}
                 }
                //if(-fmod(i.uv.x, _MainTex_ST.x) < -_MainTex_ST.x + (1.0f / _MainTex_ST.x)) {
                //
                //    c = half4 (1, 0, 0, 1);
                //}
                 //if (i.uv.x < 0.0f){
                 //
                 //    c = half4 (0, 0, 1, 1);
                 //}
               // if (i.uv.x> 1.0f && i.uv.x  < _MainTex_ST.x) {
               //      
               //      c = half4 (1, 0, 0, 1);
               // }
               // if (i.uv.x  < 0.0f && i.uv.x > -_MainTex_ST.x) {
               //
               //     c = half4 (1, 0, 0, 1);
               // }
                 //if (i.uv.x < 2.0f && i.uv.x > -2.0f) {
                 //   if (i.uv.x <= 0.0f || i.uv.y <= 0.0f || i.uv.x >= 1.0f || i.uv.y >= 1.0f)
                 //       c = half4 (1,0,0,1);
                 //}
                //if (i.uv.x <= 0.0f || i.uv.y <= 0.0f || i.uv.x >= 1.0f || i.uv.y >= 1.0f)
                //    c = half4 (1, 0, 0, 1);
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
