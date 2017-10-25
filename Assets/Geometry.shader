Shader "Point Cloud/Geometry Shader"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
        _PointSize("Point Size", Float) = 0.05
        _PointLod("Point LOD", Float) = 0.5
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
                half4 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                half4 color : COLOR;
                UNITY_FOG_COORDS(1)
            };

            half4 _Color;
            half _PointSize;
            half _PointLod;

            Varyings Vertex(Attributes input)
            {
                Varyings o;
                o.position = UnityObjectToClipPos(input.position);
                o.color = input.color * _Color;
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            [maxvertexcount(36)]
            void Geometry(point Varyings input[1], inout TriangleStream<Varyings> outStream)
            {
                float4 origin = input[0].position;
                float2 extent = UNITY_MATRIX_P._11_22 * _PointSize;

                Varyings o;
                o.color = input[0].color;
                COPY_FOG(o, input[0]);

                // top
                o.position.y = origin.y + extent.y;
                o.position.xzw = origin.xzw;
                outStream.Append(o);

                // LOD calculation based on screen space radius
                float lod = saturate(extent.y / origin.w * _PointLod * 50);
                uint div = lod * 4 + 2;

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
