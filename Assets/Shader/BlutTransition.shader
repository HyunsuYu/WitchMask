Shader "Unlit/BlutTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _OuterMass ("Outer Mass", Range(0.0, 1.0)) = 1.0
        _InnerMass ("Inner Mass", Range(0.0, 1.0)) = 0.0

        _MeltAmount ("Melt", float) = 1.0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
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
            float4 _MainTex_ST;

            float _OuterMass;
            float _InnerMass;

            float _MeltAmount;

            float getMetaball(fixed2 a, fixed2 b);

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float outer = saturate(pow(_OuterMass * getMetaball(i.uv, fixed2(0.5, 0.5)), _MeltAmount));
                float inner = saturate(pow(_InnerMass * getMetaball(i.uv, fixed2(0.5, 0.5)), _MeltAmount));

                return fixed4(1.0, 1.0, 1.0, outer - inner);
            }

            float getMetaball(fixed2 a, fixed2 b)
            {
                return 1.0 / (length(a - b) * length(a - b));
            }
            ENDCG
        }
    }
}
