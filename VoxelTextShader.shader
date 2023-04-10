Shader "Unlit/VoxelTextShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DeadTime("Dead Time", float) = 1.0
        _BaseLifeTime("Base Life Time", float) = 3.0
        _Progress("Hide value", float) = 0.95
        _ElapsedTime("Elapsed time", float) = 0

        [Enum(Zero,0, One,1, SrcColor,3,     SrcAlpha,5,   OneMinusSrcAlpha,10)]
        _BlendingModeScr("BlendingSrc", Float) = 1.0

        [Enum(Zero,0, One,1, SrcColor,3,     SrcAlpha,5,   OneMinusSrcAlpha,10)]
        _BlendingModeDst("BlendingDest", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry"}
        LOD 100
        Lighting Off
        AlphaTest Off
        Blend[_BlendingModeScr][_BlendingModeDst]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float4 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                //float4 color: COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NoiseTex;
            float4 _NoiseTex_ST;

            float _DeadTime;
            float _BaseLifeTime;
            float _Progress;
            uniform float _ElapsedTime;

            //float _NaN;

            v2f vert (appdata v)
            {
                v2f o;
                float2 noiseUV = v.color.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw; 
                float4 tex = tex2Dlod(_NoiseTex, float4(noiseUV, 0, 0));
                float ttl = tex.r + _BaseLifeTime;
                float timer = smoothstep( ttl, ttl + _DeadTime, _ElapsedTime);
                v.vertex.xyz -= v.normal * timer;

                v.vertex = lerp(v.vertex, float4(0,0,0,0), step((ttl + _DeadTime) * _Progress, _ElapsedTime));

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
