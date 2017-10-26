Shader "PCX/Geometry Shader"
{
    Properties
    {
        [HDR] _Color("Tint", Color) = (1, 1, 1, 1)
        _PointSize("Point Size", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Cull Off

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
                float2 extent = abs(UNITY_MATRIX_P._11_22 * _PointSize);

                // Copy the basic information.
                Varyings o;
                o.color = input[0].color;
                PCX_COPY_FOG(o, input[0]);

                // Determine the number of slices based on the radius of the
                // point on the screen.
                float radius = extent.y / origin.w * _ScreenParams.y;
                uint slices = min((radius + 1) / 5, 4) + 2;

                // Slightly enlarge quad points to compensate area reduction.
                // Hopefully this line would be complied without branch.
                if (slices == 2) extent *= 1.2;

                // Top vertex
                o.position.y = origin.y + extent.y;
                o.position.xzw = origin.xzw;
                outStream.Append(o);

                UNITY_LOOP for (uint i = 1; i < slices; i++)
                {
                    float sn, cs;
                    sincos(UNITY_PI / slices * i, sn, cs);

                    // Right side vertex
                    o.position.xy = origin.xy + extent * float2(sn, cs);
                    outStream.Append(o);

                    // Left side vertex
                    o.position.x = origin.x - extent.x * sn;
                    outStream.Append(o);
                }

                // Bottom vertex
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
