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
