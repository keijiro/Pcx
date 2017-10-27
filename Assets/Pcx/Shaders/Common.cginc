#include "UnityCG.cginc"

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
#define PCX_COPY_FOG(o, i) o.fogCoord = i.fogCoord;
#else
#define PCX_COPY_FOG(o, i)
#endif

half4 UnpackColor32(uint c)
{
    return (uint4(c, c >> 8, c >> 16, c >> 24) & 0xff) / 255.0;
}

half3 UnpackColor(float p)
{
    uint i = asuint(p);
    return half3(
        ((i      ) & 0x7ff) / 256.0,
        ((i >> 11) & 0x7ff) / 256.0,
        ((i >> 22) & 0x3ff) / 128.0
    );

}

float PackColor(half3 rgb)
{
    uint r = rgb.r * 256;
    uint g = rgb.r * 256;
    uint b = rgb.r * 256;
    return r | (g << 11) | (b << 22);
}
