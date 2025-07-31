Shader "Unlit/BlurShader2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Size", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _BlurSize;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 pixelSize = float2(_BlurSize / _ScreenParams.x, _BlurSize / _ScreenParams.y);

                fixed4 col = tex2D(_MainTex, uv) * 0.2;
                col += tex2D(_MainTex, uv + pixelSize * float2(1, 0)) * 0.15;
                col += tex2D(_MainTex, uv - pixelSize * float2(1, 0)) * 0.15;
                col += tex2D(_MainTex, uv + pixelSize * float2(0, 1)) * 0.15;
                col += tex2D(_MainTex, uv - pixelSize * float2(0, 1)) * 0.15;
                col += tex2D(_MainTex, uv + pixelSize * float2(1, 1)) * 0.1;
                col += tex2D(_MainTex, uv + pixelSize * float2(-1, -1)) * 0.1;

                return col;
            }
            ENDCG
        }
    }
}