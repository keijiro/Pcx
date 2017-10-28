// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

Shader "Point Cloud/Disk"
{
    Properties
    {
        _Color("Tint", Color) = (0.5, 0.5, 0.5, 1)
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
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "Common.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                half3 color : COLOR;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                half3 color : COLOR;
                UNITY_FOG_COORDS(0)
            };

            half4 _Color;
            float4x4 _Transform;
            half _PointSize;

        #if _COMPUTE_BUFFER
            StructuredBuffer<float4> _PointBuffer;
        #endif

        #if _COMPUTE_BUFFER
            Varyings Vertex(uint vid : SV_VertexID)
        #else
            Varyings Vertex(Attributes input)
        #endif
            {
            #if _COMPUTE_BUFFER
                float4 pt = _PointBuffer[vid];
                float4 pos = mul(_Transform, float4(pt.xyz, 1));
                half3 col = PcxDecodeColor(asuint(pt.w));
            #else
                float4 pos = input.position;
                half3 col = input.color;
            #endif

            #ifdef UNITY_COLORSPACE_GAMMA
                col *= _Color.rgb * 2;
            #else
                col *= LinearToGammaSpace(_Color.rgb) * 2;
                col = GammaToLinearSpace(col);
            #endif

                Varyings o;
                o.position = UnityObjectToClipPos(pos);
                o.color = col;
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
                half4 c = half4(input.color, _Color.a);
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
    CustomEditor "Pcx.DiskMaterialInspector"
}
