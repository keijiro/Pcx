Shader "Hidden/PCX/Disc Shader"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off

        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile_fog

            #include "Common.cginc"

            struct Triangle
            {
                float4 vertices[3];
                float4 color;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                half4 color : COLOR;
                UNITY_FOG_COORDS(1)
            };

            StructuredBuffer<Triangle> _TriangleBuffer;

            Varyings Vertex(uint vid : SV_VertexID)
            {
                Triangle t = _TriangleBuffer[vid / 3];
                Varyings o;
                o.position = t.vertices[vid % 3];
                o.color = t.color;
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
