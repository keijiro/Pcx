#include "UnityCG.cginc"

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
#define PCX_COPY_FOG(o, i) o.fogCoord = i.fogCoord;
#else
#define PCX_COPY_FOG(o, i)
#endif
