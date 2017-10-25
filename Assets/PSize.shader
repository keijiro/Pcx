Shader "Plypc/Point Cloud (point primitives)"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
        _BaseSize("Point Size", Float) = 0.1
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
                fixed4 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                float psize : PSIZE;
                UNITY_FOG_COORDS(1)
            };

            half4 _Color;
            half _BaseSize;

            Varyings Vertex(Attributes input)
            {
                Varyings o;
                o.position = UnityObjectToClipPos(input.position);
                o.color = input.color * fixed4(_Color.rgb * 2, _Color.a);
                o.psize = _BaseSize / o.position.w * _ScreenParams.y;
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            fixed4 Fragment(Varyings input) : SV_Target
            {
                fixed4 c = input.color;
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
}
