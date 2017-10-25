#include "UnityCG.cginc"

#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
#define COPY_FOG(o, i) o.fogCoord = i.fogCoord;
#else
#define COPY_FOG(o, i)
#endif
