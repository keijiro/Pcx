Shader "PCX/Point Primitive"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
        _PointSize("Point Size", Float) = 0.05
        [Toggle] _PSize("Enable Point Size", Float) = 1
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
            #pragma multi_compile _ _PSIZE_ON
            #pragma multi_compile _ _COMPUTE_BUFFER

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
        #ifdef _PSIZE_ON
                float psize : PSIZE;
        #endif
                UNITY_FOG_COORDS(1)
            };

            half4 _Color;
            float4x4 _Transform;
            half _PointSize;

        #if _COMPUTE_BUFFER
            StructuredBuffer<float4> _PositionBuffer;
            StructuredBuffer<uint> _ColorBuffer;
        #endif

        #if _COMPUTE_BUFFER
            Varyings Vertex(uint vid : SV_VertexID)
        #else
            Varyings Vertex(Attributes input)
        #endif
            {
        #if _COMPUTE_BUFFER
                float4 pos = mul(_Transform, float4(_PositionBuffer[vid].xyz, 1));
                half4 col = UnpackColor32(_ColorBuffer[vid]);
        #else
                float4 pos = input.position;
                half4 col = input.color;
        #endif
                Varyings o;
                o.position = UnityObjectToClipPos(pos);
                o.color = col * _Color;
        #ifdef _PSIZE_ON
                o.psize = _PointSize / o.position.w * _ScreenParams.y;
        #endif
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
