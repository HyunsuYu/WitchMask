Shader "Unlit/MaskSlotMetaball"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BaseColor ("Base Color", Color) = (0,0,0,0)
        _MeltAmount ("Melt Amount", float) = 1

        _CurMaskSlotPos ("Cur Mask Slot Pos", Vector) = (0.8, 0.8, 0, 0)
        _CurMaskSlotMass ("Cur Mask Slot Mass", float) = 1

        _SlotPos ("_SlotPos", Vector) = (0.8, 0.8, 0, 0)
        _SlotMass ("Slot Mass", float) = 1
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

            fixed4 _BaseColor;
            float _MeltAmount;

            float4 _CurMaskSlotPos;
            float _CurMaskSlotMass;

            int _BIsSlotActive;
            float4 _SlotPos;
            float _SlotMass;

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
                float sum = 0.0;

                sum += pow(_CurMaskSlotMass * getMetaball(i.uv, _CurMaskSlotPos.xy), _MeltAmount);
                // if(_BIsSlotActive == 1)
                // {
                    sum += pow(_SlotMass * getMetaball(i.uv, _SlotPos.xy), _MeltAmount);
                // }

                sum = saturate(sum);

                if(sum <= 0.5)
                {
                    sum = 0.0;
                }
                else
                {
                    sum = 1.0;
                }

                return fixed4(_BaseColor.r, _BaseColor.g, _BaseColor.b, sum);
            }

            float getMetaball(fixed2 a, fixed2 b)
            {
                return 1.0 / (length(a - b) * length(a - b));
            }
            ENDCG
        }
    }
}
