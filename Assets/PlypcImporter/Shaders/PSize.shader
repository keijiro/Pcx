Shader "Point Cloud/Point Primitive"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
        _PointSize("Point Size", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile_fog

            #include "Common.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                half4 color : COLOR;
                float psize : PSIZE;
                UNITY_FOG_COORDS(1)
            };

            half4 _Color;
            half _PointSize;

            Varyings Vertex(Attributes input)
            {
                Varyings o;
                o.position = UnityObjectToClipPos(input.position);
                o.color = input.color * _Color;
                o.psize = _PointSize / o.position.w * _ScreenParams.y;
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                half4 c = input.color;
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
}
