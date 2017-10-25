Shader "Plypc/Point Cloud (geometry shader)"
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
            #pragma geometry Geometry
            #pragma fragment Fragment
            #pragma multi_compile_fog

            #include "Common.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                fixed4 color : COLOR;
            };

            struct GeometryInput
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                UNITY_FOG_COORDS(1)
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
                UNITY_FOG_COORDS(1)
            };

            half4 _Color;
            half _BaseSize;

            GeometryInput Vertex(Attributes input)
            {
                GeometryInput o;
                o.position = UnityObjectToClipPos(input.position);
                o.color = input.color * fixed4(_Color.rgb * 2, _Color.a);
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            [maxvertexcount(36)]
            void Geometry(point GeometryInput input[1], inout TriangleStream<Varyings> outStream)
            {
                float4 origin = input[0].position;
                float2 extent = UNITY_MATRIX_P._11_22 * _BaseSize;

                Varyings o;
                o.color = input[0].color;
                COPY_FOG(o, input[0]);

                // top
                o.position.y = origin.y + extent.y;
                o.position.xzw = origin.xzw;
                outStream.Append(o);

                uint div = saturate(abs(extent.y / origin.w * 25)) * 4 + 2;

                UNITY_LOOP for (uint i = 1; i < div; i++)
                {
                    float sn, cs;
                    sincos(UNITY_PI / div * i, sn, cs);

                    // right
                    o.position.xy = origin.xy + extent * float2(sn, cs);
                    outStream.Append(o);

                    // left
                    o.position.x = origin.x - extent.x * sn;
                    outStream.Append(o);
                }

                // bottom
                o.position.x = origin.x;
                o.position.y = origin.y - extent.y;
                outStream.Append(o);

                outStream.RestartStrip();
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
